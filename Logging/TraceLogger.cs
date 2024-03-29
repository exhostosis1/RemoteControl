using Logging.Abstract;
using Shared.ConsoleWrapper;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Logging;

public class TraceLogger(ITrace trace, LoggingLevel level = LoggingLevel.Info, IMessageFormatter? formatter = null) : AbstractLogger(level,
        formatter)
{
    protected override void ProcessInfo(string message)
    {
        trace.TraceInformation(message);
    }

    protected override void ProcessWarning(string message)
    {
        trace.TraceWarning(message);
    }

    protected override void ProcessError(string message)
    {
        trace.TraceError(message);
    }
}