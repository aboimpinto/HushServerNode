using HushServerNode.ApplicationSettingsService;
using HushServerNode.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Olimpo;

namespace HushServerNode;

public static class ApplicationSettingsHostBuilder
{
    public static IHostBuilder RegisterApplicationSettingsService(this IHostBuilder builder)
    {
        builder.ConfigureServices((hostContext, services) => 
        {
            services.AddSingleton<IBootstrapper, ApplicationSettingsBootstrapper>();

            services.AddSingleton<IStackerInfo, StackerInfo>();
            services.AddSingleton<IServerInfo, ServerInfo>();
            services.AddTransient<IApplicationSettingsService, ApplicationSettingsService.ApplicationSettingsService>();
        });

        return builder;
    }
}
