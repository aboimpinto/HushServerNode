using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Olimpo;
using HushEcosystem;
using Grpc.Core;

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
                services.AddHostedService<Worker>();
            })
            .RegisterGrpc()
            .RegisterBootstrapperManager()
            .RegisterEventAggregatorManager()
            .RegisterTcpServer()
            .RegisterApplicationSettingsService()
            .RegisterBlockchainService()
            .RegisterServerService()
            .RegisterRpcModel()
            .RegisterRpcManager();
}

