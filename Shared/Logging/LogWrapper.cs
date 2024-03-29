using Shared.Enums;
using Shared.Logging.Interfaces;
using System;
using System.Threading.Tasks;

namespace Shared.Logging;

public class LogWrapper<T>(ILogger logger) : ILogger<T> where T : class
{
    public Task LogAsync(Type type, string message, LoggingLevel level) => logger.LogAsync(type, message, level);

    public void Log(Type type, string message, LoggingLevel level) => logger.Log(type, message, level);
}