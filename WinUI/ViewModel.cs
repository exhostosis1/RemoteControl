using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Interfaces;
using MainApp.Servers;
using System;
using System.Collections.ObjectModel;

namespace WinUI;

internal partial class ViewModel: ObservableObject
{
    private readonly AppHost _app = new AppHostBuilder().Build();

    public ViewModel()
    {
        _app.StartAllServers();
        PopulateServers();
    }

    public readonly ObservableCollection<ServerModel> Servers = [];

    private void PopulateServers()
    {
        Servers.Clear();

        foreach (var server in _app.GetServers())
        {
            var model = new ServerModel
            {
                Id = server.Id,
                IsAutostart = server.IsAutostart,
                Name = server.Name,
                Status = server.Status
            };

            switch (server)
            {
                case IWebServer web:
                    model.ListeningUri = web.ListeningUri;
                    break;
                case IBotServer bot:
                    model.ApiUri = bot.ApiUri;
                    model.ApiKey = bot.ApiKey;
                    model.Usernames = bot.Usernames;
                    break;
            }

            Servers.Add(model);
        }
    }

    public bool IsAutostart
    {
        get => _app.GetAutostart();
        set => _app.SetAutostart(value);
    }

    [RelayCommand]
    private void StartAll()
    {
        _app.StartAllServers();
    }

    [RelayCommand]
    private void StopAll()
    {
        _app.StopAllServers();
    }

    [RelayCommand]
    private void Start(Guid id)
    {
        _app.StartServer(id);
    }

    [RelayCommand]
    private void Stop(Guid id)
    {
        _app.StopServer(id);
    }

    [RelayCommand]
    private void Toggle(ServerModel server)
    {
        if (server.Status)
            _app.StopServer(server.Id);
        else
            _app.StartServer(server.Id);
    }

    [RelayCommand]
    private void AddServer(ServerType type)
    {
        _app.AddServer(type);
        PopulateServers();
    }

    [RelayCommand]
    private void RemoveServer(Guid id)
    {
        _app.RemoveServer(id);
        PopulateServers();
    }

    [RelayCommand]
    private void AddFirewallRules()
    {
        _app.AddFirewallRules();
    }

    [RelayCommand]
    private void OpenSite(ServerModel server)
    {
        _app.OpenSite(server.ListeningUri);
    }
}