using Shared.Config;
using Shared.Logging.Interfaces;
using Shared.Wrappers.RegistryWrapper;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.Wrappers.Registry;

namespace ConfigProviders;

public class RegistryConfigProvider(IRegistry registry, ILogger<RegistryConfigProvider> logger) : IConfigProvider
{
    private readonly IRegistryKey _regKey = registry.CurrentUser.OpenSubKey("SOFTWARE")?.OpenSubKey(KeyName, true) ??
                  registry.CurrentUser.OpenSubKey("SOFTWARE", true)?.CreateSubKey(KeyName, true) ??
                  throw new Exception("Registry branch not found");
    private const string ValueName = "Config";
    private const string KeyName = "RemoteControl";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public AppConfig GetConfig()
    {
        logger.LogInformation($"Getting config from registry {_regKey}");

        var value = _regKey.GetValue(ValueName, null) as string;

        AppConfig? result = null;

        if (string.IsNullOrWhiteSpace(value)) return result ?? new AppConfig();
        try
        {
            result = JsonSerializer.Deserialize<AppConfig>(value);
        }
        catch (JsonException e)
        {
            logger.LogError(e.Message);
        }

        return result ?? new AppConfig();
    }

    public void SetConfig(AppConfig config)
    {
        logger.LogInformation($"Writing config to registry {_regKey}");

        _regKey.SetValue(ValueName, JsonSerializer.Serialize(config, _jsonOptions), RegValueType.String);
    }
}