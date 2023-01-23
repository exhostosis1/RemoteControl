using System.Collections;
using System.Collections.Generic;

namespace Shared.Wrappers.TaskServiceWrapper;

public class TaskActionCollection : IEnumerable<TaskAction>
{
    private readonly List<TaskAction> _actions = new();

    public virtual void Add(TaskAction action) => _actions.Add(action);

    public IEnumerator<TaskAction> GetEnumerator() => _actions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _actions.GetEnumerator();
}