using Logging.Abstract;
using Shared.ConsoleWrapper;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Logging;

public class ConsoleLogger(IConsole console, LoggingLevel level = LoggingLevel.Info, IMessageFormatter? formatter = null) : AbstractLogger(level, formatter)
{
    protected override void ProcessInfo(string message)
    {
        console.WriteLine(message);
    }

    protected override void ProcessWarning(string message)
    {
        console.ForegroundColor = ConsoleColor.Yellow;
        console.WriteLine(message);
        console.ResetColor();
    }

    protected override void ProcessError(string message)
    {
        console.ForegroundColor = ConsoleColor.Red;
        console.WriteLine(message);
        console.ResetColor();
    }
}