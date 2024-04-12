﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MainApp;
using Servers;

namespace WinUI;

internal class ViewModel: IDisposable
{
    private readonly AppHost _app;
    private bool _disposed = false;

    internal ObservableCollection<Server> Servers { get; private set; } = [];
    internal bool IsAutostart = false;

    internal ViewModel()
    {
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

    private void ServerAddedHandler(object sender, Server server)
    {
        Servers.Add(server);
    }

    private void ServersReadyHandler(object sender, List<Server> servers)
    {
        Servers = new ObservableCollection<Server>(servers);
    }

    internal void SetAutostart(bool value)
    {
        _app.AutoStartChange(value);
    }

    internal void Start(int? id = null)
    {
        _app.ServerStart(id);
    }

    internal void Stop(int? id = null)
    {
        _app.ServerStop(id);
    }

    internal void AddServer(ServerType mode)
    {
        _app.ServerAdd(mode);
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