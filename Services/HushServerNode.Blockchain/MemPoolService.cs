using System.Collections.Concurrent;
using HushEcosystem.Model;
using HushEcosystem.Model.Blockchain;
using HushEcosystem.Model.Builders;
using HushEcosystem.Model.Rpc.GlobalEvents;
using HushServerNode.ApplicationSettings;
using Microsoft.Extensions.Logging;
using Olimpo;

namespace HushServerNode.Blockchain;

public class MemPoolService: 
    IMemPoolService,
    IHandle<NewFeedRequestedEvent>
{
    private readonly IApplicationSettingsService _applicationSettingsService;
    private readonly TransactionBaseConverter _transactionBaseConverter;
    private readonly IEventAggregator _eventAggregator;
    private readonly ILogger<MemPoolService> _logger;

    private ConcurrentBag<VerifiedTransaction> _nextBlockTransactionsCandidate;

    // private IList<TransactionBase> _allBlockTransactionsCandidate;

    public MemPoolService(
        IApplicationSettingsService applicationSettingsService,
        TransactionBaseConverter transactionBaseConverter,
        IEventAggregator eventAggregator,
        ILogger<MemPoolService> logger)
    {
        this._applicationSettingsService = applicationSettingsService;
        this._transactionBaseConverter = transactionBaseConverter;
        this._eventAggregator = eventAggregator;
        this._logger = logger;

        this._eventAggregator.Subscribe(this);
    }
            
    public Task InitializeMemPool()
    {
        this._nextBlockTransactionsCandidate = new ConcurrentBag<VerifiedTransaction>();
        // this._allBlockTransactionsCandidate = new List<TransactionBase>();

        return Task.CompletedTask;
    }

    public IEnumerable<VerifiedTransaction> GetNextBlockTransactionsCandidate()
    {
        return this._nextBlockTransactionsCandidate.TakeAndRemove(1000);
    }

    public void Handle(NewFeedRequestedEvent message)
    {
        // TODO [AboimPinto] Verify if the transaction is valid before add to the MemPool
        // ...

        // Add the valid transaction to the MemPool 
        var verifiedTransaction = new VerifiedTransaction
        {
            SpecificTransaction = message.NewFeedRequest.Feed,
            ValidatorAddress = this._applicationSettingsService.StackerInfo.PublicSigningAddress
        };

        var hashJsonOptions = new JsonSerializerOptionsBuilder()
            .WithTransactionBaseConverter(this._transactionBaseConverter)
            .WithModifierExcludeSignature()
            .WithModifierExcludeBlockIndex()
            .WithModifierExcludeHash()
            .Build();

        var signJsonOptions = new JsonSerializerOptionsBuilder()
            .WithTransactionBaseConverter(this._transactionBaseConverter)
            .WithModifierExcludeSignature()
            .WithModifierExcludeBlockIndex()
            .Build();

        verifiedTransaction.HashObject(hashJsonOptions);
        verifiedTransaction.Sign(this._applicationSettingsService.StackerInfo.PrivateSigningKey, signJsonOptions);

        this._nextBlockTransactionsCandidate.Add(verifiedTransaction);
    }
}
