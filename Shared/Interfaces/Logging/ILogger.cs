using Shared.Enums;

namespace Shared.Interfaces.Logging;

public interface ILogger
{
    public void Log(string message, LoggingLevel level = LoggingLevel.Info);

    public void LogInfo(string message) => Log(message, LoggingLevel.Info);
    public void LogError(string message) => Log(message, LoggingLevel.Error);
    public void LogWarn(string message) => Log(message, LoggingLevel.Warn);
}