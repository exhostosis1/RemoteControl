namespace Shared.Wrappers.TaskService;

public interface ITaskDefinition
{
    public string Name { get; set; }
    public string UserId { get; set; }
    public bool Enabled { get; }
    public TaskActionCollection Actions { get; }
    public TaskTriggerCollection Triggers { get; }
}