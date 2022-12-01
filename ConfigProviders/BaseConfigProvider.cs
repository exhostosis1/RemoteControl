using Shared.Config;
using Shared.Logging.Interfaces;

namespace ConfigProviders;

public abstract class BaseConfigProvider: IConfigProvider
{
    protected enum ConfigSection
    {
        None,
        Server,
        Bot
    }

    private ConfigItem? _cachedConfig;

    protected readonly ILogger Logger;

    protected ConfigSection configSection = ConfigSection.None;

    #region Server
    protected const string ServerSectionName = "Server";

    protected const string SchemeName = "Scheme";
    protected const string HostName = "Host";
    protected const string PortName = "Port";
    protected const string ServerAutostartName = "Autostart";

    protected string Host = "localhost";
    protected string Scheme = "http";
    protected int Port = 80;
    protected bool ServerAutostart = false;
    #endregion

    #region Bot
    protected const string BotSectionName = "Bot";
    protected const string ChatIdsName = "ChatIds";
    protected const string BotAutostartName = "Autostart";

    protected bool BotAutostart = false;
    protected List<int> ChatIds = new();
    #endregion  

    protected BaseConfigProvider(ILogger logger)
    {
        Logger = logger;
    }

    public ConfigItem GetConfig()
    {
        if (_cachedConfig != null)
            return _cachedConfig;

        var result = GetConfigInternal();
        _cachedConfig = result;

        return result;
    }

    public void SetConfig(ConfigItem config)
    {
        SetConfigInternal(config);
        _cachedConfig = config;
    }

    protected abstract ConfigItem GetConfigInternal();
    protected abstract void SetConfigInternal(ConfigItem config);
}