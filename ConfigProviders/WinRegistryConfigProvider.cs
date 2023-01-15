﻿using Microsoft.Win32;
using Shared.Config;
using Shared.Logging.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.RegistryWrapper;

namespace ConfigProviders;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Planform check in constructor is sufficient")]
public class WinRegistryConfigProvider: IConfigProvider
{
    private readonly IRegistryKey _regKey;
    private const string ValueName = "Config";
    private readonly ILogger<WinRegistryConfigProvider> _logger;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public WinRegistryConfigProvider(IRegistry registry, ILogger<WinRegistryConfigProvider> logger)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        _logger = logger;

        _regKey = registry.CurrentUser.OpenSubKey("SOFTWARE")!.OpenSubKey("RemoteControl", true) ??
                  registry.CurrentUser.OpenSubKey("SOFTWARE", true)!.CreateSubKey("RemoteControl", true);
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

        _regKey.SetValue(ValueName, JsonSerializer.Serialize(config, _jsonOptions), RegistryValueKind.String);
    }
}