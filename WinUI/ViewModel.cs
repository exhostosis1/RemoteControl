using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp;
using Servers;
using System;
using System.Threading.Tasks;

namespace WinUI;

public partial class ViewModel: ObservableObject
{
    public readonly AppHost App;

    internal ViewModel()
    {
        App = new AppHostBuilder().Build();

        App.RunAll();
    }

    [RelayCommand]
    private void SetAutostart(bool value)
    {
        App.IsAutostart = value;
    }

    [RelayCommand]
    private void StartAll()
    {
        foreach (var server in App.Servers)
        {
            server.Start();
        }
    }

    [RelayCommand]
    private void StopAll()
    {
        foreach (var server in App.Servers)
        {
            server.Stop();
        }
    }

    [RelayCommand]
    private void Start(Server server)
    {
        server.Start();
    }

    [RelayCommand]
    private void Stop(Server server)
    {
        server.Stop();
    }

    [RelayCommand]
    private void AddServer(ServerType mode)
    {
        App.Servers.Add(App.ServerFactory.GetServer(mode));
    }

    [RelayCommand]
    private void RemoveServer(Server server)
    {
        App.Servers.Remove(server);
    }

    [RelayCommand]
    private void AddFirewallRules()
    {
        App.AddFirewallRules();
    }

    [RelayCommand]
    private void OpenSite(Server server)
    {
        App.OpenSite(server);
    }

    [RelayCommand]
    private void Exit()
    {
        Environment.Exit(0);
    }
}