using HushServerNode.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Olimpo;

namespace HushServerNode.ApplicationSettingsService;

public static class ApplicationSettingsHostBuilder
{
    public static IHostBuilder RegisterApplicationSettingsService(this IHostBuilder builder)
    {
        builder.ConfigureServices((hostContext, services) => 
        {
            services.AddSingleton<IBootstrapper, ApplicationSettingsBootstrapper>();

            services.AddSingleton<IStackerInfo, StackerInfo>();
            services.AddSingleton<IServerInfo, ServerInfo>();
            services.AddTransient<IApplicationSettingsService, ApplicationSettingsService>();
        });

        return builder;
    }
}
