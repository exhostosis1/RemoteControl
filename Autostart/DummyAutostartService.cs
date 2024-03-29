using Shared.AutoStart;
using Shared.Logging.Interfaces;

namespace AutoStart;

public class DummyAutoStartService(ILogger<DummyAutoStartService> logger) : IAutoStartService
{
    private bool _autoStart;

    public bool CheckAutoStart()
    {
        logger.LogInfo("Checking dummy auto start");
        return _autoStart;
    }

    public void SetAutoStart(bool value)
    {
        logger.LogInfo($"Setting dummy auto start to {value}");
        _autoStart = value;
    }
}