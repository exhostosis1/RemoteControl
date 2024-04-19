using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Servers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Usernames = [.. value.Config.Usernames];
            Status = value.Status;
            ListeningUri = value.Config.Uri.ToString();
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
    [ObservableProperty] private string _listeningUri;
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
        var shouldStart = false;

        if (Server.Status)
        {
            Server.Stop();
            shouldStart = true;
        }

        var config = new ServerConfig(Server.Config.Type)
        {
            Name = Name
        };

        switch (config.Type)
        {
            case ServerType.Web:
                try
                {
                    config.Uri = new Uri(ListeningUri);
                }
                catch (Exception e)
                {
                    _appHost.FireErrorEvent(e.Message);
                    return;
                }
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

        if(shouldStart)
            Server.Start();

        _appHost.SaveConfig();
    }

    public void ToggleSwitch_OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (sender is not ToggleSwitch ts) return;

        e.Handled = true;

        ts.IsEnabled = false;
        ts.IsOn = !ts.IsOn;

        if (Status)
        {
            Server.Stop();
        }
        else
        {
            Server.Start();
        }

        ts.IsEnabled = true;
    }
}