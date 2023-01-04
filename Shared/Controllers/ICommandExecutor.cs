namespace Shared.Controllers;

public interface ICommandExecutor
{
    public string Execute(string command);
}