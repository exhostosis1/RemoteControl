using Shared.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Logging.Interfaces;

public interface ILogger
{
    public Task LogAsync(Type type, string message, LoggingLevel level);
    public void Log(Type type, string message, LoggingLevel level);

    public Task LogInfoAsync(Type type, string message) => LogAsync(type, message, LoggingLevel.Info);
    public Task LogErrorAsync(Type type, string message) => LogAsync(type, message, LoggingLevel.Error);
    public Task LogWarnAsync(Type type, string message) => LogAsync(type, message, LoggingLevel.Warn);

    public void LogInfo(Type type, string message) => Log(type, message, LoggingLevel.Info);
    public void LogError(Type type, string message) => Log(type, message, LoggingLevel.Error);
    public void LogWarn(Type type, string message) => Log(type, message, LoggingLevel.Warn);

    public void Flush(int timeout = int.MaxValue);
    public void Flush(TimeSpan timeout);
    public void Flush(CancellationToken token);

    public Task FlushAsync(CancellationToken token = default);
}