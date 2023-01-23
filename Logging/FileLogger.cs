using Logging.Abstract;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Logging;

public class FileLogger : AbstractLogger
{
    private readonly string _path;

    private static readonly object FileLock = new();

    public FileLogger(string filePath, LoggingLevel level = LoggingLevel.Error, IMessageFormatter? formatter = null) : base(level, formatter)
    {
        _path = filePath;

        if (string.IsNullOrWhiteSpace(_path))
            throw new ArgumentException(nameof(_path));
    }

    protected override void ProcessInfo(string message)
    {
        lock (FileLock)
        {
            File.AppendAllText(_path, message + Environment.NewLine);
        }
    }

    protected override void ProcessWarning(string message) => ProcessInfo(message);
    protected override void ProcessError(string message) => ProcessInfo(message);
}