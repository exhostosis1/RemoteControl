namespace Shared.Logging.Interfaces;

public interface IMessageFormatter
{
    public string Format(LogMessage message);
}