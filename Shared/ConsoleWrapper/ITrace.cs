namespace Shared.ConsoleWrapper;

public interface ITrace
{
    public void TraceInformation(string message);
    public void TraceWarning(string message);
    public void TraceError(string message);
}