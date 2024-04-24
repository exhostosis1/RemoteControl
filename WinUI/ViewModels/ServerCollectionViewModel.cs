using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Servers;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace WinUI.ViewModels;

public partial class ServerCollectionViewModel
{
    private readonly App _app = Application.Current as App ?? throw new NullReferenceException();

    public ObservableCollection<IServer> Servers { get; }

    public event EventHandler<string>? Error; 

    public ServerCollectionViewModel()
    {
        Servers = new ObservableCollection<IServer>(_app.Host.GetServers());

        foreach (var server in Servers.Where(x => x.Config.AutoStart))
        {
            try
            {
                server.Start();
            }
            catch (Exception e)
            {
                Error?.Invoke(this, e.Message);
            }
        }
    }

    [RelayCommand]
    private void Reload()
    {
        Servers.Clear();

        try
        {
            foreach (var server in _app.Host.GetServers())
            {
                Servers.Add(server);
            }
        }
        catch (Exception e)
        {
            Error?.Invoke(this, e.Message);
        }
    }

    [RelayCommand]
    private void Add(ServerType type)
    {
        Servers.Add(_app.Host.ServerFactory.GetServer(new ServerConfig(type)));
    }

    [RelayCommand]
    private void AddFirewallRules()
    {
        AppHost.AddFirewallRules(Servers.Where(x => x.Status).Select(x => x.Config.Uri));
    }

    public void RemoveServer(IServer server)
    {
        Servers.Remove(server);
    }

    [RelayCommand]
    private void UpdateConfig()
    {
        _app.Host.SaveConfig(Servers.Select(x => x.Config));
    }
}