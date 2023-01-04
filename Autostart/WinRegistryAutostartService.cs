using Microsoft.Win32;
using Shared.Logging.Interfaces;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Autostart;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Platform check in constructor is sufficient")]
public class WinRegistryAutostartService : BaseAutostartService
{
    private readonly RegistryKey _regKey;
    private const string RegName = "Remote Control";
    private readonly string _regValue = $"\"{Process.GetCurrentProcess().MainModule?.FileName ?? throw new NullReferenceException()}\"";

    public WinRegistryAutostartService(ILogger logger): base(logger)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        _regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE")?.OpenSubKey("Microsoft")?.OpenSubKey("Windows")
            ?.OpenSubKey("CurrentVersion")?.OpenSubKey("Run", true) ?? throw new NullReferenceException("Cannot open autorun registry key");
    }

    public override bool CheckAutostart()
    {
        Logger.LogInfo("Checking win registry autorun");
        return _regKey.GetValue(RegName, "") as string == _regValue;
    }

    public override void SetAutostart(bool value)
    {
        Logger.LogInfo("Setting win registry autorun");

        _regKey.DeleteValue(RegName, false);

        if (value)
        {
            try
            {
                _regKey.SetValue(RegName, _regValue, RegistryValueKind.String);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }
    }
}