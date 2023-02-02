using Shared.Enums;
using Shared.Logging.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Logging;

public class LogWrapper<T> : ILogger<T> where T : class
{
    private readonly ILogger _logger;

    public LogWrapper(ILogger logger)
    {
        _logger = logger;
    }

    public Task LogAsync(string message, LoggingLevel level) => _logger.LogAsync(typeof(T), message, level);
    public void Log(string message, LoggingLevel level) => _logger.Log(typeof(T), message, level);
    public Task LogAsync(Type type, string message, LoggingLevel level) => _logger.LogAsync(type, message, level);
    public void Log(Type type, string message, LoggingLevel level) => _logger.Log(type, message, level);

    public void Flush(int timeout) => Flush(TimeSpan.FromMilliseconds(timeout));
    public void Flush(TimeSpan timeout) => _logger.Flush(timeout);
    public void Flush(CancellationToken token) => _logger.Flush(token);

    public Task FlushAsync(CancellationToken token) => _logger.FlushAsync(token);
}