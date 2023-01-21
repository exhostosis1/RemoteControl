namespace Shared.Wrappers.TaskServiceWrapper;

public interface ITaskService
{
    public ITaskDefinition? FindTask(string name);
    public bool RegisterNewTask(ITaskDefinition task);
    public void DeleteTask(string name, bool throwIfTaskNotFound);

    public ITaskDefinition NewTask(string name, string userId);
}