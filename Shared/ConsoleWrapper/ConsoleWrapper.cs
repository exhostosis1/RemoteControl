using System;

namespace Shared.ConsoleWrapper;

public class ConsoleWrapper : IConsole
{
    public void WriteLine(string? value) => Console.WriteLine(value);

    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }
    public void ResetColor() => Console.ResetColor();
}