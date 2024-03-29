using Shared.Config;
using Shared.Logging.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConfigProviders;

public class LocalFileConfigProvider(ILogger<LocalFileConfigProvider> logger, string filePath) : IConfigProvider
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public AppConfig GetConfig()
    {
        logger.LogInfo($"Getting config from file {filePath}");

        if (!File.Exists(filePath))
        {
            logger.LogWarn("No config file");
            return new AppConfig();
        }

        AppConfig? result = null;

        try
        {
            result = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(filePath));
        }
        catch (JsonException e)
        {
            logger.LogError(e.Message);
        }

        return result ?? new AppConfig();
    }

    public void SetConfig(AppConfig appConfig)
    {
        logger.LogInfo($"Writing config to file {filePath}");

        File.WriteAllText(filePath,
            JsonSerializer.Serialize(appConfig, _jsonOptions));
    }
}