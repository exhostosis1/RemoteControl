using System;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Servers;

namespace WinUI;

public partial class ServerViewModel: ObservableObject
{
    public ServerType Type { get; init; }
    public int Id { get; init; }

    [ObservableProperty]
    private bool _status;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _schema;

    [ObservableProperty]
    private string _host;

    [ObservableProperty]
    private int _port;

    [ObservableProperty]
    private string _apiUrl;

    [ObservableProperty]
    private string _apiKey;

    [ObservableProperty]
    private string _usernames;

    public readonly Action<ServerConfig?> Start;
    public readonly Action Stop;

    public ServerViewModel(Server server, SynchronizationContext context)
    {
        Type = server.Type;
        _status = server.Status;
        _apiKey = server.Type == ServerType.Bot ? server.Config.ApiKey : "";
        _apiUrl = server.Type == ServerType.Bot ? server.Config.ApiUri : "";
        _host = server.Type == ServerType.Web ? server.Config.Host : "";
        _name = server.Config.Name;
        _port = server.Type == ServerType.Web ? server.Config.Port : -1;
        _schema = server.Type == ServerType.Web ? server.Config.Scheme : "";
        _usernames = server.Type == ServerType.Bot ? server.Config.UsernamesString : "";

        server.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != "Status" || sender is not Server s) return;

            context.Post(_ => Status = s.Status, null);
        };

        Start = server.Start;
        Stop = server.Stop;
    }
}