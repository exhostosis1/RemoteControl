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
    private readonly TestMessageFormatter _formatter = new();

    public ConsoleLoggerTests()
    {
        _console = new Mock<IConsole>(MockBehavior.Strict);
        _logger = new ConsoleLogger(_console.Object, LoggingLevel.Info, _formatter);
    }

    [Fact]
    public async Task LogInfoTest()
    {
        _console.Setup(x => x.WriteLine(It.IsAny<string>()));

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Info;

        await _logger.LogAsync(type, message, level);

        var formattedMessage = _formatter.Format(new LogMessage(type, level, DateTime.Now, message));

        _console.Verify(x => x.WriteLine(formattedMessage), Times.Once);
    }

    [Fact]
    public async Task LogWarningTest()
    {
        _console.Setup(x => x.WriteLine(It.IsAny<string>()));
        _console.SetupSet(x => x.ForegroundColor = ConsoleColor.Yellow);
        _console.Setup(x => x.ResetColor());

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Warn;
        var date = DateTime.Now;

        await _logger.LogAsync(type, message, level);
        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        _console.Verify(x => x.WriteLine(formattedMessage), Times.Once);
        _console.VerifySet(x => x.ForegroundColor = ConsoleColor.Yellow, Times.Once);
        _console.Verify(x => x.ResetColor(), Times.Once);
    }

    [Fact]
    public async Task LogErrorTest()
    {
        _console.Setup(x => x.WriteLine(It.IsAny<string>()));
        _console.SetupSet(x => x.ForegroundColor = ConsoleColor.Red);
        _console.Setup(x => x.ResetColor());

        const string message = "test message";
        var type = GetType();
        const LoggingLevel level = LoggingLevel.Error;
        var date = DateTime.Now;

        await _logger.LogAsync(type, message, level);
        
        var formattedMessage = _formatter.Format(new LogMessage(type, level, date, message));

        _console.Verify(x => x.WriteLine(formattedMessage), Times.Once);
        _console.VerifySet(x => x.ForegroundColor = ConsoleColor.Red, Times.Once);
        _console.Verify(x => x.ResetColor(), Times.Once);
    }

    [Theory]
    [InlineData(1000)]
    public void ParallelLoggingTest(int count)
    {
        _console.Setup(x => x.WriteLine(It.IsAny<string>()));

        var tasks = new Task[count];
        for (var i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() => _logger.Log(GetType(), "test message", LoggingLevel.Info));
        }

        Task.WaitAll(tasks);

        _console.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Exactly(count));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}