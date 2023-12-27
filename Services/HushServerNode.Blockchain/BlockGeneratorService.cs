using System.Reactive.Linq;
using HushServerNode.ApplicationSettings;
using HushServerNode.Blockchain.Builders;
using HushServerNode.Blockchain.Events;
using Olimpo;

namespace HushServerNode.Blockchain;

public class BlockGeneratorService :
    IBlockGeneratorService,
    IHandle<BlockchainInitializedEvent>
{
    private readonly IBlockBuilder _blockBuilder;
    private readonly IBlockchainService _blockchainService;
    private readonly IMemPoolService _memPoolService;
    private readonly IApplicationSettingsService _applicationSettingsService;
    private readonly IEventAggregator _eventAggregator;

    private IObservable<long> _blockGeneratorLoop;

    public BlockGeneratorService(
        IBlockBuilder blockBuilder,
        IBlockchainService blockchainService,
        IMemPoolService memPoolService,
        IApplicationSettingsService applicationSettingsService,
        IEventAggregator eventAggregator)
    {
        this._blockBuilder = blockBuilder;
        this._blockchainService = blockchainService;
        this._memPoolService = memPoolService;
        this._applicationSettingsService = applicationSettingsService;
        this._eventAggregator = eventAggregator;

        this._eventAggregator.Subscribe(this);

        this._blockGeneratorLoop = Observable.Interval(TimeSpan.FromSeconds(3));
    }

    public void Handle(BlockchainInitializedEvent message)
    {
        this._blockGeneratorLoop.Subscribe(async x => 
        {
            var transactions = this._memPoolService.GetNextBlockTransactionsCandidate();

            var block = this._blockBuilder
                .WithBlockIndex(this._blockchainService.CurrentBlockIndex + 1)
                .WithBlockId(this._blockchainService.CurrentNextBlockId)
                .WithPreviousBlockId(this._blockchainService.CurrentBlockId)
                .WithNextBlockId(Guid.NewGuid().ToString())
                .WithRewardBeneficiary(this._applicationSettingsService.StackerInfo)
                .Build();

            await this._eventAggregator.PublishAsync(new BlockCreatedEvent(block));
        });
    }
}

public interface IBlockGeneratorService
{
    
}