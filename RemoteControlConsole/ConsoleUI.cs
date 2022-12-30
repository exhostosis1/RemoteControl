using Shared;
using Shared.Enums;

namespace RemoteControlConsole;

// ReSharper disable once InconsistentNaming
public class ConsoleUI: IUserInterface
{
    public event ProcessorEventHandler? StartEvent;
    public event ProcessorEventHandler? StopEvent;
    public event EmptyEventHandler? CloseEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event EmptyEventHandler? AddFirewallRuleEvent;
    public event ConfigEventHandler? ConfigChangedEvent;
    public void SetViewModel(IEnumerable<ControlProcessorDto> model)
    {
        Model = model.ToList();
    }

    public void SetAutostartValue(bool value)
    {
        IsAutostart = value;
    }

    private List<ControlProcessorDto> Model { get; set; } = new();

    public bool IsAutostart { get; set; }

    // ReSharper disable once InconsistentNaming
    public void RunUI()
    {
        DisplayInfo(Model);

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
                    if(Model.Any(x => x.Running))
                        StopEvent?.Invoke(null, ControlProcessorType.Common);
                    else
                        StartEvent?.Invoke(null, ControlProcessorType.Common);
                    break;
                case "a":
                    AutostartChangedEvent?.Invoke(!IsAutostart);
                    break;
                default:
                    continue;
            }

            DisplayInfo(Model);
        }
    }

    public void ShowError(string message)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
            
        Console.WriteLine(message);
            
        Console.ForegroundColor = color;
    }

    private void DisplayInfo(List<ControlProcessorDto> dtos)
    {
        foreach (var dto in dtos)
        {
            switch (dto)
            {
                case ServerDto s:
                    Console.WriteLine(s.Running ? $"Server {s.Name} listening on {s.ListeningUri}" : $"Server {s.Name} stopped");
                    break;
                case BotDto b:
                    Console.WriteLine(b.Running ? $"Bot {b.Name} responds to {b.BotUsernames}" : $"Bot {b.Name} stopped");
                    break;
            }

            Console.WriteLine($"Autostart {(IsAutostart ? "enabled" : "disabled")}");
            Console.WriteLine();
        }

        Console.Write($"{(dtos.Any(x => x.Running) ? "[s]top" : "[s]tart")}, [a]utostart, e[x]it:");
    }
}