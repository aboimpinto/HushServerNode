using HushEcosystem.Model.Blockchain;

namespace HushServerNode.Blockchain;

public interface IBlockchainService
{
    string CurrentBlockId { get; }
    double CurrentBlockIndex { get; }
    string CurrentNextBlockId { get; }
    string CurrentPreviousBlockId { get; }

    Task InitializeBlockchainAsync();

    IEnumerable<TransactionBase> ListTransactionsForAddress(string address, int lastHeightSynched);
}
