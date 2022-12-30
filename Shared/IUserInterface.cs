using System.Collections.Generic;

namespace Shared;

public interface IUserInterface
{
    public event ProcessorEventHandler? StartEvent;
    public event ProcessorEventHandler? StopEvent;
    public event EmptyEventHandler? CloseEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event EmptyEventHandler? AddFirewallRuleEvent;
    public event ConfigEventHandler? ConfigChangedEvent;

    public void SetViewModel(IEnumerable<ControlProcessorDto> model);

    public void SetAutostartValue(bool value);

    // ReSharper disable once InconsistentNaming
    public void RunUI();
    public void ShowError(string message);
}