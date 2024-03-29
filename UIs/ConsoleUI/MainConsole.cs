using Shared.Config;
using Shared.Enums;
using Shared.Observable;
using Shared.Server;
using Shared.UI;

namespace ConsoleUI;

// ReSharper disable once InconsistentNaming
public class MainConsole : IUserInterface
{
    public IObservable<int?> ServerStart => _serverStart;
    public IObservable<int?> ServerStop => _serverStop;
    public IObservable<object?> AppClose => _appClose;
    public IObservable<bool> AutoStartChange => _autoStartChange;
    public IObservable<(int, CommonConfig)> ConfigChange => _configChange;
    public IObservable<ServerType> ServerAdd => _serverAdd;
    public IObservable<int> ServerRemove => _serverRemove;

    private readonly MyObservable<int?> _serverStart = new();
    private readonly MyObservable<int?> _serverStop = new();
    private readonly MyObservable<object?> _appClose = new();
    private readonly MyObservable<bool> _autoStartChange = new();
    private readonly MyObservable<(int, CommonConfig)> _configChange = new();
    private readonly MyObservable<ServerType> _serverAdd = new();
    private readonly MyObservable<int> _serverRemove = new();

    public void SetAutoStartValue(bool value)
    {
        IsAutoStart = value;
    }

    private List<IServer> Model { get; set; } = [];

    private bool IsAutoStart { get; set; }

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
                _appClose.Next(null);
                return;
            }

            switch (key)
            {
                case "s":
                    if (Model.Any(x => x.Status.Working))
                        _serverStop.Next(null);
                    else
                        _serverStart.Next(null);
                    break;
                case "a":
                    _autoStartChange.Next(!IsAutoStart);
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
        _serverAdd.Next(ServerType.Http);
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

            Console.WriteLine($"AutoStart {(IsAutoStart ? "enabled" : "disabled")}");
            Console.WriteLine();
        }

        Console.Write($"{(dtos.Any(x => x.Status.Working) ? "[s]top" : "[s]tart")}, [a]utoStart, e[x]it:");
    }

    protected void OnProcessorRemovedEvent(int e)
    {
        _serverRemove.Next(e);
    }

    protected void OnConfigChangedEvent((int, CommonConfig) e)
    {
        _configChange.Next(e);
    }
}