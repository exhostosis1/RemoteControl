using Logging.Abstract;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Logging;

public class ConsoleLogger : AbstractLogger
{
    public ConsoleLogger(Type callerType, LoggingLevel level = LoggingLevel.Info, IMessageFormatter? formatter = null) : base(callerType, level, formatter)
    {}

    protected override void ProcessInfo(string message)
    {
        Console.WriteLine(message);
    }

    protected override void ProcessWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    protected override void ProcessError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}