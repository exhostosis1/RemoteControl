using Microsoft.Win32.TaskScheduler;
using Shared.TaskServiceWrapper;

namespace Autostart.Task;

public class ActionCollectionWrapper : IActionCollection
{
    private readonly ActionCollection _collection;

    public ActionCollectionWrapper(ActionCollection collection)
    {
        _collection = collection;
    }

    public void Add(string path, string? arguments = null, string? workingDirectory = null) => _collection.Add(path, arguments, workingDirectory);
}