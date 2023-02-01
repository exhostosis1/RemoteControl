using Shared.Enums;
using System;

namespace Shared.Logging.Interfaces;

public interface ILogger
{
    public void Log(Type type, string message, LoggingLevel level);

    public void LogInfo(Type type, string message) => Log(type, message, LoggingLevel.Info);
    public void LogError(Type type, string message) => Log(type, message, LoggingLevel.Error);
    public void LogWarn(Type type, string message) => Log(type, message, LoggingLevel.Warn);

    public void Flush();
}