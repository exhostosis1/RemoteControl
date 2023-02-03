using Shared.Enums;
using System.Threading.Tasks;

namespace Shared.Logging.Interfaces;

public interface ILogger<out T> : ILogger where T : class
{
    public Task LogAsync(string message, LoggingLevel level) => LogAsync(typeof(T), message, level);
    public void Log(string message, LoggingLevel level) => Log(typeof(T), message, level);

    public Task LogInfoAsync(string message) => LogAsync(typeof(T), message, LoggingLevel.Info);
    public Task LogErrorAsync(string message) => LogAsync(typeof(T), message, LoggingLevel.Error);
    public Task LogWarnAsync(string message) => LogAsync(typeof(T), message, LoggingLevel.Warn);

    public void LogInfo(string message) => Log(typeof(T), message, LoggingLevel.Info);
    public void LogError(string message) => Log(typeof(T), message, LoggingLevel.Error);
    public void LogWarn(string message) => Log(typeof(T), message, LoggingLevel.Warn);
}