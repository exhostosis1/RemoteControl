using Logging.Abstract;
using Shared.Enums;
using Shared.Logging.Interfaces;
using System.Diagnostics;

namespace Logging;

public class TraceLogger: AbstractLogger
{
    public TraceLogger(Type callerType, LoggingLevel level = LoggingLevel.Info, IMessageFormatter? formatter = null) : base(callerType, level,
        formatter)
    { }

    protected override void ProcessInfo(string message)
    {
        Trace.TraceInformation(message);
    }

    protected override void ProcessWarning(string message)
    {
        Trace.TraceWarning(message);
    }

    protected override void ProcessError(string message)
    {
        Trace.TraceError(message);
    }
}