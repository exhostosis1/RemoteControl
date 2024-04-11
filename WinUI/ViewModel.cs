using System;
using System.Collections.Generic;
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

    internal List<Server> Servers { get; private set; } = [];

    internal string FirstHost => Servers.FirstOrDefault()?.Config.Host ?? "no server";

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

    }

    private void ServerAddedHandler(object sender, Server server)
    {

    }

    private void ServersReadyHandler(object sender, List<Server> servers)
    {
        Servers = servers;
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