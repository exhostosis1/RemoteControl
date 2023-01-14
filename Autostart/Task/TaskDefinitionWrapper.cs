using Microsoft.Win32.TaskScheduler;
using Shared.TaskServiceWrapper;

namespace Autostart.Task;

public class TaskDefinitionWrapper : ITaskDefinition
{
    public IActionCollection Actions => new ActionCollectionWrapper(TaskDefinition.Actions);
    public ITriggerCollection Triggers => new TriggerCollectionWrapper(TaskDefinition.Triggers);
    public ITaskPrincipal Principal => new TaskPrincipalWrapper(TaskDefinition.Principal);

    public readonly TaskDefinition TaskDefinition;

    public TaskDefinitionWrapper(TaskDefinition definition)
    {
        TaskDefinition = definition;
    }
}