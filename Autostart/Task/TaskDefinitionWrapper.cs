using Shared.TaskServiceWrapper;

namespace Autostart.Task;

public class LocalTaskDefinition : ITaskDefinition
{
    public string Name { get; set; }
    public string UserId { get; set; }
    public bool Enabled { get; init; }

    public List<TaskTrigger> Triggers => new();

    public List<TaskAction> Actions => new();

    public LocalTaskDefinition(string name, string userId)
    {
        Name = name;
        UserId = userId;
    }
}