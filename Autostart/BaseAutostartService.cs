using Shared;
using Shared.Logging.Interfaces;

namespace Autostart;

public abstract class BaseAutostartService: IAutostartService
{
    internal ILogger Logger;

    internal BaseAutostartService(ILogger logger)
    {
        Logger = logger;
    }

    public abstract bool CheckAutostart();

    public abstract void SetAutostart(bool value);
}