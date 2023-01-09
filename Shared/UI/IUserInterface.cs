using System.Collections.Generic;
using Shared.ControlProcessor;

namespace Shared.UI;

public interface IUserInterface
{
    public event NullableIntEventHandler? StartEvent;
    public event NullableIntEventHandler? StopEvent;
    public event EmptyEventHandler? CloseEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event ConfigWithIdEventHandler? ConfigChangedEvent;
    public event StringEventHandler? ProcessorAddedEvent;
    public event IntEventHandler? ProcessorRemovedEvent;

    public void SetAutostartValue(bool value);

    // ReSharper disable once InconsistentNaming
    public void RunUI(List<AbstractControlProcessor> processors);
    public void ShowError(string message);
    public void AddProcessor(AbstractControlProcessor processor);
}