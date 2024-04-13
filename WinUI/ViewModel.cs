using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MainApp;
using Servers;
using CommunityToolkit.Mvvm;
using CommunityToolkit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WinUI;

public partial class ViewModel: ObservableObject, IDisposable
{
    private readonly AppHost _app;
    private bool _disposed = false;

    internal ObservableCollection<ServerViewModel> Servers { get; private set; } = [];
    private readonly SynchronizationContext _context;

    [ObservableProperty] 
    private bool _isAutostart;

    internal ViewModel()
    {
        _context = SynchronizationContext.Current;

        _app = Task.Run(() => new AppHostBuilder().Build()).Result;

        _app.AutostartChanged += AutostartChangedHandler;
        _app.ServerAdded += ServerAddedHandler;
        _app.ServersReady += ServersReadyHandler;

        _app.Run();
    }

    private void AutostartChangedHandler(object sender, bool status)
    {
        IsAutostart = status;
    }

    private void ServerAddedHandler(object _, Server server)
    {
        Servers.Add(ServerViewModel.Create(server, _context));
    }

    private void ServersReadyHandler(object _, List<Server> servers)
    {
        Servers = new ObservableCollection<ServerViewModel>(servers.Select(x => ServerViewModel.Create(x, _context)));
    }

    [RelayCommand]
    private void SetAutostart(bool value)
    {
        _app.AutoStartChange(value);
    }

    [RelayCommand]
    private void Start(int? id = null)
    {
        _app.ServerStart(id);
    }

    [RelayCommand]
    private void Stop(int? id = null)
    {
        _app.ServerStop(id);
    }

    [RelayCommand]
    private void AddServer(ServerType mode)
    {
        _app.ServerAdd(mode);
    }

    [RelayCommand]
    private void RemoveServer(int id)
    {
        _app.ServerRemove(id);

        Servers.Remove(Servers.FirstOrDefault(x => x.Id == id));
    }

    public void Dispose()
    {
        if (_disposed) return;

        _app.AutostartChanged -= AutostartChangedHandler;
        _app.ServerAdded -= ServerAddedHandler;
        _app.ServersReady -= ServersReadyHandler;

        _disposed = true;
    }
}