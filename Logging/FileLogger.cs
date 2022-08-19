using Logging.Abstract;
using Shared.Enums;
using Shared.Logging.Interfaces;

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

        protected override void ProcessMessage(string message)
        {
            File.AppendAllText(_path, message);
        }
    }
}
