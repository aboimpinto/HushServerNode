using HushServerNode.BlockchainService;
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
            services.AddSingleton<IBootstrapper, BlockchainBootstrapper>();
            services.AddSingleton<IBlockchainService, BlockchainService.BlockchainService>();

            // services.AddSingleton<IBootstrapper, ApplicationSettingsBootstrapper>();

            // services.AddSingleton<IStackerInfo, StackerInfo>();
            // services.AddSingleton<IServerInfo, ServerInfo>();
            // services.AddTransient<IApplicationSettingsService, ApplicationSettingsService>();
        });

        return builder;
    }
}
