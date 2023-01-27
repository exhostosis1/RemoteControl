using Shared.Config;
using Shared.Enums;
using Shared.Server;
using Shared.UI;

namespace ConsoleUI;

// ReSharper disable once InconsistentNaming
public class MainConsole : IUserInterface
{
    public event EventHandler<int?>? OnStart;
    public event EventHandler<int?>? OnStop;
    public event EventHandler? OnClose;
    public event EventHandler<bool>? OnAutostartChanged;
    public event EventHandler<(int, CommonConfig)>? OnConfigChanged;
    public event EventHandler<ServerType>? OnServerAdded;
    public event EventHandler<int>? OnServerRemoved;

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
                OnClose?.Invoke(null, EventArgs.Empty);
                return;
            }

            switch (key)
            {
                case "s":
                    if (Model.Any(x => x.Status.Working))
                        OnStart?.Invoke(null, null);
                    else
                        OnStart?.Invoke(null, null);
                    break;
                case "a":
                    OnAutostartChanged?.Invoke(null, !IsAutostart);
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

    public void AddServer(IServer server)
    {
        OnServerAdded?.Invoke(null, ServerType.Http);
    }

    private void DisplayInfo(List<IServer> dtos)
    {
        foreach (var dto in dtos)
        {
            switch (dto)
            {
                case IServer<WebConfig> s:
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

    protected void OnProcessorRemovedEvent(int e)
    {
        OnServerRemoved?.Invoke(this, e);
    }

    protected void OnConfigChangedEvent((int, CommonConfig) e)
    {
        OnConfigChanged?.Invoke(this, e);
    }
}