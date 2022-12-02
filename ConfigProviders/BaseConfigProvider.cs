using Shared.Config;
using Shared.Logging.Interfaces;

namespace ConfigProviders;

public abstract class BaseConfigProvider: IConfigProvider
{
    private AppConfig? _cachedConfig;

    protected readonly ILogger Logger;

    protected BaseConfigProvider(ILogger logger)
    {
        Logger = logger;
    }

    public AppConfig GetConfig()
    {
        if (_cachedConfig != null)
            return _cachedConfig;

        var result = GetConfigInternal();
        _cachedConfig = result;

        return result;
    }

    public bool SetConfig(AppConfig config)
    {
        try
        {
            SetConfigInternal(config);
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            return false;
        }

        _cachedConfig = config;
        return true;
    }

    protected abstract AppConfig GetConfigInternal();
    protected abstract void SetConfigInternal(AppConfig config);
}