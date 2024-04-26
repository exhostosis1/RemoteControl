using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Servers;
using System.Collections.ObjectModel;

namespace MainApp.ViewModels;

public partial class ServerCollectionViewModel: ObservableObject
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
            OnPropertyChanged(nameof(HostAutostart));
        }
    }

    public ServerCollectionViewModel(AppHost app)
    {
        _app = app;
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

        foreach (var server in _app.GetServers())
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
        Servers.Add(new ServerViewModel(_app.ServerFactory.GetServer(new ServerConfig(type)), _removeCommand));
    }

    [RelayCommand]
    private void AddFirewallRules()
    {
        AppHost.AddFirewallRules(Servers.Where(x => x.Status).Select(x => new Uri(x.ListeningUri)));
    }

    [RelayCommand]
    private void OpenSite(string uri)
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