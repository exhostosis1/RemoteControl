using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Diagnostics;

namespace MainApp;

internal class RegistryAutoStartService(ILogger logger)
{
    private readonly RegistryKey _regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE")?.OpenSubKey("Microsoft")?.OpenSubKey("Windows")
        ?.OpenSubKey("CurrentVersion")?.OpenSubKey("Run", true) ?? throw new NullReferenceException("Cannot open autorun registry key");
    private const string RegName = "Remote Control";
    private readonly string _regValue = $"\"{Process.GetCurrentProcess().MainModule?.FileName ?? throw new NullReferenceException()}\"";

    public bool CheckAutoStart()
    {
        if (!logger.IsEnabled(LogLevel.Information))
            return _regKey.GetValue(RegName, "") as string == _regValue;

        logger.LogInformation("Checking win registry autorun");
        return _regKey.GetValue(RegName, "") as string == _regValue;
    }

    public bool SetAutoStart(bool value)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Setting win registry autorun");

        _regKey.DeleteValue(RegName, false);

        if (!value) return false;
        try
        {
            _regKey.SetValue(RegName, _regValue, RegistryValueKind.String);
            return true;
        }
        catch (Exception e)
        {
            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError("{message}", e.Message);
            return false;
        }
    }
}