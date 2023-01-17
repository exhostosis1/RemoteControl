using System.Collections.Generic;

namespace Shared.TaskServiceWrapper;

public record TaskAction(string Filename, string? Arguments = null, string? Directory = null);
public record TaskTrigger;
public record TaskLogonTrigger(string UserId): TaskTrigger;

public interface ITaskDefinition
{
    public string Name { get; set; }
    public string UserId { get; set; }
    public bool Enabled { get; }
    public List<TaskAction> Actions { get; }
    public List<TaskTrigger> Triggers { get; }
}