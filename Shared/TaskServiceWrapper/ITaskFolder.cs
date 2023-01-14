namespace Shared.TaskServiceWrapper;

public interface ITaskFolder
{
    public void DeleteTask(string name, bool exceptionOnNotExists);
    public void RegisterTaskDefinition(string name, ITaskDefinition definition);
}