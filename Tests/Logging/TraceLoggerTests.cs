using Logging;
using Logging.Formatters;
using Moq;
using Shared.ConsoleWrapper;
using Shared.Enums;
using Shared.Logging;
using Shared.Logging.Interfaces;

namespace UnitTests.Logging;

public class TraceLoggerTests : IDisposable
{
    private readonly TraceLogger _logger;
    private readonly Mock<ITrace> _trace;
    private readonly IMessageFormatter _formatter = new TestMessageFormatter();

    public TraceLoggerTests()
    {
        _trace = new Mock<ITrace>(MockBehavior.Strict);
        _logger = new TraceLogger(_trace.Object, LoggingLevel.Info, _formatter);
    }

    [Fact]
    public void LogInfoTest()
    {
        _trace.Setup(x => x.TraceInformation(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Info;
        var date = DateTime.Now;

        _logger.Log(type, message, level);
        
        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        _logger.Flush();

        _trace.Verify(x => x.TraceInformation(formattedMessage), Times.Once);
    }

    [Fact]
    public void LogWarningTest()
    {
        _trace.Setup(x => x.TraceWarning(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Warn;
        var date = DateTime.Now;

        _logger.Log(type, message, level);
        
        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        _logger.Flush();

        _trace.Verify(x => x.TraceWarning(formattedMessage), Times.Once);
    }

    [Fact]
    public void LogErrorTest()
    {
        _trace.Setup(x => x.TraceError(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Error;
        var date = DateTime.Now;

        _logger.Log(type, message, level);
        
        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        _logger.Flush();

        _trace.Verify(x => x.TraceError(formattedMessage), Times.Once);
    }

    public void Dispose()
    {
    }
}