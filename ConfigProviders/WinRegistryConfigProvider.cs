using Microsoft.Win32;
using Shared.Config;
using Shared.Logging.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConfigProviders;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Planform check in constructor is sufficient")]
public class WinRegistryConfigProvider: BaseConfigProvider
{
    private readonly RegistryKey _regKey;
    private const string ValueName = "Config";

    public WinRegistryConfigProvider(ILogger logger): base(logger)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        _regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE")!.OpenSubKey("RemoteControl", true) ?? 
                  Registry.CurrentUser.OpenSubKey("SOFTWARE", true)!.CreateSubKey("RemoteControl", true);
    }

    protected override AppConfig GetConfigInternal()
    {
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
                Logger.LogError(e.Message);
            }
        }

        return result ?? new AppConfig();
    }

    protected override void SetConfigInternal(AppConfig config)
    {
        _regKey.SetValue(ValueName,
            JsonSerializer.Serialize(config,
                new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
            RegistryValueKind.String);
    }
}