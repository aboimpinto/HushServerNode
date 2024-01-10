using System.Reactive.Subjects;
using HushEcosystem.Model;
using HushEcosystem.Model.Blockchain;
using HushServerNode.ApplicationSettings;
using HushServerNode.Blockchain.Builders;
using HushServerNode.Blockchain.Events;
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

    private Dictionary<string, List<TransactionBase>> _groupedTransactions;
    private Dictionary<string, double> _addressBalance;

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

    public IEnumerable<TransactionBase> ListTransactionsForAddress(string address, int lastHeightSynched)
    {
        if (this._groupedTransactions.ContainsKey(address))
        {
            return this._groupedTransactions[address]
                .Where(x => x.Issuer == address && x.BlockHeight > lastHeightSynched)
                .OrderBy(x => x.BlockHeight);
        }

        return new List<TransactionBase>();
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
        var blockValidator = block.Transactions.GetRewardTransaction();
        var blockValidatorAddress = blockValidator.Issuer;

        var blockChecked = ((ISignable)block).CheckSignature(blockValidatorAddress, this._transactionBaseConverter);

        if (blockChecked)
        {
            foreach(var transaction in block.Transactions)
            {
                if (!((ISignable)transaction).CheckSignature(transaction.Issuer, this._transactionBaseConverter))
                {
                    blockChecked = false;
                    break;
                }
            }
        }

        return blockChecked;
    }

    private void IndexBlock(Block signedBlock)
    {
        if(this._groupedTransactions == null)
        {
            this._groupedTransactions = new Dictionary<string, List<TransactionBase>>();
        }

        if (this._addressBalance == null)
        {
            this._addressBalance = new Dictionary<string, double>();
        }

        // Group transactions where a certain address is involved.
        foreach(var transaction in signedBlock.Transactions)
        {
            if (this._groupedTransactions.ContainsKey(transaction.Issuer))
            {
                this._groupedTransactions[transaction.Issuer].Add(transaction);
            }
            else
            {
                this._groupedTransactions.Add(transaction.Issuer, new List<TransactionBase> { transaction });
            }

            if(this._addressBalance.ContainsKey(transaction.Issuer))
            {
                if (transaction is BlockCreationTransaction blockCreationTransaction)
                {
                    this._addressBalance[transaction.Issuer] += blockCreationTransaction.Reward;
                }
            }
            else
            {
                if (transaction is BlockCreationTransaction blockCreationTransaction)
                {
                    this._addressBalance.Add(transaction.Issuer, blockCreationTransaction.Reward);
                }
            }
        }

    }
}
