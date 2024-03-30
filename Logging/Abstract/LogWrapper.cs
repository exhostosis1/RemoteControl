using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Logging.Abstract;

internal class LogWrapper<T>(ILogger logger) : ILogger<T> where T : class
{
    public Task LogAsync(Type type, string message, LoggingLevel level) => logger.LogAsync(type, message, level);

    public void Log(Type type, string message, LoggingLevel level) => logger.Log(type, message, level);
    public ILogger<T1> WrapLogger<T1>() where T1 : class
    {
        throw new NotImplementedException();
    }
}