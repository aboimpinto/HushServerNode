
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Olimpo;

namespace HushServerNode;

public class Program
{
    public static void Main()
    {
        CreateHostBuilder()
            .Build()
            .Run();
    }

    public static IHostBuilder CreateHostBuilder() => 
        Host.CreateDefaultBuilder()
            .UseSystemd()
            .ConfigureLogging(x => 
            {

            })
            .ConfigureServices((hostContext, services) => 
            {
                // services.AddTransient<ISpecificTransactionDeserializer, BlockCreationTransactionDeserializer>();

                // services.AddSingleton<IBlockCreateEventFactory, BlockCreateEventFactory>();
                // services.AddSingleton<IBlockBuilderFactory, BlockBuildFactory>();
                // services.AddTransient<TransactionBaseConverter>();

                services.AddHostedService<Worker>();
            })
            .RegisterBootstrapperManager()
            .RegisterEventAggregatorManager()
            .RegisterTcpServer()
            .RegisterApplicationSettingsService()
            .RegisterBlockchainService();
            // .RegisterApplicationSettings()
            // .RegisterTcpServer()
            // .RegisterServer()
            // .RegisterBlockGenerator()
            // .RegisterBlockchain()
            // .RegisterListener()
            // .RegisterMemPool();
}

