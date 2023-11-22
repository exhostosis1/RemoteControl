using Logging;
using Logging.Formatters;
using Shared.Enums;
using Shared.Logging;

namespace UnitTests.Logging;

public class FileLoggerTests : IDisposable
{
    private readonly FileLogger _fileLogger;
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "log");
    private readonly TestMessageFormatter _formatter = new();

    public FileLoggerTests()
    {
        _fileLogger = new FileLogger(_filePath, LoggingLevel.Error, _formatter);

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

        await _fileLogger.LogAsync(type, message, level);

        Assert.False(File.Exists(_filePath));
    }

    [Fact]
    public async Task LogWarningTest()
    {
        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Warn;

        await _fileLogger.LogAsync(type, message, level);

        Assert.False(File.Exists(_filePath));
    }

    [Fact]
    public async Task LogErrorTest()
    {
        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Error;
        var date = DateTime.Now;

        await _fileLogger.LogAsync(type, message, level);

        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        Assert.True(File.Exists(_filePath));

        var writtenMessage = (await File.ReadAllLinesAsync(_filePath)).First();

        Assert.Equal(formattedMessage, writtenMessage);
    }

    [Theory]
    [InlineData(1000)]
    public void ParallelLoggingTest(int count)
    {
        var tasks = new Task[count];
        for (var i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() => _fileLogger.Log(this.GetType(), "test message", LoggingLevel.Error));
        }

        Task.WaitAll(tasks);

        Assert.True(File.Exists(_filePath));
        Assert.True(File.ReadAllLines(_filePath).Length == count);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }
}