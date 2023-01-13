using Logging;
using Logging.Formatters;

using Shared.Enums;
using Shared.Logging;

namespace Tests;

public class FileLoggerTests : IDisposable
{
    private readonly FileLogger _fileLogger;
    private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");

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
        var message = "test message";
        var type = GetType();
        var level = LoggingLevel.Info;

        _fileLogger.Log(type, message, level);

        var loggedText = "";


        try
        {
            loggedText = await File.ReadAllTextAsync(_filePath);
        }
        catch (IOException)
        {
        }

        Assert.True(string.IsNullOrEmpty(loggedText));
    }

    [Fact]
    public async Task LogWarningTest()
    {
        var message = "test message";
        var type = GetType();
        var level = LoggingLevel.Warn;

        _fileLogger.Log(type, message, level);

        var loggedText = "";

        try
        {
            loggedText = await File.ReadAllTextAsync(_filePath);
        }
        catch (IOException)
        {
        }

        Assert.True(string.IsNullOrEmpty(loggedText));
    }

    [Fact]
    public async Task LogErrorTest()
    {
        var message = "test message";
        var type = GetType();
        var level = LoggingLevel.Error;
        var date = DateTime.Now;

        var formatter = new DefaultMessageFormatter();
        var formattedMessage = formatter.Format(new LogMessage(type, level, date, message)) + Environment.NewLine;

        _fileLogger.Log(type, message, level);

        var loggedText = "";

        for (var i = 0; i < 10; i++)
        {
            try
            {
                loggedText = await File.ReadAllTextAsync(_filePath);
                break;
            }
            catch (IOException)
            {
                await Task.Delay(1000);
            }
        }

        Assert.True(formattedMessage == loggedText);
    }

    public void Dispose()
    {
    }
}