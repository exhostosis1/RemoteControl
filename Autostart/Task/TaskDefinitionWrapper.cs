using Shared.Wrappers.TaskService;

namespace AutoStart.Task;

public class LocalTaskDefinition(string name, string userId) : ITaskDefinition
{
    public string Name { get; set; } = name;
    public string UserId { get; set; } = userId;
    public bool Enabled { get; init; }

    public TaskTriggerCollection Triggers => [];

    public TaskActionCollection Actions => [];
}