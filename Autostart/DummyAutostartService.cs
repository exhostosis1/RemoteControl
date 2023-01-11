using Shared.Logging.Interfaces;

namespace Autostart;

public class DummyAutostartService: BaseAutostartService
{
    private bool _autostart;
    private readonly ILogger<DummyAutostartService> _logger;

    public DummyAutostartService(ILogger<DummyAutostartService> logger)
    {
        _logger = logger;
    }

    public override bool CheckAutostart()
    {
        _logger.LogInfo("Checking dummy autostart");
        return _autostart;
    }

    public override void SetAutostart(bool value)
    {
        _logger.LogInfo($"Setting dummy autostart to {value}");
        _autostart = value;
    }
}