using Microsoft.Win32.TaskScheduler;
using Shared.TaskServiceWrapper;

namespace Autostart.Task;

public class TaskPrincipalWrapper : ITaskPrincipal
{
    private readonly TaskPrincipal _principal;

    public TaskPrincipalWrapper(TaskPrincipal principal)
    {
        _principal = principal;
    }

    public string UserId
    {
        get => _principal.UserId;
        set => _principal.UserId = value;
    }
}