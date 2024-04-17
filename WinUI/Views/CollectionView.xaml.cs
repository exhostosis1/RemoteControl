using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Interfaces;
using MainApp.Servers;
using System.Collections.ObjectModel;
using System.Linq;
using WinUI.ViewModels;

namespace WinUI.Views;

[ObservableObject]
internal sealed partial class CollectionView
{
    public readonly ObservableCollection<ServerViewModel> Servers = [];
    private AppHost _app = ApphostProvider.AppHost;

    public CollectionView()
    {
        this.InitializeComponent();

        PopulateServers();
        _app.ServerStatusChanged += (_, value) =>
        {
            var model = Servers.First(x => x.Id == value.Item1);
            model.Status = value.Item2;
        };
    }

    [RelayCommand]
    private void AddServer(ServerType type)
    {
        _app.AddServer(type);
    }

    private void PopulateServers()
    {
        Servers.Clear();

        foreach (var server in _app.GetServers())
        {
            var model = new ServerViewModel
            {
                Id = server.Id,
                Status = server.Status,
                IsAutostart = server.IsAutostart,
                Name = server.Name
            };

            switch (server)
            {
                case IWebServer web:
                    model.Type = ServerType.Web;
                    model.ListeningUri = web.ListeningUri;
                    break;
                case IBotServer bot:
                    model.Type = ServerType.Bot;
                    model.ApiUri = bot.ApiUri;
                    model.ApiKey = bot.ApiKey;
                    model.Usernames = bot.Usernames;
                    break;
                default:
                    break;
            }

            Servers.Add(model);
        }
    }
}