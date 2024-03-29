using Shared.AutoStart;
using Shared.Logging.Interfaces;
using Shared.Wrappers.RegistryWrapper;
using System.Diagnostics;
using Shared.Wrappers.Registry;

namespace AutoStart;

public class RegistryAutoStartService(IRegistry registryWrapper, ILogger<RegistryAutoStartService> logger)
    : IAutoStartService
{
    private readonly IRegistryKey _regKey = registryWrapper.CurrentUser.OpenSubKey("SOFTWARE")?.OpenSubKey("Microsoft")?.OpenSubKey("Windows")
        ?.OpenSubKey("CurrentVersion")?.OpenSubKey("Run", true) ?? throw new NullReferenceException("Cannot open autorun registry key");
    private const string RegName = "Remote Control";
    private readonly string _regValue = $"\"{Process.GetCurrentProcess().MainModule?.FileName ?? throw new NullReferenceException()}\"";

    public bool CheckAutoStart()
    {
        logger.LogInfo("Checking win registry autorun");
        return _regKey.GetValue(RegName, "") as string == _regValue;
    }

    public void SetAutoStart(bool value)
    {
        logger.LogInfo("Setting win registry autorun");

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