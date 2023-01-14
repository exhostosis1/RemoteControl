using Microsoft.Win32.TaskScheduler;
using Shared.TaskServiceWrapper;

namespace Autostart.Task;

public class TriggerCollectionWrapper: ITriggerCollection
{
    private readonly TriggerCollection _collection;

    public TriggerCollectionWrapper(TriggerCollection collection)
    {
        _collection = collection;
    }

    public void Add(ITrigger trigger) => _collection.Add(((LogonTriggerWrapper)trigger).Trigger);
}