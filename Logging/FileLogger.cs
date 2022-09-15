using Logging.Abstract;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Logging
{
    public class FileLogger : AbstractLogger
    {
        private readonly string _path;

        private static readonly object FileLock = new();

        public FileLogger(string filePath, LoggingLevel level = LoggingLevel.Error, IMessageFormatter? formatter = null) : base(level, formatter)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException(nameof(filePath));

            if (!File.Exists(filePath))
                File.Create(filePath);

            _path = filePath;
        }

        protected override void ProcessInfo(string message)
        {
            lock (FileLock)
            {
                File.AppendAllText(_path, message + "\n");
            }
        }

        protected override void ProcessWarning(string message) => ProcessInfo(message);
        protected override void ProcessError(string message) => ProcessInfo(message);
    }
}
