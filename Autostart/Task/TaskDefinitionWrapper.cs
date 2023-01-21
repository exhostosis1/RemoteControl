using Shared.TaskServiceWrapper;
using Shared.Wrappers.TaskServiceWrapper;

namespace Autostart.Task;

public class LocalTaskDefinition : ITaskDefinition
{
    public string Name { get; set; }
    public string UserId { get; set; }
    public bool Enabled { get; init; }

    public TaskTriggerCollection Triggers => new();

    public TaskActionCollection Actions => new();

    public LocalTaskDefinition(string name, string userId)
    {
        Name = name;
        UserId = userId;
    }
}