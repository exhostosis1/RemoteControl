using Logging.Formatters;
using Shared.Enums;
using Shared.Logging;
using Shared.Logging.Interfaces;

namespace Logging.Abstract;

public abstract class AbstractLogger : ILogger
{
    private readonly List<LogMessage> _messages = new();
    private readonly IMessageFormatter _formatter;
    private readonly LoggingLevel _currentLoggingLevel;
    private readonly SemaphoreSlim _semaphore = new(0);

    protected AbstractLogger(LoggingLevel level, IMessageFormatter? formatter)
    {
        _formatter = formatter ?? new DefaultMessageFormatter();
        _currentLoggingLevel = level;

        new TaskFactory().StartNew(WriteMessages, TaskCreationOptions.LongRunning);
    }

    public void Log(Type type, string message, LoggingLevel level = LoggingLevel.Error)
    {
        if (level >= _currentLoggingLevel)
        {
            _messages.Add(new LogMessage(type, level, DateTime.Now, message));

            _semaphore.Release();
        }
    }

    public void Flush()
    {
        while (_messages.Count > 0)
        {
            Thread.Sleep(100);
        }
    }

    private void WriteMessages()
    {
        while (true)
        {
            _semaphore.Wait();

            var message = _messages.First();

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

            _messages.Remove(message);
        }
    }

    protected abstract void ProcessInfo(string message);
    protected abstract void ProcessWarning(string message);
    protected abstract void ProcessError(string message);
}