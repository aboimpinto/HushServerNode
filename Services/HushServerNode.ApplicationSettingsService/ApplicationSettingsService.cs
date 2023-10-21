using System.Reflection;
using HushServerNode.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HushServerNode.ApplicationSettingsService;

public class ApplicationSettingsService : IApplicationSettingsService
{
    private const string ApplicationSettingsFileName = "ApplicationSettings.json";
    private readonly IServerInfo _serverInfo;
    private readonly IStackerInfo _stackerInfo;
    private readonly ILogger<ApplicationSettingsService> _logger;

    public ApplicationSettingsService(
        IServerInfo serverInfo,
        IStackerInfo stackerInfo,
        ILogger<ApplicationSettingsService> logger)
    {
        this._serverInfo = serverInfo;
        this._stackerInfo = stackerInfo;
        this._logger = logger;
    }

    public void LoadSettings()
    {
        this._logger.LogInformation("Load ApplicationSettings");

        var executingAssemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (string.IsNullOrEmpty(executingAssemblyFolder))
        {
            this._logger.LogError("Could not get the executing assembly folder. Settings aren't going to be load.");
            return;
        }

        var applicationSettingsFilePath = Path.Combine(
                executingAssemblyFolder, 
                "ApplicationSettings.json");

        var config = new ConfigurationBuilder()
            .AddJsonFile(applicationSettingsFilePath)
            .AddEnvironmentVariables()
            .Build();

        this.LoadServerInfo(config);
        this.LoadStakerInfo(config);
    }

    private void LoadServerInfo(IConfigurationRoot config)
    {
        var serverInfo = config
            .GetRequiredSection("ServerInfo")
            .Get<ServerInfo>();
        
        if (serverInfo == null)
        {
            throw new InvalidOperationException("Missing server information in ApplicationSetting.json");
        }

        this._serverInfo.ListeningPort = serverInfo.ListeningPort;
    }

    private void LoadStakerInfo(IConfigurationRoot config)
    {
        var stackerInfo = config
            .GetRequiredSection("StackerInfo")
            .Get<StackerInfo>();

        if (stackerInfo == null)
        {
            throw new InvalidOperationException("Missing stacker information in ApplicationSetting.json");
        }

        this._stackerInfo.PublicEncryptAddress = stackerInfo.PublicEncryptAddress;
        this._stackerInfo.PublicSigningAddress = stackerInfo.PublicSigningAddress;
    }
}
