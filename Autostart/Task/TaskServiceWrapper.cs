using Microsoft.Win32.TaskScheduler;
using Shared.TaskServiceWrapper;

namespace Autostart.Task;

public class TaskServiceWrapper: ITaskService
{
    private readonly TaskService _taskService;
 
    public ITaskFolder RootFolder => new TaskFolderWrapper(_taskService.RootFolder);

    public TaskServiceWrapper(TaskService service)
    {
        _taskService = service;
    }

    public ITaskDefinition NewTask() => new TaskDefinitionWrapper(_taskService.NewTask());

    public ITask? FindTask(string name)
    {
        var result = _taskService.FindTask(name);
        return result == null ? null : new TaskWrapper(result);
    }
}