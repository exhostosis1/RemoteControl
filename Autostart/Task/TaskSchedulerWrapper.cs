using Microsoft.Win32.TaskScheduler;
using Shared.Wrappers.TaskServiceWrapper;
using System.Runtime.InteropServices;

namespace Autostart.Task;

public class TaskSchedulerWrapper : ITaskService
{
    private static readonly TaskService TaskService = new();

    public TaskSchedulerWrapper()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");
    }

    public bool RegisterNewTask(ITaskDefinition task)
    {
        var td = TaskService.NewTask();
        td.Principal.UserId = task.UserId;

        foreach (var taskAction in task.Actions)
        {
            td.Actions.Add(taskAction.Filename, taskAction.Arguments, taskAction.Directory);
        }

        foreach (var taskTrigger in task.Triggers)
        {
            switch (taskTrigger)
            {
                case TaskLogonTrigger lt:
                    td.Triggers.Add(new LogonTrigger { UserId = lt.UserId });
                    break;
                default:
                    break;
            }
        }

        return TaskService.RootFolder.RegisterTaskDefinition(task.Name, td) != null;
    }

    public void DeleteTask(string name, bool throwIfTaskNotFound) =>
        TaskService.RootFolder.DeleteTask(name, throwIfTaskNotFound);

    public ITaskDefinition? FindTask(string name)
    {
        var task = TaskService.FindTask(name);

        if (task == null) return null;

        var result = new LocalTaskDefinition(task.Name, task.Definition.Principal.UserId)
        {
            Enabled = task.Enabled
        };

        return result;
    }

    public ITaskDefinition NewTask(string name, string userId) => new LocalTaskDefinition(name, userId);
}