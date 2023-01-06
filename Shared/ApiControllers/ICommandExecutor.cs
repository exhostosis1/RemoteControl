namespace Shared.ApiControllers;

public interface ICommandExecutor
{
    public string Execute(string command);
}