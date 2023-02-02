using Logging;
using Logging.Formatters;
using Shared.Enums;
using Shared.Logging;
using Shared.Logging.Interfaces;

namespace UnitTests.Logging;

public class FileLoggerTests : IDisposable
{
    private readonly FileLogger _fileLogger;
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "log");
    private readonly IMessageFormatter _formatter = new TestMessageFormatter();

    public FileLoggerTests()
    {
        _fileLogger = new FileLogger(_filePath, LoggingLevel.Error, _formatter);

        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }

    [Fact]
    public void LogInfoTest()
    {
        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Info;

        _fileLogger.Log(type, message, level);

        _fileLogger.Flush();

        Assert.False(File.Exists(_filePath));
    }

    [Fact]
    public void LogWarningTest()
    {
        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Warn;

        _fileLogger.Log(type, message, level);

        _fileLogger.Flush();

        Assert.False(File.Exists(_filePath));
    }

    [Fact]
    public void LogErrorTest()
    {
        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Error;
        var date = DateTime.Now;

        _fileLogger.Log(type, message, level);

        _fileLogger.Flush();

        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        Assert.True(File.Exists(_filePath));

        var writtenMessage = (File.ReadAllLines(_filePath)).First();

        Assert.Equal(formattedMessage, writtenMessage);
    }

    public void Dispose()
    {
    }
}