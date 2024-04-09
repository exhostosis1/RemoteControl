using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Shared.AutoStart;

namespace MainApp;

public class RegistryAutoStartService(ILogger logger)
    : IAutoStartService
{
    private readonly RegistryKey _regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE")?.OpenSubKey("Microsoft")?.OpenSubKey("Windows")
        ?.OpenSubKey("CurrentVersion")?.OpenSubKey("Run", true) ?? throw new NullReferenceException("Cannot open autorun registry key");
    private const string RegName = "Remote Control";
    private readonly string _regValue = $"\"{Process.GetCurrentProcess().MainModule?.FileName ?? throw new NullReferenceException()}\"";

    public bool CheckAutoStart()
    {
        logger.LogInformation("Checking win registry autorun");
        return _regKey.GetValue(RegName, "") as string == _regValue;
    }

    public void SetAutoStart(bool value)
    {
        logger.LogInformation("Setting win registry autorun");

        _regKey.DeleteValue(RegName, false);

        if (!value) return;
        try
        {
            _regKey.SetValue(RegName, _regValue, RegistryValueKind.String);
        }
        catch (Exception e)
        {
            logger.LogError("{message}", e.Message);
        }
    }
}