using Shared.Enums;

namespace Shared.Logging.Interfaces;

public interface ILogger<out T> : ILogger where T : class
{
    public void Log(string message, LoggingLevel level) => Log(typeof(T), message, level);
    public void LogInfo(string message) => LogInfo(typeof(T), message);
    public void LogError(string message) => LogError(typeof(T), message);
    public void LogWarn(string message) => LogWarn(typeof(T), message);
}