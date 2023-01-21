using Shared.Config;
using Shared.Server;
using Shared.UI;

namespace ConsoleUI;

// ReSharper disable once InconsistentNaming
public class MainConsole: IUserInterface
{
    public event EventHandler<int?>? StartEvent;
    public event EventHandler<int?>? StopEvent;
    public event EventHandler? CloseEvent;
    public event EventHandler<bool>? AutostartChangedEvent;
    public event EventHandler<(int, CommonConfig)>? ConfigChangedEvent;
    public event EventHandler<string>? ProcessorAddedEvent;
    public event EventHandler<int>? ProcessorRemovedEvent;

    public void SetAutostartValue(bool value)
    {
        IsAutostart = value;
    }

    private List<IServer> Model { get; set; } = new();

    private bool IsAutostart { get; set; }

    // ReSharper disable once InconsistentNaming
    public void RunUI(List<IServer> processors)
    {
        Model = processors;
        DisplayInfo(Model);

        while (true)
        {
            var key = Console.ReadLine();

            if (key == "x")
            {
                CloseEvent?.Invoke(null, EventArgs.Empty);
                return;
            }

            switch (key)
            {
                case "s":
                    if(Model.Any(x => x.Status.Working))
                        StopEvent?.Invoke(null, null);
                    else
                        StartEvent?.Invoke(null, null);
                    break;
                case "a":
                    AutostartChangedEvent?.Invoke(null, !IsAutostart);
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

    public void AddProcessor(IServer processor)
    {
        throw new NotImplementedException();
    }

    private void DisplayInfo(List<IServer> dtos)
    {
        foreach (var dto in dtos)
        {
            switch (dto)
            {
                case IServer<ServerConfig> s:
                    Console.WriteLine(s.Status.Working ? $"Server {s.CurrentConfig.Name} listening on {s.CurrentConfig.Uri}" : $"Server {s.CurrentConfig.Name} stopped");
                    break;
                case IServer<BotConfig> b:
                    Console.WriteLine(b.Status.Working ? $"Bot {b.CurrentConfig.Name} responds to {b.CurrentConfig.UsernamesString}" : $"Bot {b.CurrentConfig.Name} stopped");
                    break;
            }

            Console.WriteLine($"Autostart {(IsAutostart ? "enabled" : "disabled")}");
            Console.WriteLine();
        }

        Console.Write($"{(dtos.Any(x => x.Status.Working) ? "[s]top" : "[s]tart")}, [a]utostart, e[x]it:");
    }
}