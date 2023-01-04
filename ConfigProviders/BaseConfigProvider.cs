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

        AppConfig? result;

        try
        {
            result = GetConfigInternal();
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            throw;
        }

        _cachedConfig = result;

        return result;
    }

    public void SetConfig(AppConfig config)
    {
        try
        {
            SetConfigInternal(config);
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            throw;
        }

        _cachedConfig = config;
    }

    protected abstract AppConfig GetConfigInternal();

    protected abstract void SetConfigInternal(AppConfig config);
}