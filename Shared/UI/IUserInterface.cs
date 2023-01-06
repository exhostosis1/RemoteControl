using System.Collections.Generic;
using Shared.ControlProcessor;

namespace Shared.UI;

public interface IUserInterface
{
    public event IntEventHandler? StartEvent;
    public event IntEventHandler? StopEvent;
    public event EmptyEventHandler? CloseEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event ConfigWithIndexEventHandler? ConfigChangedEvent;
    public event StringEventHandler? ProcessorAddedEvent;

    public void SetAutostartValue(bool value);

    // ReSharper disable once InconsistentNaming
    public void RunUI(IEnumerable<IControlProcessor> processors);
    public void ShowError(string message);
}