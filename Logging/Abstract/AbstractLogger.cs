using Logging.Formatters;
using Shared.Enums;
using Shared.Logging;
using Shared.Logging.Interfaces;
using System.Collections.Concurrent;

namespace Logging.Abstract;

public abstract class AbstractLogger : ILogger
{
    private readonly BlockingCollection<LogMessage> _messages = new();
    private readonly IMessageFormatter _formatter;
    private readonly LoggingLevel _currentLoggingLevel;

    private bool _locked;
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
            while(_locked)
                Thread.Sleep(50);

            _messages.Add(new LogMessage(type, level, DateTime.Now, message));
            _count++;
        }
    }

    public void Flush(int millisecondsTimeout = int.MaxValue) => Flush(TimeSpan.FromMilliseconds(millisecondsTimeout));

    public void Flush(TimeSpan timeout)
    {
        _locked = true;
        var start = DateTime.Now;

        while (_count > 0)
        {
            Thread.Sleep(50);

            if (DateTime.Now - start > timeout)
            {
                _locked = false;
                throw new TimeoutException("Flush operation timed out");
            }
        }

        _locked = false;
    }

    private void WriteMessages()
    {
        foreach (var message in _messages.GetConsumingEnumerable())
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
            
            _count = _messages.Count;
        }
    }

    protected abstract void ProcessInfo(string message);
    protected abstract void ProcessWarning(string message);
    protected abstract void ProcessError(string message);
}