using Shared.AutoStart;
using Shared.Logging.Interfaces;

namespace AutoStart;

public class DummyAutoStartService : IAutoStartService
{
    private bool _autoStart;
    private readonly ILogger<DummyAutoStartService> _logger;

    public DummyAutoStartService(ILogger<DummyAutoStartService> logger)
    {
        _logger = logger;
    }

    public bool CheckAutoStart()
    {
        _logger.LogInfo("Checking dummy auto start");
        return _autoStart;
    }

    public void SetAutoStart(bool value)
    {
        _logger.LogInfo($"Setting dummy auto start to {value}");
        _autoStart = value;
    }
}