using HushServerNode.Blockchain.Model;

namespace HushServerNode.Blockchain;

public interface IMemPoolService
{
    Task InitializeMemPool();

    IEnumerable<TransactionBase> GetNextBlockTransactionsCandidate();
}