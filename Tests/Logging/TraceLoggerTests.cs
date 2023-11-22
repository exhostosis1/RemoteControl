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
    private readonly TestMessageFormatter _formatter = new();

    public TraceLoggerTests()
    {
        _trace = new Mock<ITrace>(MockBehavior.Strict);
        _logger = new TraceLogger(_trace.Object, LoggingLevel.Info, _formatter);
    }

    [Fact]
    public async Task LogInfoTest()
    {
        _trace.Setup(x => x.TraceInformation(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Info;
        var date = DateTime.Now;

        await _logger.LogAsync(type, message, level);
        
        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        _trace.Verify(x => x.TraceInformation(formattedMessage), Times.Once);
    }

    [Fact]
    public async Task LogWarningTest()
    {
        _trace.Setup(x => x.TraceWarning(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Warn;
        var date = DateTime.Now;

        await _logger.LogAsync(type, message, level);
        
        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        _trace.Verify(x => x.TraceWarning(formattedMessage), Times.Once);
    }

    [Fact]
    public async Task LogErrorTest()
    {
        _trace.Setup(x => x.TraceError(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Error;
        var date = DateTime.Now;

        await _logger.LogAsync(type, message, level);
        
        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        _trace.Verify(x => x.TraceError(formattedMessage), Times.Once);
    }

    [Theory]
    [InlineData(1000)]
    public void ParallelLoggingTest(int count)
    {
        _trace.Setup(x => x.TraceInformation(It.IsAny<string>()));

        var tasks = new Task[count];
        for (var i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() => _logger.Log(GetType(), "test message", LoggingLevel.Info));
        }

        Task.WaitAll(tasks);

        _trace.Verify(x => x.TraceInformation(It.IsAny<string>()), Times.Exactly(count));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}