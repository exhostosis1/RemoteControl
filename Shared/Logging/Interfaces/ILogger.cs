using System;
using Shared.Enums;

namespace Shared.Logging.Interfaces;

public interface ILogger
{
    public void Log(Type type, string message, LoggingLevel level);

    public void LogInfo(Type type, string message) => Log(type, message, LoggingLevel.Info);
    public void LogError(Type type,string message) => Log(type, message, LoggingLevel.Error);
    public void LogWarn(Type type,string message) => Log(type,message, LoggingLevel.Warn);
}

public interface ILogger<T> : ILogger where T : class
{
    public void Log(string message, LoggingLevel level) => Log(typeof(T), message, level);
    public void LogInfo(string message) => LogInfo(typeof(T), message);
    public void LogError(string message) => LogError(typeof(T), message);
    public void LogWarn(string message) => LogWarn(typeof(T), message);
}