﻿using Microsoft.Win32;
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

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public WinRegistryConfigProvider(ILogger logger): base(logger)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        _regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE")!.OpenSubKey("RemoteControl", true) ?? 
                  Registry.CurrentUser.OpenSubKey("SOFTWARE", true)!.CreateSubKey("RemoteControl", true);
    }

    protected override SerializableAppConfig GetConfigInternal()
    {
        Logger.LogInfo($"Getting config from registry {_regKey}");

        var value = _regKey.GetValue(ValueName, null) as string;

        SerializableAppConfig? result = null;

        if (!string.IsNullOrWhiteSpace(value))
        {
            try
            {
                result = JsonSerializer.Deserialize<SerializableAppConfig>(value);
            }
            catch (JsonException e)
            {
                Logger.LogError(e.Message);
            }
        }

        return result ?? new SerializableAppConfig();
    }

    protected override void SetConfigInternal(SerializableAppConfig config)
    {
        Logger.LogInfo($"Writing config to registry {_regKey}");

        _regKey.SetValue(ValueName, JsonSerializer.Serialize(config, _jsonOptions), RegistryValueKind.String);
    }
}