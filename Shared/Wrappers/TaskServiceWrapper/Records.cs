namespace Shared.TaskServiceWrapper;

public record TaskAction(string Filename, string? Arguments = null, string? Directory = null);
public record TaskTrigger;
public record TaskLogonTrigger(string UserId) : TaskTrigger;