using Shared.Config;
using Shared.Logging.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConfigProviders;

public class LocalFileConfigProvider : BaseConfigProvider
{
    private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";

    public LocalFileConfigProvider(ILogger logger): base(logger){}

    protected override AppConfig GetConfigInternal()
    {
        Logger.LogInfo($"Getting config from file {ConfigPath}");

        if (!File.Exists(ConfigPath))
        {
            Logger.LogWarn("No config file");
            return new AppConfig();
        }

        AppConfig? result = null;

        try
        {
            result = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(ConfigPath));
        }
        catch (JsonException e)
        {
            Logger.LogError(e.Message);
        }

        return result ?? new AppConfig();
    }

    protected override void SetConfigInternal(AppConfig appConfig)
    {
        Logger.LogInfo($"Writing config to file {ConfigPath}");

        File.WriteAllText(ConfigPath,
            JsonSerializer.Serialize(appConfig,
                new JsonSerializerOptions
                    { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));
    }
}