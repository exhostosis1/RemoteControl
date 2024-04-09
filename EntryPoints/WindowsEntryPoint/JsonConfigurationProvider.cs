using System.Text.Json;
using System.Text.Json.Serialization;
using MainApp.Config;
using Microsoft.Extensions.Logging;

namespace MainApp;

public class JsonConfigurationProvider(ILogger logger, string filePath) : IConfigProvider
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public AppConfig GetConfig()
    {
        logger.LogInformation("Getting config from file {filePath}", filePath);

        if (!File.Exists(filePath))
        {
            logger.LogWarning("No config file");
            return new AppConfig();
        }

        AppConfig? result = null;

        try
        {
            result = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(filePath));
        }
        catch (JsonException e)
        {
            logger.LogError("{message}", e.Message);
        }

        return result ?? new AppConfig();
    }

    public void SetConfig(AppConfig appConfig)
    {
        logger.LogInformation("Writing config to file {filePath}", filePath);

        File.WriteAllText(filePath,
            JsonSerializer.Serialize(appConfig, _jsonOptions));
    }
}