using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using HushEcosystem.Model;
using HushEcosystem.Model.Blockchain;
using HushEcosystem.Model.Builders;
using HushServerNode.ApplicationSettings;
using HushServerNode.Blockchain.Builders;
using HushServerNode.Blockchain.Events;
using HushServerNode.Blockchain.ExtensionMethods;
using HushServerNode.Blockchain.IndexStrategies;
using Microsoft.Extensions.Logging;
using Olimpo;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Org.BouncyCastle.Utilities.Zlib;

namespace HushServerNode.Blockchain;

public class BlockchainService : 
    IBootstrapper, 
    IBlockchainService,
    IHandle<BlockCreatedEvent>
{
    private readonly ILogger<BlockchainService> _logger;

    private readonly List<Block> _blockchain = new List<Block>();

    private double _lastBlockIndex;

    private string _lastBlockId = string.Empty;
    private readonly IEventAggregator _eventAggregator;
    private readonly IBlockBuilder _blockBuilder;
    private readonly IApplicationSettingsService _applicationSettingsService;
    private readonly TransactionBaseConverter _transactionBaseConverter;
    private readonly IBlockchainIndexDb _blockchainIndexDb;
    private readonly IEnumerable<IIndexStrategy> _indexStrategies;
    private Block _currentBlock;

    public string CurrentBlockId { get => this._currentBlock.BlockId; }
    public double CurrentBlockIndex { get => this._currentBlock.Index; }
    public string CurrentPreviousBlockId { get => this._currentBlock.PreviousBlockId; }
    public string CurrentNextBlockId { get => this._currentBlock.NextBlockId; }

    public BlockchainService(
        IEventAggregator eventAggregator,
        IBlockBuilder blockBuilder,
        IApplicationSettingsService applicationSettingsService,
        TransactionBaseConverter transactionBaseConverter,
        IBlockchainIndexDb blockchainIndexDb,
        IEnumerable<IIndexStrategy> indexStrategies, 
        ILogger<BlockchainService> logger)
    {
        this._eventAggregator = eventAggregator;
        this._blockBuilder = blockBuilder;
        this._applicationSettingsService = applicationSettingsService;
        this._transactionBaseConverter = transactionBaseConverter;
        this._blockchainIndexDb = blockchainIndexDb;
        this._indexStrategies = indexStrategies;
        this._logger = logger;

        this._eventAggregator.Subscribe(this);
    }

    public int Priority { get; set; } = 10;

    public Subject<bool> BootstrapFinished { get; }

    public async Task InitializeBlockchainAsync()
    {
        this._logger.LogInformation("Initializing Blockchain...");

        // HACK [AboimPinto]: Initialize blockchain from genesis 
        // Check local blockchain last block hash.
        this._lastBlockIndex = 0;

        if (this._lastBlockIndex == 0)
        {
            // It's a new Blockchain, need to create the genesis block.
            var generisBlockId = Guid.NewGuid().ToString(); 
            var genesisNextBlockId = Guid.NewGuid().ToString(); 
            var genesisBlockIndex = 1;

            var genesisBlock = this._blockBuilder
                .WithBlockIndex(genesisBlockIndex)
                .WithBlockId(generisBlockId)
                .WithNextBlockId(genesisNextBlockId)
                .WithRewardBeneficiary(this._applicationSettingsService.StackerInfo, genesisBlockIndex)
                .Build();

            this._blockchain.Add(genesisBlock);
            this._currentBlock = genesisBlock;
            this._logger.LogInformation("Creating Genesis Block - {0} | Next Block - {1}", this.CurrentBlockId, this.CurrentNextBlockId);

            this.IndexBlock(genesisBlock);

            await this._eventAggregator.PublishAsync(new BlockchainInitializedEvent(this.CurrentBlockId, this.CurrentNextBlockId, genesisBlockIndex));
        }
        else
        {
            throw new NotImplementedException("Working with a persistent Blockchain is not implemented yet.");
        }
    }

    public IEnumerable<VerifiedTransaction> ListTransactionsForAddress(string address, double lastHeightSynched)
    {
        if (this._blockchainIndexDb.GroupedTransactions.ContainsKey(address))
        {
            return this._blockchainIndexDb.GroupedTransactions[address]
                .Where(x => 
                    x.SpecificTransaction.Issuer == address && 
                    x.BlockIndex > lastHeightSynched)
                .OrderBy(x => x.BlockIndex);
        }

        return new List<VerifiedTransaction>();
    }

    public double GetBalanceForAddress(string address)
    {
        if (this._blockchainIndexDb.AddressBalance.ContainsKey(address))
        {
            return this._blockchainIndexDb.AddressBalance[address];
        }

        return 0;
    }

    public UserProfile GetUserProfile(string publicAddress)
    {
        return this._blockchainIndexDb.Profiles
            .SingleOrDefault(x => x.UserPublicSigningAddress == publicAddress);
    }

    public void Shutdown()
    {
    }

    public async Task Startup()
    {
        await this.InitializeBlockchainAsync();
    }

    public void Handle(BlockCreatedEvent message)
    {
        if (this.VerifyBlock(message.Block))
        {
            this._blockchain.Add(message.Block);
            this._currentBlock = message.Block;

            this.IndexBlock(message.Block);

            this._logger.LogInformation("Creating Block: {0} | Previous Block: {1} | Next Block: {2}", 
                this.CurrentBlockId, 
                this.CurrentPreviousBlockId,  
                this.CurrentNextBlockId);

            // TODO [AboimPinto]: Signal the MemPool the created event to remove the transactions from the MemPool.
        }
        else
        {
            // TODO [AboimPinto]: what we should do when the block is not verified?
        }
    }

    private bool VerifyBlock(Block block)
    {
        var blockJsonOptions = new JsonSerializerOptionsBuilder()
            .WithTransactionBaseConverter(this._transactionBaseConverter)
            .WithModifierExcludeSignature()
            .WithModifierExcludeBlockIndex()
            .Build();

        var blockGeneratorAddress = block.GetBlockGeneratorAddress();
        var blockChecked = block.CheckSignature(blockGeneratorAddress, blockJsonOptions);

        if (blockChecked)
        {
            // interate over the transactions and check the signature of each one.
            foreach(var transaction in block.Transactions)
            {
                var transactionJsonOptions = new JsonSerializerOptionsBuilder()
                    .WithTransactionBaseConverter(this._transactionBaseConverter)
                    .WithModifierExcludeBlockIndex()
                    .WithModifierExcludeSignature()
                    .Build();

                if (!transaction.CheckSignature(transaction.ValidatorAddress, transactionJsonOptions))
                {
                    blockChecked = false;
                    break;
                }
            }
        }

        return blockChecked;
    }

    private void IndexBlock(Block block)
    {
        foreach(var transaction in block.Transactions)
        {
            var indexStrategiesThatCanHandle = this._indexStrategies
                .Where(x => x.CanHandle(transaction));
                
            foreach (var item in indexStrategiesThatCanHandle)
            {
                item.Handle(transaction);
            }
        }
    }
}
