using Shared.Config;
using Shared.Logging.Interfaces;

namespace ConfigProviders;

public abstract class BaseConfigProvider: IConfigProvider
{
    private SerializableAppConfig? _cachedConfig;

    private readonly ILogger _logger;

    protected BaseConfigProvider(ILogger logger)
    {
        _logger = logger;
    }

    public SerializableAppConfig GetSerializableConfig()
    {
        if (_cachedConfig != null)
            return _cachedConfig;

        SerializableAppConfig? result;

        try
        {
            result = GetConfigInternal();
        }
        catch (Exception e)
        {
            _logger.LogError(GetType(), e.Message);
            throw;
        }

        _cachedConfig = result;

        return result;
    }

    public AppConfig GetConfig() => new(GetSerializableConfig());

    public void SetSerializableConfig(SerializableAppConfig config)
    {
        try
        {
            SetConfigInternal(config);
        }
        catch (Exception e)
        {
            _logger.LogError(GetType(),e.Message);
            throw;
        }

        _cachedConfig = config;
    }

    public void SetConfig(AppConfig config) => SetSerializableConfig(new SerializableAppConfig(config));

    protected abstract SerializableAppConfig GetConfigInternal();

    protected abstract void SetConfigInternal(SerializableAppConfig config);
}