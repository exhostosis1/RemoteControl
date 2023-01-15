using Microsoft.Win32.TaskScheduler;
using Shared.TaskServiceWrapper;

namespace Autostart.Task;

public class TaskFolderWrapper : ITaskFolder
{
    private readonly TaskFolder _folder;

    public TaskFolderWrapper(TaskFolder folder)
    {
        _folder = folder;
    }

    public void DeleteTask(string name, bool exceptionOnNotExists) => _folder.DeleteTask(name, exceptionOnNotExists);

    public void RegisterTaskDefinition(string name, ITaskDefinition definition) =>
        _folder.RegisterTaskDefinition(name, ((TaskDefinitionWrapper)definition).TaskDefinition);
}