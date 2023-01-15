using Microsoft.Win32.TaskScheduler;
using Shared.TaskServiceWrapper;

namespace Autostart.Task;

public class TaskServiceWrapper : ITaskService
{
    private static readonly TaskService TaskService = new();

    public ITaskFolder RootFolder => new TaskFolderWrapper(TaskService.RootFolder);
    

    public ITaskDefinition NewTask() => new TaskDefinitionWrapper(TaskService.NewTask());

    public ITask? FindTask(string name)
    {
        var result = TaskService.FindTask(name);
        return result == null ? null : new TaskWrapper(result);
    }
}