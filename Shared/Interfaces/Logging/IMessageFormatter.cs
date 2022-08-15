namespace Shared.Interfaces.Logging;

public interface IMessageFormatter
{
    public string Format(LogMessage message);
}