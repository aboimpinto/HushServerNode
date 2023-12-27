using HushServerNode.Blockchain;
using HushServerNode.Blockchain.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Olimpo;

namespace HushServerNode;

public static class BlockchainHostBuilder
{
    public static IHostBuilder RegisterBlockchainService(this IHostBuilder builder)
    {
        builder.ConfigureServices((hostContext, services) => 
        {
            services.AddTransient<TransactionBaseConverter>();
            services.AddTransient<IBlockBuilder, BlockBuilder>();

            services.AddSingleton<IBootstrapper, BlockchainBootstrapper>();
            services.AddSingleton<IBlockchainService, BlockchainService>();

            services.AddSingleton<IMemPoolService, MemPoolService>();
            services.AddSingleton<IBlockGeneratorService, BlockGeneratorService>();
        });

        return builder;
    }
}
