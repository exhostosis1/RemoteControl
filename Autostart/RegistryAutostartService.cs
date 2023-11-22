using Shared.AutoStart;
using Shared.Logging.Interfaces;
using Shared.Wrappers.RegistryWrapper;
using System.Diagnostics;

namespace AutoStart;

public class RegistryAutoStartService : IAutoStartService
{
    private readonly IRegistryKey _regKey;
    private const string RegName = "Remote Control";
    private readonly string _regValue = $"\"{Process.GetCurrentProcess().MainModule?.FileName ?? throw new NullReferenceException()}\"";
    private readonly ILogger<RegistryAutoStartService> _logger;

    public RegistryAutoStartService(IRegistry registryWrapper, ILogger<RegistryAutoStartService> logger)
    {
        _logger = logger;

        _regKey = registryWrapper.CurrentUser.OpenSubKey("SOFTWARE")?.OpenSubKey("Microsoft")?.OpenSubKey("Windows")
            ?.OpenSubKey("CurrentVersion")?.OpenSubKey("Run", true) ?? throw new NullReferenceException("Cannot open autorun registry key");
    }

    public bool CheckAutoStart()
    {
        _logger.LogInfo("Checking win registry autorun");
        return _regKey.GetValue(RegName, "") as string == _regValue;
    }

    public void SetAutoStart(bool value)
    {
        _logger.LogInfo("Setting win registry autorun");

        _regKey.DeleteValue(RegName, false);

        if (value)
        {
            try
            {
                _regKey.SetValue(RegName, _regValue, RegValueType.String);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}