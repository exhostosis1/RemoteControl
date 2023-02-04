using Shared.Autostart;
using Shared.Logging.Interfaces;

namespace Autostart;

public class DummyAutostartService : IAutostartService
{
    private bool _autostart;
    private readonly ILogger<DummyAutostartService> _logger;

    public DummyAutostartService(ILogger<DummyAutostartService> logger)
    {
        _logger = logger;
    }

    public bool CheckAutostart()
    {
        _logger.LogInfo("Checking dummy autostart");
        return _autostart;
    }

    public void SetAutostart(bool value)
    {
        _logger.LogInfo($"Setting dummy autostart to {value}");
        _autostart = value;
    }
}