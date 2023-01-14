using Logging.Abstract;
using Shared.ConsoleWrapper;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Logging;

public class ConsoleLogger: AbstractLogger
{
    private readonly IConsole _console;

    public ConsoleLogger(IConsole console, LoggingLevel level = LoggingLevel.Info, IMessageFormatter? formatter = null)
        : base(level, formatter)
    {
        _console = console;
    }

    protected override void ProcessInfo(string message)
    {
        _console.WriteLine(message);
    }

    protected override void ProcessWarning(string message)
    {
        _console.ForegroundColor = ConsoleColor.Yellow;
        _console.WriteLine(message);
        _console.ResetColor();
    }

    protected override void ProcessError(string message)
    {
        _console.ForegroundColor = ConsoleColor.Red;
        _console.WriteLine(message);
        _console.ResetColor();
    }
}