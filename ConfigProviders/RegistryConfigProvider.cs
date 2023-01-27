using Shared.Config;
using Shared.Logging.Interfaces;
using Shared.Wrappers.RegistryWrapper;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConfigProviders;

public class RegistryConfigProvider : IConfigProvider
{
    private readonly IRegistryKey _regKey;
    private const string ValueName = "Config";
    private const string KeyName = "RemoteControl";
    private readonly ILogger<RegistryConfigProvider> _logger;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public RegistryConfigProvider(IRegistry registry, ILogger<RegistryConfigProvider> logger)
    {
        _logger = logger;

        _regKey = registry.CurrentUser.OpenSubKey("SOFTWARE")?.OpenSubKey(KeyName, true) ??
                  registry.CurrentUser.OpenSubKey("SOFTWARE", true)?.CreateSubKey(KeyName, true) ??
                  throw new Exception("Registry branch not found");
    }

    public AppConfig GetConfig()
    {
        _logger.LogInfo($"Getting config from registry {_regKey}");

        var value = _regKey.GetValue(ValueName, null) as string;

        AppConfig? result = null;

        if (!string.IsNullOrWhiteSpace(value))
        {
            try
            {
                result = JsonSerializer.Deserialize<AppConfig>(value);
            }
            catch (JsonException e)
            {
                _logger.LogError(e.Message);
            }
        }

        return result ?? new AppConfig();
    }

    public void SetConfig(AppConfig config)
    {
        _logger.LogInfo($"Writing config to registry {_regKey}");

        _regKey.SetValue(ValueName, JsonSerializer.Serialize(config, _jsonOptions), RegValueType.String);
    }
}