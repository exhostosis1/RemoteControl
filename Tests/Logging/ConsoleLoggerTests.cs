using Logging;
using Logging.Formatters;
using Moq;
using Shared.ConsoleWrapper;
using Shared.Enums;
using Shared.Logging;

namespace UnitTests.Logging;

public class ConsoleLoggerTests : IDisposable
{
    private readonly ConsoleLogger _logger;
    private readonly Mock<IConsole> _console;

    public ConsoleLoggerTests()
    {
        _console = new Mock<IConsole>(MockBehavior.Strict);
        _logger = new ConsoleLogger(_console.Object);
    }

    [Fact]
    public void LogInfoTest()
    {
        _console.Setup(x => x.WriteLine(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Info;
        var date = DateTime.Now;

        _logger.Log(type, message, level);

        var formatter = new DefaultMessageFormatter();
        var formattedMessage = formatter.Format(new LogMessage(type, level, date, message));

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

        var formatter = new DefaultMessageFormatter();
        var formattedMessage = formatter.Format(new LogMessage(type, level, date, message));

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

        var formatter = new DefaultMessageFormatter();
        var formattedMessage = formatter.Format(new LogMessage(type, level, date, message));

        _logger.Flush();

        _console.Verify(x => x.WriteLine(formattedMessage), Times.Once);
        _console.VerifySet(x => x.ForegroundColor = ConsoleColor.Red, Times.Once);
        _console.Verify(x => x.ResetColor(), Times.Once);
    }

    public void Dispose()
    {
    }
}