using Logging;
using Logging.Formatters;
using Shared.Enums;
using Shared.Logging;

namespace UnitTests.Logging;

public class FileLoggerTests : IDisposable
{
    private readonly FileLogger _fileLogger;
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "log");

    public FileLoggerTests()
    {
        _fileLogger = new FileLogger(_filePath);

        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }

    [Fact]
    public async Task LogInfoTest()
    {
        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Info;

        _fileLogger.Log(type, message, level);

        await Task.Delay(100);

        Assert.False(File.Exists(_filePath));
    }

    [Fact]
    public async Task LogWarningTest()
    {
        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Warn;

        _fileLogger.Log(type, message, level);

        await Task.Delay(100);

        Assert.False(File.Exists(_filePath));
    }

    [Fact]
    public async Task LogErrorTest()
    {
        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Error;
        var date = DateTime.Now;

        _fileLogger.Log(type, message, level);

        await Task.Delay(100);

        var formattedMessage = new DefaultMessageFormatter().Format(new LogMessage(type, level, date, message));

        Assert.True(File.Exists(_filePath));

        var writtenMessage = (await File.ReadAllLinesAsync(_filePath)).First();

        Assert.Equal(formattedMessage, writtenMessage);
    }

    public void Dispose()
    {
    }
}