using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp;
using Servers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WinUI;

public partial class ViewModel: ObservableObject
{
    private readonly AppHost _app;

    internal ObservableCollection<ServerViewModel> Servers { get; private set; } = [];
    private readonly SynchronizationContext _context;

    [ObservableProperty] 
    private bool _isAutostart;

    internal ViewModel()
    {
        _context = SynchronizationContext.Current;

        _app = Task.Run(() => new AppHostBuilder().Build()).Result;

        _isAutostart = _app.IsAutostart;

        _app.RunAll();
        Servers = new ObservableCollection<ServerViewModel>(
            _app.Servers.Select(x => new ServerViewModel(x, _context)));
    }

    [RelayCommand]
    private void SetAutostart(bool value)
    {
        _app.IsAutostart = value;
    }

    [RelayCommand]
    private void Start(ServerViewModel? model)
    {
        if(model == null)
            foreach (var server in Servers)
            {
                server.Start(null);
            }
        else
            model.Start(null);
    }

    [RelayCommand]
    private void Stop(ServerViewModel? model)
    {
        if (model == null)
            foreach (var server in Servers)
            {
                server.Stop();
            }
        else
            model.Stop();
    }

    [RelayCommand]
    private void AddServer(ServerType mode)
    {
        Servers.Add(new ServerViewModel(_app.ServerFactory.GetServer(mode), _context));
    }

    [RelayCommand]
    private void RemoveServer(ServerViewModel model)
    {
        Servers.Remove(model);
    }
}