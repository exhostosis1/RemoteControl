using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Servers;

namespace WinUI.ViewModels;

internal partial class ServerViewModel : ObservableObject
{
    private readonly AppHost _appHost = ApphostProvider.AppHost;

    public string Name { get; set; }
    public ServerType Type { get; set; }
    public Guid Id { get; set; }

    [ObservableProperty] private bool _status;

    public bool IsAutostart { get; set; }
    public Uri ListeningUri { get; set; }
    public Uri ApiUri { get; set; }
    public string ApiKey { get; set; }
    public IReadOnlyList<string> Usernames { get; set; }

    [RelayCommand]
    private void Start()
    {
        _appHost.StartServer(Id);
    }

    [RelayCommand]
    private void Stop()
    {
        _appHost.StopServer(Id);
    }

    [RelayCommand]
    private void Update()
    {
        var a = 5;
    }

    [RelayCommand]
    private void Remove()
    {
        _appHost.RemoveServer(Id);
    }
}