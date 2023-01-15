using Shared.TaskServiceWrapper;

namespace Autostart.Task;

public class TaskWrapper : ITask
{
    private readonly Microsoft.Win32.TaskScheduler.Task _task;

    public TaskWrapper(Microsoft.Win32.TaskScheduler.Task task)
    {
        _task = task;
    }

    public bool Enabled
    {
        get => _task.Enabled;
        set => _task.Enabled = value;
    }
}