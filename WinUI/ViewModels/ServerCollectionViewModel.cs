using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Servers;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUI.ViewModels;

public partial class ServerCollectionViewModel: ObservableObject
{
    private readonly App _app = Application.Current as App ?? throw new NullReferenceException();

    public ObservableCollection<ServerViewModel> Servers { get; } = [];
    private readonly RelayCommand<ServerViewModel> _removeCommand;

    [ObservableProperty] private bool _errorShow;
    [ObservableProperty] private string _errorMessage;

    public bool HostAutostart
    {
        get => _app.Host.GetAutorun();
        set
        {
            _app.Host.SetAutorun(value);
            OnPropertyChanged(nameof(HostAutostart));
        }
    }

    public ServerCollectionViewModel()
    {
        _removeCommand = new (vm => Servers.Remove(vm ?? throw new NullReferenceException()));

        ReloadServers();
    }

    private void ReloadServers()
    {
        foreach (var serverViewModel in Servers)
        {
            serverViewModel.StopCommand.Execute(true);
            serverViewModel.Dispose();
        }

        Servers.Clear();

        foreach (var server in _app.Host.GetServers())
        {
            Servers.Add(new ServerViewModel(server, _removeCommand));

            if (server.Config.AutoStart)
            {
                try
                {
                    server.Start();
                }
                catch (Exception e)
                {
                    ErrorMessage = e.Message;
                    ErrorShow = true;
                }
            }
        }
    }

    [RelayCommand]
    private void Reload()
    {
        ReloadServers();
    }

    [RelayCommand]
    private void Add(ServerType type)
    {
        Servers.Add(new ServerViewModel(_app.Host.ServerFactory.GetServer(new ServerConfig(type)), _removeCommand));
    }

    [RelayCommand]
    private void AddFirewallRules()
    {
        AppHost.AddFirewallRules(Servers.Where(x => x.Status).Select(x => new Uri(x.ListeningUri)));
    }

    [RelayCommand]
    private void UpdateConfig()
    {
        _app.Host.SaveConfig(Servers.Select(x => x.GetConfig()));
    }

    [RelayCommand]
    private void StartAll()
    {
        foreach (var serverViewModel in Servers)
        {
            serverViewModel.StartCommand.Execute(null);
        }
    }

    [RelayCommand]
    private void StopAll()
    {
        foreach (var serverViewModel in Servers)
        {
            serverViewModel.StopCommand.Execute(null);
        }
    }
}