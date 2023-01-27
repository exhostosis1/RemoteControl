using System.Diagnostics;

namespace Shared.ConsoleWrapper;

public class TraceWrapper: ITrace
{
    public void TraceInformation(string message) => Trace.TraceInformation(message);
    public void TraceWarning(string message) => Trace.TraceWarning(message);
    public void TraceError(string message) => Trace.TraceError(message);
}