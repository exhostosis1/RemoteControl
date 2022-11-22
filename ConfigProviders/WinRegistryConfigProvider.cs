using Microsoft.Win32;
using Shared.Logging.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

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

    protected override Uri GetConfigInternal()
    {
        var names = _regKey.GetValueNames();

        foreach (var name in names)
        {
            switch (name)
            {
                case HostName:
                    Host = _regKey.GetValue(name) as string ?? Host;
                    break;
                case PortName:
                    Port = _regKey.GetValue(name) as int? ?? Port;
                    break;
                case SchemeName:
                    Scheme = _regKey.GetValue(name) as string ?? Scheme;
                    break;
            }
        }

        try
        {
            return new UriBuilder(Scheme, Host, Port).Uri;
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            throw;
        }
    }

    protected override void SetConfigInternal(Uri config)
    {
        _regKey.SetValue(HostName, config.Host, RegistryValueKind.String);
        _regKey.SetValue(SchemeName, config.Scheme, RegistryValueKind.String);
        _regKey.SetValue(PortName, config.Port, RegistryValueKind.DWord);
    }
}