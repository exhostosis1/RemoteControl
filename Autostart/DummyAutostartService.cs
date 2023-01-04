using Shared.Logging.Interfaces;

namespace Autostart;

public class DummyAutostartService: BaseAutostartService
{
    private bool _autostart;

    public DummyAutostartService(ILogger logger): base(logger){}

    public override bool CheckAutostart()
    {
        Logger.LogInfo("Checking dummy autostart");
        return _autostart;
    }

    public override void SetAutostart(bool value)
    {
        Logger.LogInfo($"Setting dummy autostart to {value}");
        _autostart = value;
    }
}