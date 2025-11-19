using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Servers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace WinUI.ViewModels;

public partial class ServerCollectionViewModel : ObservableObject
{
    private readonly AppHost _app;

    public ObservableCollection<ServerViewModel> Servers { get; } = [];
    private readonly RelayCommand<ServerViewModel> _removeCommand;

    public bool HostAutostart
    {
        get => _app.GetAutorun();
        set
        {
            _app.SetAutorun(value);
            OnPropertyChanged();
        }
    }

    public ServerCollectionViewModel(AppHost app)
    {
        _app = app;
        _removeCommand = new(vm => Servers.Remove(vm ?? throw new NullReferenceException()));

        ReloadServers();
        SetSystemEvents();
    }

    private void ReloadServers()
    {
        foreach (var serverViewModel in Servers)
        {
            serverViewModel.StopCommand.Execute(true);
            serverViewModel.Dispose();
        }

        Servers.Clear();

        foreach (var server in _app.GetWorkers())
        {

            var vm = new ServerViewModel(server, _removeCommand);
            Servers.Add(vm);

            if (vm.StartAutomatically)
            {
                vm.StartCommand.Execute(null);
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
        Servers.Add(new ServerViewModel(_app.ServerFactory.GetServer(new ServerConfig(type)), _removeCommand, true));
    }

    [RelayCommand]
    private void AddFirewallRules()
    {
        _app.AddFirewallRules(Servers.Where(x => x.Status).Select(x => new Uri(x.ListeningUri)));
    }

    [RelayCommand]
    private void AddPermissionsToUser()
    {
        _app.AddListeningPermissionsToUser(Servers.Where(x => x is { Type: ServerType.Web, Status: false }).Select(x => x.ListeningUri));
    }

    [RelayCommand]
    private static void OpenSite(string uri)
    {
        AppHost.OpenSite(uri);
    }

    [RelayCommand]
    private void UpdateConfig()
    {
        _app.SaveConfig(Servers.Select(x => x.GetConfig()));
    }

    [RelayCommand]
    private void StartAll()
    {
        foreach (var serverViewModel in Servers)
        {
            serverViewModel.Status = true;
        }
    }

    [RelayCommand]
    private void StopAll()
    {
        foreach (var serverViewModel in Servers)
        {
            serverViewModel.Status = false;
        }
    }

    private void SetSystemEvents()
    {
        SystemEvents.SessionSwitch += SessionSwitchHandler;
    }

    private List<ServerViewModel> _runningServers = [];

    private void SessionSwitchHandler(object sender, SessionSwitchEventArgs args)
    {
        switch (args.Reason)
        {
            case SessionSwitchReason.SessionLock:
                {
                    _app.GetLogger().LogInformation("Stopping servers due to logout");

                    _runningServers = [.. Servers.Where(x => x.Status)];
                    _runningServers.ForEach(x => x.Status = false);

                    break;
                }
            case SessionSwitchReason.SessionUnlock:
                {
                    _app.GetLogger().LogInformation("Restoring servers");

                    _runningServers.ForEach(x => x.Status = true);
                    break;
                }
            case SessionSwitchReason.ConsoleConnect:
            case SessionSwitchReason.ConsoleDisconnect:
            case SessionSwitchReason.RemoteConnect:
            case SessionSwitchReason.RemoteDisconnect:
            case SessionSwitchReason.SessionLogon:
            case SessionSwitchReason.SessionLogoff:
            case SessionSwitchReason.SessionRemoteControl:
            default:
                break;

        }
    }
}