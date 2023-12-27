using HushServerNode.ApplicationSettings;
using HushServerNode.Blockchain.Builders;
using HushServerNode.Blockchain.Events;
using HushServerNode.Blockchain.Model;
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

    private Block _currentBlock;
    
    public string CurrentBlockId { get => this._currentBlock.BlockId; }
    public double CurrentBlockIndex { get => this._currentBlock.Index; }
    public string CurrentPreviousBlockId { get => this._currentBlock.PreviousBlockId; }
    public string CurrentNextBlockId { get => this._currentBlock.NextBlockId; }

    public BlockchainService(
        IEventAggregator eventAggregator,
        IBlockBuilder blockBuilder,
        IApplicationSettingsService applicationSettingsService,
        ILogger<BlockchainService> logger)
    {
        this._eventAggregator = eventAggregator;
        this._blockBuilder = blockBuilder;
        this._applicationSettingsService = applicationSettingsService;
        this._logger = logger;

        this._eventAggregator.Subscribe(this);
    }

    public int Priority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
                .WithRewardBeneficiary(this._applicationSettingsService.StackerInfo)
                .Build();

            this._blockchain.Add(genesisBlock);
            this._currentBlock = genesisBlock;
            this._logger.LogInformation("Creating Genesis Block - {0} | Next Block - {1}", this.CurrentBlockId, this.CurrentNextBlockId);

            await this._eventAggregator.PublishAsync(new BlockchainInitializedEvent(this.CurrentBlockId, this.CurrentNextBlockId, genesisBlockIndex));
        }
        else
        {
            throw new NotImplementedException("Working with a persistent Blockchain is not implemented yet.");
        }
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
        this._blockchain.Add(message.Block);
        this._currentBlock = message.Block;

        this._logger.LogInformation("Creating Block: {0} | Previous Block: {1} | Next Block: {2}", 
            this.CurrentBlockId, 
            this.CurrentPreviousBlockId,  
            this.CurrentNextBlockId);

        // TODO [AboimPinto]: Signal the MemPool the created event to remove the transactions from the MemPool.
    }
}
