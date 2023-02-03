using Shared.Config;
using Shared.Logging.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConfigProviders;

public class LocalFileConfigProvider : IConfigProvider
{
    private readonly string _configPath;
    private readonly ILogger<LocalFileConfigProvider> _logger;

    public LocalFileConfigProvider(ILogger<LocalFileConfigProvider> logger, string filePath)
    {
        _configPath = filePath;
        _logger = logger;
    }

    public LocalFileConfigProvider(ILogger<LocalFileConfigProvider> logger): this(logger, Path.Combine(AppContext.BaseDirectory, "config.ini"))
    {}

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public AppConfig GetConfig()
    {
        _logger.LogInfo($"Getting config from file {_configPath}");

        if (!File.Exists(_configPath))
        {
            _logger.LogWarn("No config file");
            return new AppConfig();
        }

        AppConfig? result = null;

        try
        {
            result = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(_configPath));
        }
        catch (JsonException e)
        {
            _logger.LogError(e.Message);
        }

        return result ?? new AppConfig();
    }

    public void SetConfig(AppConfig appConfig)
    {
        _logger.LogInfo($"Writing config to file {_configPath}");

        File.WriteAllText(_configPath,
            JsonSerializer.Serialize(appConfig, _jsonOptions));
    }
}