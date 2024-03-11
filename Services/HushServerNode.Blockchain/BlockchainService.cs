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
using Microsoft.Extensions.Logging;
using Olimpo;

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
    private Block _currentBlock;

    private Dictionary<string, List<VerifiedTransaction>> _groupedTransactions;
    private Dictionary<string, double> _addressBalance;
    private readonly IList<UserProfile> _profiles = new List<UserProfile>();

    public string CurrentBlockId { get => this._currentBlock.BlockId; }
    public double CurrentBlockIndex { get => this._currentBlock.Index; }
    public string CurrentPreviousBlockId { get => this._currentBlock.PreviousBlockId; }
    public string CurrentNextBlockId { get => this._currentBlock.NextBlockId; }

    public BlockchainService(
        IEventAggregator eventAggregator,
        IBlockBuilder blockBuilder,
        IApplicationSettingsService applicationSettingsService,
        TransactionBaseConverter transactionBaseConverter,
        ILogger<BlockchainService> logger)
    {
        this._eventAggregator = eventAggregator;
        this._blockBuilder = blockBuilder;
        this._applicationSettingsService = applicationSettingsService;
        this._transactionBaseConverter = transactionBaseConverter;
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
        if (this._groupedTransactions.ContainsKey(address))
        {
            return this._groupedTransactions[address]
                .Where(x => 
                    x.SpecificTransaction.Issuer == address && 
                    x.BlockIndex > lastHeightSynched)
                .OrderBy(x => x.BlockIndex);
        }

        return new List<VerifiedTransaction>();
    }

    public double GetBalanceForAddress(string address)
    {
        if (this._addressBalance.ContainsKey(address))
        {
            return this._addressBalance[address];
        }

        return 0;
    }

    public UserProfile GetUserProfile(string publicAddress)
    {
        return this._profiles
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
        if(this._groupedTransactions == null)
        {
            this._groupedTransactions = new Dictionary<string, List<VerifiedTransaction>>();
        }

        if (this._addressBalance == null)
        {
            this._addressBalance = new Dictionary<string, double>();
        }

        // Group transactions where a certain address is involved.
        foreach(var transaction in block.Transactions)
        {
            transaction.BlockIndex = block.Index;

            if (this._groupedTransactions.ContainsKey(transaction.SpecificTransaction.Issuer))
            {
                this._groupedTransactions[transaction.SpecificTransaction.Issuer].Add(transaction);
            }
            else
            {
                this._groupedTransactions.Add(transaction.SpecificTransaction.Issuer, new List<VerifiedTransaction> { transaction });
            }

            if(this._addressBalance.ContainsKey(transaction.SpecificTransaction.Issuer))
            {
                if (transaction.SpecificTransaction is IValueableTransaction valuableTransaction)
                {
                    this._addressBalance[transaction.SpecificTransaction.Issuer] += valuableTransaction.Value;
                }
            }
            else
            {
                if (transaction.SpecificTransaction is IValueableTransaction valuableTransaction)
                {
                    this._addressBalance.Add(transaction.SpecificTransaction.Issuer, valuableTransaction.Value);
                }
            }

            // TODO [AboimPinto]: this check and indexing should be done outside this class and maybe done using strategy pattern.
            if (transaction.SpecificTransaction.TransactionId == UserProfile.TypeCode)
            {
                var userProfile = (UserProfile)transaction.SpecificTransaction;
                
                var existingProfile = this._profiles.SingleOrDefault(x => x.UserPublicSigningAddress == userProfile.UserPublicSigningAddress);
                if (existingProfile == null)
                {
                    this._profiles.Add(userProfile);
                }
                else
                {
                    existingProfile.IsPublic = userProfile.IsPublic;
                }
            }
        }
    }
}
