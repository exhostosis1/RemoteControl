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

    private int _count;

    private readonly ManualResetEventSlim _addEvent = new(true);
    private readonly AutoResetEvent _countEvent = new(false);

    protected AbstractLogger(LoggingLevel level, IMessageFormatter? formatter)
    {
        _formatter = formatter ?? new DefaultMessageFormatter();
        _currentLoggingLevel = level;

        new TaskFactory().StartNew(WriteMessages, TaskCreationOptions.LongRunning);
    }

    public Task LogAsync(Type type, string message, LoggingLevel level)
    {
        return Task.Run(() => Log(type, message, level));
    }

    public void Log(Type type, string message, LoggingLevel level = LoggingLevel.Error)
    {
        if (level >= _currentLoggingLevel)
        {
            _addEvent.Wait(TimeSpan.FromMinutes(1));

            _messages.Add(new LogMessage(type, level, DateTime.Now, message));
            _count++;
        }
    }

    public void Flush(int millisecondsTimeout = int.MaxValue) => Flush(TimeSpan.FromMilliseconds(millisecondsTimeout));
    public void Flush(TimeSpan timeout) => Flush(new CancellationTokenSource(timeout).Token);
    public void Flush(CancellationToken token)
    {
        _addEvent.Reset();

        try
        {
            while (_count > 0)
            {
                token.ThrowIfCancellationRequested();
                _countEvent.WaitOne();
            }
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Flush timed out or cancelled");
        }
        finally
        {
            _addEvent.Set();
        }
    }

    public Task FlushAsync(CancellationToken token)
    {
        return Task.Run(() => Flush(token), token);
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
            _countEvent.Set();
        }
    }

    protected abstract void ProcessInfo(string message);
    protected abstract void ProcessWarning(string message);
    protected abstract void ProcessError(string message);
}