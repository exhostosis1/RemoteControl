using Shared.Config;
using Shared.Logging.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConfigProviders;

public class LocalFileConfigProvider : BaseConfigProvider
{
    private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";

    public LocalFileConfigProvider(ILogger logger): base(logger){}
    private readonly JsonSerializerOptions _jsonOptions = new()
    { 
        WriteIndented = true, 
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    protected override SerializableAppConfig GetConfigInternal()
    {
        Logger.LogInfo($"Getting config from file {ConfigPath}");

        if (!File.Exists(ConfigPath))
        {
            Logger.LogWarn("No config file");
            return new SerializableAppConfig();
        }

        SerializableAppConfig? result = null;

        try
        {
            result = JsonSerializer.Deserialize<SerializableAppConfig>(File.ReadAllText(ConfigPath));
        }
        catch (JsonException e)
        {
            Logger.LogError(e.Message);
        }

        return result ?? new SerializableAppConfig();
    }

    protected override void SetConfigInternal(SerializableAppConfig appConfig)
    {
        Logger.LogInfo($"Writing config to file {ConfigPath}");

        File.WriteAllText(ConfigPath,
            JsonSerializer.Serialize(appConfig, _jsonOptions));
    }
}