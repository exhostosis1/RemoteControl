using Logging;
using Logging.Formatters;
using Moq;
using Shared.ConsoleWrapper;
using Shared.Enums;
using Shared.Logging;
using Shared.Logging.Interfaces;

namespace UnitTests.Logging;

public class ConsoleLoggerTests : IDisposable
{
    private readonly ConsoleLogger _logger;
    private readonly Mock<IConsole> _console;
    private readonly IMessageFormatter _formatter = new TestMessageFormatter();

    public ConsoleLoggerTests()
    {
        _console = new Mock<IConsole>(MockBehavior.Strict);
        _logger = new ConsoleLogger(_console.Object, LoggingLevel.Info, _formatter);
    }

    [Fact]
    public void LogInfoTest()
    {
        _console.Setup(x => x.WriteLine(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Info;

        _logger.Log(type, message, level);

        var formattedMessage = _formatter.Format(new LogMessage(type, level, DateTime.Now, message));

        _logger.Flush();

        _console.Verify(x => x.WriteLine(formattedMessage), Times.Once);
    }

    [Fact]
    public void LogWarningTest()
    {
        _console.Setup(x => x.WriteLine(It.IsAny<string>()));
        _console.SetupSet(x => x.ForegroundColor = ConsoleColor.Yellow);
        _console.Setup(x => x.ResetColor());

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Warn;
        var date = DateTime.Now;

        _logger.Log(type, message, level);
        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        _logger.Flush();

        _console.Verify(x => x.WriteLine(formattedMessage), Times.Once);
        _console.VerifySet(x => x.ForegroundColor = ConsoleColor.Yellow, Times.Once);
        _console.Verify(x => x.ResetColor(), Times.Once);
    }

    [Fact]
    public void LogErrorTest()
    {
        _console.Setup(x => x.WriteLine(It.IsAny<string>()));
        _console.SetupSet(x => x.ForegroundColor = ConsoleColor.Red);
        _console.Setup(x => x.ResetColor());

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Error;
        var date = DateTime.Now;

        _logger.Log(type, message, level);
        
        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        _logger.Flush();

        _console.Verify(x => x.WriteLine(formattedMessage), Times.Once);
        _console.VerifySet(x => x.ForegroundColor = ConsoleColor.Red, Times.Once);
        _console.Verify(x => x.ResetColor(), Times.Once);
    }

    public void Dispose()
    {
    }
}