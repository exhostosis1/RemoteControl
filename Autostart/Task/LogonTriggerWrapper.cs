using Microsoft.Win32.TaskScheduler;
using Shared.TaskServiceWrapper;

namespace Autostart.Task;

public class LogonTriggerWrapper : ITrigger
{
    public readonly LogonTrigger Trigger;

    public string UserId
    {
        get => Trigger.UserId;
        set => Trigger.UserId = value;
    }

    public LogonTriggerWrapper()
    {
        Trigger = new LogonTrigger();
    }
}