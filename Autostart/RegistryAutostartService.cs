using Shared;
using Shared.Logging.Interfaces;
using Shared.RegistryWrapper;
using System.Diagnostics;

namespace Autostart;

public class RegistryAutostartService : IAutostartService
{
    private readonly IRegistryKey _regKey;
    private const string RegName = "Remote Control";
    private readonly string _regValue = $"\"{Process.GetCurrentProcess().MainModule?.FileName ?? throw new NullReferenceException()}\"";
    private readonly ILogger<RegistryAutostartService> _logger;

    public RegistryAutostartService(IRegistry registryWrapper, ILogger<RegistryAutostartService> logger)
    {
        _logger = logger;

        _regKey = registryWrapper.CurrentUser.OpenSubKey("SOFTWARE")?.OpenSubKey("Microsoft")?.OpenSubKey("Windows")
            ?.OpenSubKey("CurrentVersion")?.OpenSubKey("Run", true) ?? throw new NullReferenceException("Cannot open autorun registry key");
    }

    public bool CheckAutostart()
    {
        _logger.LogInfo("Checking win registry autorun");
        return _regKey.GetValue(RegName, "") as string == _regValue;
    }

    public void SetAutostart(bool value)
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