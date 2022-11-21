using Microsoft.Win32;
using Shared.Config;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Shared.Logging.Interfaces;

namespace ConfigProviders;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Planform check in constructor is sufficient")]
public class WinRegistryConfigProvider: BaseConfigProvider
{
    private readonly RegistryKey _regKey;

    public WinRegistryConfigProvider(ILogger logger): base(logger)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        _regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE")!.OpenSubKey("RemoteControl", true) ?? 
                  Registry.CurrentUser.OpenSubKey("SOFTWARE", true)!.CreateSubKey("RemoteControl", true);
    }

    protected override AppConfig GetConfigInternal()
    {
        var result = new AppConfig();

        var names = _regKey.GetValueNames();

        foreach (var name in names)
        {
            switch (name)
            {
                case HostName:
                    result.Host = _regKey.GetValue(name) as string ?? result.Host;
                    break;
                case PortName:
                    result.Port = _regKey.GetValue(name) as int? ?? result.Port;
                    break;
                case SchemeName:
                    result.Scheme = _regKey.GetValue(name) as string ?? result.Scheme;
                    break;
            }
        }

        return result;
    }

    protected override void SetConfigInternal(AppConfig config)
    {
        _regKey.SetValue(HostName, config.Host, RegistryValueKind.String);
        _regKey.SetValue(SchemeName, config.Scheme, RegistryValueKind.String);
        _regKey.SetValue(PortName, config.Port, RegistryValueKind.DWord);
    }
}