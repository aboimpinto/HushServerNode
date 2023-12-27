namespace HushServerNode.Blockchain;

public interface IBlockchainService
{
    Task InitializeBlockchainAsync();

    string CurrentBlockId { get; }
    double CurrentBlockIndex { get; }
    string CurrentNextBlockId { get; }
    string CurrentPreviousBlockId { get; }
}
