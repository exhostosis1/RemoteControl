using System.Collections;
using System.Collections.Generic;

namespace Shared.TaskServiceWrapper;

public class TaskTriggerCollection: IEnumerable<TaskTrigger>
{
    private readonly List<TaskTrigger> _triggers = new();

    public virtual void Add(TaskTrigger trigger) => _triggers.Add(trigger);

    public IEnumerator<TaskTrigger> GetEnumerator() => _triggers.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _triggers.GetEnumerator();
}