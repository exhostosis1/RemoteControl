using Shared.Enums;

namespace Shared.Interfaces.Logging;

public interface ILogger
{
    public void Log(string message, LoggingLevel level = LoggingLevel.Error);
}