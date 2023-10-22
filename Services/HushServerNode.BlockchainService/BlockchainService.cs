namespace HushServerNode.BlockchainService;

public class BlockchainService : IBlockchainService
{
    public Task InitializeBlockchainAsync()
    {
        return Task.CompletedTask;
    }
}
