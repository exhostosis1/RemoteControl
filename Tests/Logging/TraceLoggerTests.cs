using Logging;
using Logging.Formatters;
using Moq;
using Shared.ConsoleWrapper;
using Shared.Enums;
using Shared.Logging;

namespace UnitTests.Logging;

public class TraceLoggerTests : IDisposable
{
    private readonly TraceLogger _logger;
    private readonly Mock<ITrace> _trace;

    public TraceLoggerTests()
    {
        _trace = new Mock<ITrace>(MockBehavior.Strict);
        _logger = new TraceLogger(_trace.Object);
    }

    [Fact]
    public async void LogInfoTest()
    {
        _trace.Setup(x => x.TraceInformation(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Info;
        var date = DateTime.Now;

        _logger.Log(type, message, level);

        var formatter = new DefaultMessageFormatter();
        var formattedMessage = formatter.Format(new LogMessage(type, level, date, message));

        await Task.Delay(100);

        _trace.Verify(x => x.TraceInformation(formattedMessage), Times.Once);
    }

    [Fact]
    public async void LogWarningTest()
    {
        _trace.Setup(x => x.TraceWarning(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Warn;
        var date = DateTime.Now;

        _logger.Log(type, message, level);

        var formatter = new DefaultMessageFormatter();
        var formattedMessage = formatter.Format(new LogMessage(type, level, date, message));

        await Task.Delay(100);

        _trace.Verify(x => x.TraceWarning(formattedMessage), Times.Once);
    }

    [Fact]
    public async void LogErrorTest()
    {
        _trace.Setup(x => x.TraceError(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Error;
        var date = DateTime.Now;

        _logger.Log(type, message, level);

        var formatter = new DefaultMessageFormatter();
        var formattedMessage = formatter.Format(new LogMessage(type, level, date, message));

        await Task.Delay(100);

        _trace.Verify(x => x.TraceError(formattedMessage), Times.Once);
    }

    public void Dispose()
    {
    }
}