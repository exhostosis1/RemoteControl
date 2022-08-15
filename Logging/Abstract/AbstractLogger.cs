using Logging.Formatters;
using Shared;
using Shared.Enums;
using Shared.Interfaces.Logging;
using System.Collections.Concurrent;

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
                ProcessMessage(_formatter.Format(message));
            }
        }

        protected abstract void ProcessMessage(string message);
    }
}
