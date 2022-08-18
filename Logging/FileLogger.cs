using Logging.Abstract;
using Shared.Enums;
using Shared.Interfaces.Logging;

namespace Logging
{
    public class FileLogger : AbstractLogger
    {
        private readonly string _path;

        public FileLogger(string filePath, LoggingLevel level = LoggingLevel.Error, IMessageFormatter? formatter = null) : base(level, formatter)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !Path.IsPathFullyQualified(filePath))
                throw new ArgumentException(nameof(filePath));

            if (!File.Exists(filePath))
                File.Create(filePath);

            _path = filePath;
        }

        public FileLogger(string filePath, LoggingLevel level = LoggingLevel.Error)
            : this(filePath, level, null)
        {

        }

        public FileLogger(string filePath, IMessageFormatter? formatter = null)
            : this(filePath, LoggingLevel.Error, formatter)
        {

        }

        public FileLogger(string filePath)
            : this(filePath, LoggingLevel.Error, null)
        {

        }

        protected override void ProcessMessage(string message)
        {
            File.AppendAllText(_path, message);
        }
    }
}
