using Olimpo;

namespace HushServerNode.BlockchainService;

public class BlockchainBootstrapper : IBootstrapper
{
    private readonly IBlockchainService _blockchainService;

    public int Priority { get; set; } = 10;

    public BlockchainBootstrapper(IBlockchainService blockchainService)
    {
        this._blockchainService = blockchainService;
    }

    public void Shutdown()
    {
    }

    public async Task Startup()
    {
        await this._blockchainService.InitializeBlockchainAsync();
    }
}
