using Logging.Abstract;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace UnitTests.Logging;

public class AbstractLoggerTest: IDisposable
{
    private readonly TestLogger _logger;

    private class TestLogger: AbstractLogger
    {
        public int Count = 0;

        public TestLogger(LoggingLevel level = LoggingLevel.Error, IMessageFormatter? formatter = null) : base(level, formatter)
        {
        }

        protected override void ProcessInfo(string message)
        {
            throw new NotImplementedException();
        }

        protected override void ProcessWarning(string message)
        {
            throw new NotImplementedException();
        }

        protected override void ProcessError(string message)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Count++;
        }
    }

    public AbstractLoggerTest()
    {
        _logger = new TestLogger();
    }

    [Fact]
    public void FlushTest()
    {
        for (var i = 0; i < 10; i++)
        {
            _logger.Log(this.GetType(), "test message");
        }

        Task.Run(() =>
        {
            Thread.Sleep(1000);
            _logger.Log(this.GetType(), "locked message");
        });

        _logger.Flush();

        Assert.True(_logger.Count == 10);
    }

    public void Dispose()
    {
    }
}