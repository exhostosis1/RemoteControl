using Logging;
using Logging.Formatters;
using Moq;
using Shared.ConsoleWrapper;
using Shared.Enums;
using Shared.Logging;

namespace Tests;

public class ConsoleLoggerTests : IDisposable
{
    private readonly ConsoleLogger _logger;
    private readonly IConsole _console;

    public ConsoleLoggerTests()
    {
        _console = Mock.Of<IConsole>();
        _logger = new ConsoleLogger(_console);
    }

    [Fact]
    public async void LogInfoTest()
    {
        var message = "test message";
        var type = GetType();
        var level = LoggingLevel.Info;
        var date = DateTime.Now;

        _logger.Log(type, message, level);

        var formatter = new DefaultMessageFormatter();
        var formattedMessage = formatter.Format(new LogMessage(type, level, date, message));

        await Task.Delay(100);

        Mock.Get(_console).Verify(x => x.WriteLine(formattedMessage), Times.Once);
        Mock.Get(_console).VerifySet(x => x.ForegroundColor = It.IsAny<ConsoleColor>(), Times.Never());
        Mock.Get(_console).Verify(x => x.ResetColor(), Times.Never);
    }

    [Fact]
    public async void LogWarningTest()
    {
        var message = "test message";
        var type = GetType();
        var level = LoggingLevel.Warn;
        var date = DateTime.Now;

        _logger.Log(type, message, level);

        var formatter = new DefaultMessageFormatter();
        var formattedMessage = formatter.Format(new LogMessage(type, level, date, message));

        await Task.Delay(100);

        Mock.Get(_console).Verify(x => x.WriteLine(formattedMessage), Times.Once);
        Mock.Get(_console).VerifySet(x => x.ForegroundColor = ConsoleColor.Yellow, Times.Once);
        Mock.Get(_console).Verify(x => x.ResetColor(), Times.Once);
    }

    [Fact]
    public async void LogErrorTest()
    {
        var message = "test message";
        var type = GetType();
        var level = LoggingLevel.Error;
        var date = DateTime.Now;

        _logger.Log(type, message, level);

        var formatter = new DefaultMessageFormatter();
        var formattedMessage = formatter.Format(new LogMessage(type, level, date, message));

        await Task.Delay(100);

        Mock.Get(_console).Verify(x => x.WriteLine(formattedMessage), Times.Once);
        Mock.Get(_console).VerifySet(x => x.ForegroundColor = ConsoleColor.Red, Times.Once);
        Mock.Get(_console).Verify(x => x.ResetColor(), Times.Once);
    }

    public void Dispose()
    {
    }
}