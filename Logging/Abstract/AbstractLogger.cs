using System.Collections.Concurrent;
using Logging.Formatters;
using Shared;
using Shared.Enums;
using Shared.Logging;
using Shared.Logging.Interfaces;

namespace Logging.Abstract;

public abstract class AbstractLogger : ILogger
{
    private readonly BlockingCollection<LogMessage> _messages = new();
    private readonly IMessageFormatter _formatter;
    private readonly LoggingLevel _currentLoggingLevel;

    private readonly Utils.Waiter _addWaiter = new();
    private readonly Utils.Waiter _countWaiter = new();

    private int _count;

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
            _addWaiter.WaitIfLocked();

            _messages.Add(new LogMessage(type, level, DateTime.Now, message));
            _count++;
        }
    }

    public void Flush()
    {
        _addWaiter.Lock();

        while (_count > 0)
        {
            _countWaiter.WaitIfLocked();
        }

        _addWaiter.Unlock();
    }

    private void WriteMessages()
    {
        foreach (var message in _messages.GetConsumingEnumerable())
        {
            _countWaiter.Lock();

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
            
            _count = _messages.Count;
            _countWaiter.Unlock();
        }
    }

    protected abstract void ProcessInfo(string message);
    protected abstract void ProcessWarning(string message);
    protected abstract void ProcessError(string message);
}