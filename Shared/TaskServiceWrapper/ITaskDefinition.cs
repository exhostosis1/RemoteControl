namespace Shared.TaskServiceWrapper;

public interface ITaskDefinition
{
    public IActionCollection Actions { get; }
    public ITriggerCollection Triggers { get; }
    public ITaskPrincipal Principal { get; }
}