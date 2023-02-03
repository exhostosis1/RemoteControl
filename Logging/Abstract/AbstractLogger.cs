using Logging.Formatters;
using Shared.Enums;
using Shared.Logging;
using Shared.Logging.Interfaces;

namespace Logging.Abstract;

public abstract class AbstractLogger : ILogger
{
    private readonly IMessageFormatter _formatter;
    private readonly LoggingLevel _currentLoggingLevel;

    protected AbstractLogger(LoggingLevel level, IMessageFormatter? formatter)
    {
        _formatter = formatter ?? new DefaultMessageFormatter();
        _currentLoggingLevel = level;
    }

    public Task LogAsync(Type type, string message, LoggingLevel level)
    {
        return Task.Run(() => Log(type, message, level));
    }

    public void Log(Type type, string message, LoggingLevel level = LoggingLevel.Error)
    {
        if (level >= _currentLoggingLevel)
        {
            WriteMessage(new LogMessage(type, level, DateTime.Now, message));
        }
    }

    private void WriteMessage(LogMessage message)
    {
        switch (message.Level)
        {
            case LoggingLevel.Error:
                ProcessError(_formatter.Format(message));
                break;
            case LoggingLevel.Warn:
                ProcessWarning(_formatter.Format(message));
                break;
            case LoggingLevel.Info:
                ProcessInfo(_formatter.Format(message));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected abstract void ProcessInfo(string message);
    protected abstract void ProcessWarning(string message);
    protected abstract void ProcessError(string message);
}