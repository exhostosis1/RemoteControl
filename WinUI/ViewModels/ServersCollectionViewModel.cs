using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Servers;
using System.Collections.ObjectModel;

namespace WinUI.ViewModels;

internal partial class ServersCollectionViewModel
{
    private static readonly AppHost AppHost = ApphostProvider.AppHost;

    public ObservableCollection<IServer> Servers = AppHost.Servers;

    [RelayCommand]
    private void Reload()
    {
        AppHost.ReloadServers();
    }

    [RelayCommand]
    private void Add(ServerType type)
    {
        AppHost.AddServer(type);
    }

    [RelayCommand]
    private void AddFirewallRules()
    {
        AppHost.AddFirewallRules();
    }
}