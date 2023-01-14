using System;

namespace Shared.ConsoleWrapper;

public interface IConsole
{
    public void WriteLine(string? value);
    public ConsoleColor ForegroundColor { get; set; }
    public void ResetColor();
}