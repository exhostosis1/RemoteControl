using System;
using System.Collections.Generic;
using System.Threading;
using ABI.Microsoft.UI.Xaml.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Servers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.ViewModels;

internal sealed partial class ServerViewModel: ObservableObject
{
    private readonly AppHost _appHost = ApphostProvider.AppHost;
    private IServer? _server;

    public IServer? Server
    {
        get => _server;
        set
        {
            _server = value;

            if (_server == null) return;

            Status = _server.Status;
            Type = _server.Config.Type;
            Name = _server.Config.Name;
            ListeningUri = _server.Config.Uri.ToString();
            ApiUri = _server.Config.ApiUri;
            ApiKey = _server.Config.ApiKey;
            Usernames = _server.Config.Usernames;
            StartAutomatically = _server.Config.AutoStart;

            _server.Error += (sender, message) => Error?.Invoke(sender, message);
            InitializeTimer();
        }
    }

    private Timer _timer;
    private readonly SynchronizationContext _context;

    [ObservableProperty] private ServerType _type;
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _listeningUri;
    [ObservableProperty] private string _apiUri;
    [ObservableProperty] private string _apiKey;
    [ObservableProperty] private List<string> _usernames;
    [ObservableProperty] private bool _startAutomatically;

    [ObservableProperty] private bool _status;
    [ObservableProperty] private bool _switchIsEnabled;

    public event EventHandler<string>? Error;

    public ServerViewModel(IServer server)
    {
        _context = SynchronizationContext.Current ?? throw new Exception("No synchronization context found");

        Server = server;
        server.Error += (sender, message) => Error?.Invoke(sender, message);
    }

    private void InitializeTimer()
    {
        _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    private void DoWork(object? state)
    {
        if (_pending || _server is null) return;

        _context.Post((_) =>
        {
            Status = _server.Status;
            SwitchIsEnabled = true;
            _ignoreEvent = false;
        }, null);
    }

    [RelayCommand]
    private void Start()
    {
        _server?.Start();
    }

    [RelayCommand]
    private void Stop()
    {
        _server?.Stop();
    }

    [RelayCommand]
    private void Remove()
    {
        if (_server is null) return;

        _server.Stop();
        _timer.Dispose();
        _appHost.RemoveServer(_server.Id);
    }

    [RelayCommand]
    private void Update()
    {
        var shouldStart = false;
        if (_server == null) return;

        if (_server.Status)
        {
            _server.Stop();
            shouldStart = true;
        }

        var config = new ServerConfig(_server.Config.Type)
        {
            Name = Name,
            AutoStart = StartAutomatically
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

        _server.Config = config;

        if (shouldStart)
            _server.Start();


        _appHost.SaveConfig();
    }

    private bool _ignoreEvent = false;
    private bool _pending = false;

    public void Switch_OnPointerReleased(object sender, RoutedEventArgs _)
    {
        if (_ignoreEvent || _server is null || sender is not ToggleSwitch) return;
        _ignoreEvent = true;
        _pending = true;

        SwitchIsEnabled = false;

        if (_server.Status)
            _server.Stop();
        else
            _server.Start();

        _pending = false;
    }
}