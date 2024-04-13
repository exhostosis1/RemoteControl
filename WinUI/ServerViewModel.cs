using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Servers;

namespace WinUI;

internal partial class ServerViewModel: ObservableObject
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

    public static ServerViewModel Create(Server server, SynchronizationContext context)
    {
        var vm = new ServerViewModel
        {
            Type = server.Type,
            Id = server.Id,
            Status = server.Status,
            ApiKey = server.Type == ServerType.Bot ? server.Config.ApiKey : "",
            ApiUrl = server.Type == ServerType.Bot ? server.Config.ApiUri : "",
            Host = server.Type == ServerType.Web ? server.Config.Host : "",
            Name = server.Config.Name,
            Port = server.Type == ServerType.Web ? server.Config.Port : -1,
            Schema = server.Type == ServerType.Web ? server.Config.Scheme : "",
            Usernames = server.Type == ServerType.Bot ? server.Config.UsernamesString : ""
        };

        server.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName != "Status" || sender is not Server s) return;

            context.Post(_ => vm.Status = s.Status, null);
        };

        return vm;
    }
}