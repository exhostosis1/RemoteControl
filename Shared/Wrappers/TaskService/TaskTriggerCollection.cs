using System.Collections;
using System.Collections.Generic;

namespace Shared.Wrappers.TaskServiceWrapper;

public class TaskTriggerCollection : IEnumerable<TaskTrigger>
{
    private readonly List<TaskTrigger> _triggers = [];

    public virtual void Add(TaskTrigger trigger) => _triggers.Add(trigger);

    public IEnumerator<TaskTrigger> GetEnumerator() => _triggers.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _triggers.GetEnumerator();
}