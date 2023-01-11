using Shared;

namespace Autostart;

public abstract class BaseAutostartService: IAutostartService
{
    public abstract bool CheckAutostart();

    public abstract void SetAutostart(bool value);
}