using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Servers;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;

namespace WinUI.ViewModels;

internal partial class ServerViewModel : ObservableObject
{
    private IServer _server;
    public IServer Server
    {
        get => _server;
        set
        {
            Type = value.Config.Type;
            Name = value.Config.Name;
            ApiKey = value.Config.ApiKey;
            Usernames = value.Config.Usernames.ToList();
            Status = value.Status;
            ListeningUri = value.Config.Uri;
            ApiUri = value.Config.ApiUri;
            StartAutomatically = value.Config.AutoStart;

            value.PropertyChanged += (sender, args) =>
            {
                if (sender is not IServer s || args.PropertyName != nameof(Server.Status)) return;

                Status = s.Status;
            };

            _server = value;
        }
    }

    private readonly AppHost _appHost = ApphostProvider.AppHost;


    [ObservableProperty] private bool _status;
    [ObservableProperty] private ServerType _type;
    [ObservableProperty] private string _name;
    [ObservableProperty] private Uri _listeningUri;
    [ObservableProperty] private string _apiUri;
    [ObservableProperty] private string _apiKey;
    [ObservableProperty] private List<string> _usernames;
    [ObservableProperty] private bool _startAutomatically;

    public ServerViewModel()
    {
    }

    [RelayCommand]
    private void Start()
    {
        Server.Start();
    }

    [RelayCommand]
    private void Stop()
    {
        Server.Stop();
    }

    [RelayCommand]
    private void Remove()
    {
        _appHost.RemoveServer(_server.Id);
    }

    [RelayCommand]
    private void Update()
    {
        Server.Stop();

        var config = new ServerConfig(Server.Config.Type)
        {
            Name = Name
        };

        switch (config.Type)
        {
            case ServerType.Web:
                config.Uri = ListeningUri;
                break;
            case ServerType.Bot:
                config.ApiUri = ApiUri;
                config.ApiKey = ApiKey;
                config.Usernames = Usernames;
                break;
            default:
                break;
        }

        Server.Config = config;

        Server.Start();

        _appHost.SaveConfig();
    }

    public void ToggleSwitch_OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (sender is not ToggleSwitch ts) return;

        e.Handled = true;

        ts.IsOn = Status;

        if (Status)
            Server.Stop();
        else
            Server.Start();
    }
}