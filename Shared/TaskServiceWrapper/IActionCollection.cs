namespace Shared.TaskServiceWrapper;

public interface IActionCollection
{
    public void Add(string path, string? arguments = null, string? workingDirectory = null);
}