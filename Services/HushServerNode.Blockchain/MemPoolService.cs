using HushEcosystem.Model.Blockchain;
using Microsoft.Extensions.Logging;

namespace HushServerNode.Blockchain;

public class MemPoolService: IMemPoolService
{
    private readonly ILogger<MemPoolService> _logger;

    private IEnumerable<TransactionBase> _nextBlockTransactionsCandidate;

    private IEnumerable<TransactionBase> _allBlockTransactionsCandidate;

    public MemPoolService(ILogger<MemPoolService> logger)
    {
        this._logger = logger;
    }
            

    public Task InitializeMemPool()
    {
        this._nextBlockTransactionsCandidate = new List<TransactionBase>();
        this._allBlockTransactionsCandidate = new List<TransactionBase>();

        return Task.CompletedTask;
    }

    public IEnumerable<TransactionBase> GetNextBlockTransactionsCandidate()
    {
        return this._nextBlockTransactionsCandidate;
    }
}
