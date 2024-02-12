using HushEcosystem.Model.Blockchain;

namespace HushServerNode.Blockchain;

public interface IMemPoolService
{
    Task InitializeMemPool();

    IEnumerable<VerifiedTransaction> GetNextBlockTransactionsCandidate();
}