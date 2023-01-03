using System.Collections.Generic;
using Shared.ControlProcessor;

namespace Shared.UI;

public interface IUserInterface
{
    public event IntEventHandler? StartEvent;
    public event IntEventHandler? StopEvent;
    public event EmptyEventHandler? CloseEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event EmptyEventHandler? AddFirewallRuleEvent;
    public event ConfigEventHandler? ConfigChangedEvent;
    public event ConfigEventHandler? ProcessorAddedEvent;

    public void SetViewModel(List<IControlProcessor> model);

    public void SetAutostartValue(bool value);

    // ReSharper disable once InconsistentNaming
    public void RunUI();
    public void ShowError(string message);
}