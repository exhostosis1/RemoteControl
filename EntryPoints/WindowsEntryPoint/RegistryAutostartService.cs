using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Shared.AutoStart;
using Shared.Wrappers.Registry;
using Shared.Wrappers.RegistryWrapper;

namespace WindowsEntryPoint;

public class RegistryAutoStartService(IRegistry registryWrapper, ILogger logger)
    : IAutoStartService
{
    private readonly IRegistryKey _regKey = registryWrapper.CurrentUser.OpenSubKey("SOFTWARE")?.OpenSubKey("Microsoft")?.OpenSubKey("Windows")
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
            _regKey.SetValue(RegName, _regValue, RegValueType.String);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
    }
}