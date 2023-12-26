using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Olimpo;

namespace HushServerNode.Services.NodeSettings
{
    public static class NodeSettingsHostBuilder
    {
        public static IHostBuilder RegisterNodeSettings(this IHostBuilder builder)
        {
            builder.ConfigureServices((hostContext, services) => 
            {
                var nodeSettingsService = new NodeSettingsService();
                services.AddSingleton(nodeSettingsService as IBootstrapper);
                services.AddSingleton<INodeSettingsService, NodeSettingsService>();
                services.AddSingleton<IBootstrapper>(x => 
                {
                    if (x.GetService<INodeSettingsService>() is not IBootstrapper bootstrapper)
                    {
                        throw new InvalidOperationException("NodeSettingsService doesn't implement IBottstrapper internet");
                    }

                    return bootstrapper;
                });

                // services.AddSingleton<IStackerInfo, StackerInfo>();
                // services.AddSingleton<IServerInfo, ServerInfo>();
                // services.AddSingleton<IBootstrapper, ApplicationSettingsService>(); 
            });

            return builder;
        } 
    }
}