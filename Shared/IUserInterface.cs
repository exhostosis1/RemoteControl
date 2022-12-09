using Shared.Config;
using System.Collections.Generic;

namespace Shared;

public interface IUserInterface
{
    public event StringEventHandler? StartEvent;
    public event StringEventHandler? StopEvent;
    public event ConfigEventHandler? ConfigChangedEvent;
    public event EmptyEventHandler? CloseEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event EmptyEventHandler? AddFirewallRuleEvent;

    public IList<IControlProcessor> ControlProcessors { get; set; }

    public bool IsAutostart { get; set; }

    // ReSharper disable once InconsistentNaming
    public void RunUI(AppConfig config);
    public void ShowError(string message);
}