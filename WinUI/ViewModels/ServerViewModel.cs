using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Servers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.ViewModels;

public sealed partial class ServerViewModel: ObservableObject, IDisposable, IAsyncDisposable
{
    private readonly IServer _server;

    private readonly Timer _timer;
    private readonly SynchronizationContext _context;

    [ObservableProperty] private ServerType _type;
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _listeningUri;
    [ObservableProperty] private string _apiUri;
    [ObservableProperty] private string _apiKey;
    [ObservableProperty] private List<string> _usernames;
    [ObservableProperty] private bool _startAutomatically;
    [ObservableProperty] private bool _isSwitchEnabled = true;

    private bool _status;

    public bool Status
    {
        get => _status = _server.Status;
        set
        {
            IsSwitchEnabled = false;
            if (value)
            {
                _server?.Start();
            }
            else
            {
                _server?.Stop();
            }
        }
    }

    private readonly RelayCommand<ServerViewModel> _removeCommand;

    public ServerViewModel(IServer server, RelayCommand<ServerViewModel> removeCommand)
    {
        _context = SynchronizationContext.Current ?? throw new Exception("No synchronization context found");
        _removeCommand = removeCommand;

        _server = server;

        Type = _server.Config.Type;
        Name = _server.Config.Name;
        ListeningUri = _server.Config.Uri.ToString();
        ApiUri = _server.Config.ApiUri;
        ApiKey = _server.Config.ApiKey;
        Usernames = _server.Config.Usernames;
        StartAutomatically = _server.Config.AutoStart;

        _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    private void DoWork(object? state)
    {
        _context.Post((_) =>
        {
            IsSwitchEnabled = true;

            if (_server.Status == _status) return;

            OnPropertyChanged(nameof(Status));
        }, null);
    }

    [RelayCommand]
    private void Start()
    {
        try
        {
            _server?.Start();
        }
        catch (Exception e)
        {
            //Error?.Invoke(this, e.Message);
        }
    }

    [RelayCommand]
    private void Stop()
    {
        _server?.Stop();
    }

    [RelayCommand]
    private void Remove()
    {
        _server.Stop();
        _removeCommand.Execute(this);
        Dispose();
    }

    [RelayCommand]
    private void Update()
    {
        var shouldStart = false;

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
                    //Error?.Invoke(this, e.Message);
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
    }

    public ServerConfig GetConfig() => _server.Config;

    public void Dispose()
    {
        _timer.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _timer.DisposeAsync();
    }
}