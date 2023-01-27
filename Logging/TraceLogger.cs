using Logging.Abstract;
using Shared.ConsoleWrapper;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Logging;

public class TraceLogger : AbstractLogger
{
    private readonly ITrace _trace;

    public TraceLogger(ITrace trace, LoggingLevel level = LoggingLevel.Info, IMessageFormatter? formatter = null) :
        base(level,
            formatter)
    {
        _trace = trace;
    }

    protected override void ProcessInfo(string message)
    {
        _trace.TraceInformation(message);
    }

    protected override void ProcessWarning(string message)
    {
        _trace.TraceWarning(message);
    }

    protected override void ProcessError(string message)
    {
        _trace.TraceError(message);
    }
}