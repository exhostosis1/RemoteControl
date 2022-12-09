using Shared;
using Shared.Config;
using Shared.Enums;

namespace RemoteControlConsole;

// ReSharper disable once InconsistentNaming
public class ConsoleUI: IUserInterface
{
    public event StringEventHandler? StartEvent;
    public event StringEventHandler? StopEvent;
    public event EmptyEventHandler? CloseEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event EmptyEventHandler? AddFirewallRuleEvent;
    public event ConfigEventHandler? ConfigChangedEvent;

    public IList<IControlProcessor> ControlProcessors { get; set; } = new List<IControlProcessor>();
    public bool IsAutostart { get; set; }

    // ReSharper disable once InconsistentNaming
    public void RunUI(AppConfig config)
    {
        DisplayInfo(config);

        while (true)
        {
            var key = Console.ReadLine();

            if (key == "x")
            {
                CloseEvent?.Invoke();
                return;
            }

            switch (key)
            {
                case "s":
                    if(ControlProcessors.Any(x => x.Status == ControlProcessorStatus.Working))
                        StopEvent?.Invoke(null);
                    else
                        StartEvent?.Invoke(null);
                    break;
                case "a":
                    AutostartChangedEvent?.Invoke(!IsAutostart);
                    break;
                default:
                    continue;
            }

            DisplayInfo(config);
        }
    }

    public void ShowError(string message)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
            
        Console.WriteLine(message);
            
        Console.ForegroundColor = color;
    }

    private void DisplayInfo(AppConfig config)
    {
        foreach (var controlProcessor in ControlProcessors)
        {
            Console.WriteLine(controlProcessor.Status == ControlProcessorStatus.Working ? $"{(controlProcessor.Type == ControlProcessorType.Server ? "Server" : "Bot")} listening on {controlProcessor.Info}" : "Server stopped");
            Console.WriteLine($"Autostart {(IsAutostart ? "enabled" : "disabled")}");
            Console.WriteLine();
        }

        Console.Write($"{(ControlProcessors.Any(x => x.Status == ControlProcessorStatus.Working) ? "[s]top" : "[s]tart")}, [a]utostart, e[x]it:");
    }
}