namespace Shared.TaskServiceWrapper;

public interface ITaskService
{
    public ITaskDefinition NewTask();
    public ITask? FindTask(string name);
    public ITaskFolder RootFolder { get; }
}