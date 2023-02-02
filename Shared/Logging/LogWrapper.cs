using Shared.Enums;
using Shared.Logging.Interfaces;
using System;

namespace Shared.Logging;

public class LogWrapper<T> : ILogger<T> where T : class
{
    private readonly ILogger _logger;

    public LogWrapper(ILogger logger)
    {
        _logger = logger;
    }

    public void Log(Type type, string message, LoggingLevel level) => _logger.Log(type, message, level);
    public void Flush(int timeout) => Flush(TimeSpan.FromMilliseconds(timeout));
    public void Flush(TimeSpan timeout) => _logger.Flush(timeout);
}