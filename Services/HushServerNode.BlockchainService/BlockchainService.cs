using Microsoft.Extensions.Logging;

namespace HushServerNode.BlockchainService;

public class BlockchainService : IBlockchainService
{
    private readonly ILogger<BlockchainService> _logger;

    public BlockchainService(ILogger<BlockchainService> logger)
    {
        this._logger = logger;
    }

    public Task InitializeBlockchainAsync()
    {
        this._logger.LogInformation("Initializing Blockchain...");
        return Task.CompletedTask;
    }
}
