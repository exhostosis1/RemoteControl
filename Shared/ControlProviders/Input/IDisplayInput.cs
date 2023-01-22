using Shared.Enums;

namespace Shared.ControlProviders.Input;

public interface IDisplayInput
{
    public void SetState(MonitorState state);
}