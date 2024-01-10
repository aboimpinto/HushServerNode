using HushEcosystem.Model.Blockchain;

namespace HushServerNode.Blockchain;

public interface IMemPoolService
{
    Task InitializeMemPool();

    IEnumerable<TransactionBase> GetNextBlockTransactionsCandidate();
}