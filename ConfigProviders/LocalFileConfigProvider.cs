using Shared.Logging.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.Config;

namespace ConfigProviders;

public class LocalFileConfigProvider : BaseConfigProvider
{
    private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";
    private readonly ILogger<LocalFileConfigProvider> _logger;

    public LocalFileConfigProvider(ILogger<LocalFileConfigProvider> logger) : base(logger)
    {
        _logger = logger;
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    { 
        WriteIndented = true, 
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    protected override AppConfig GetConfigInternal()
    {
        _logger.LogInfo($"Getting config from file {ConfigPath}");

        if (!File.Exists(ConfigPath))
        {
            _logger.LogWarn("No config file");
            return new AppConfig();
        }

        AppConfig? result = null;

        try
        {
            result = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(ConfigPath));
        }
        catch (JsonException e)
        {
            _logger.LogError(e.Message);
        }

        return result ?? new AppConfig();
    }

    protected override void SetConfigInternal(AppConfig appConfig)
    {
        _logger.LogInfo($"Writing config to file {ConfigPath}");

        File.WriteAllText(ConfigPath,
            JsonSerializer.Serialize(appConfig, _jsonOptions));
    }
}