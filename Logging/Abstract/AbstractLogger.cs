using Logging.Formatters;
using Shared.Enums;
using Shared.Logging.Interfaces;
using System.Collections.Concurrent;
using Shared.Logging;

namespace Logging.Abstract
{
    public abstract class AbstractLogger : ILogger
    {
        private readonly BlockingCollection<LogMessage> _messages = new();
        private readonly IMessageFormatter _formatter;
        private readonly LoggingLevel _currentLoggingLevel;

        protected AbstractLogger(LoggingLevel level, IMessageFormatter? formatter)
        {
            _formatter = formatter ?? new DefaultMessageFormatter();
            _currentLoggingLevel = level;
            new TaskFactory().StartNew(WriteMessages, TaskCreationOptions.LongRunning);
        }

        public void Log(string message, LoggingLevel level = LoggingLevel.Error)
        {
            if(level >= _currentLoggingLevel)
                _messages.Add(new LogMessage(level, DateTime.UtcNow, message));
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
            }
        }

        protected abstract void ProcessInfo(string message);
        protected abstract void ProcessWarning(string message);
        protected abstract void ProcessError(string message);
    }
}
