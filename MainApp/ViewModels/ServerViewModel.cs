using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Servers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MainApp.ViewModels;

public partial class ServerViewModel: ObservableObject, IDisposable
{
    private readonly Server _server;

    private readonly SynchronizationContext _context;

    [ObservableProperty] private ServerType _type;
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _listeningUri;
    [ObservableProperty] private string _apiUri;
    [ObservableProperty] private string _apiKey;
    [ObservableProperty] private string _usernames;
    [ObservableProperty] private bool _startAutomatically;
    [ObservableProperty] private bool _isSwitchEnabled = true;

    [ObservableProperty] private bool _errorShow;
    [ObservableProperty] private string _errorMessage;
    [ObservableProperty] private bool _expanded;

    private bool _status;

    public bool Status
    {
        get => _status = _server.Status;
        set
        {
            if (value == _status) return;
            
            IsSwitchEnabled = false;
            if (value)
            {
                _status = StartPrivate();
                IsSwitchEnabled = true;
            }
            else
            {
                _status = StopPrivate();
                IsSwitchEnabled = true;
            }

            OnPropertyChanged(nameof(Status));
        }
    }

    private readonly RelayCommand<ServerViewModel> _removeCommand;

    internal ServerViewModel(Server server, RelayCommand<ServerViewModel> removeCommand, bool expanded = false)
    {
        _context = SynchronizationContext.Current ?? throw new Exception("No synchronization context found");
        _removeCommand = removeCommand;

        _server = server;

        Type = _server.Config.Type;
        Name = _server.Config.Name;
        ListeningUri = _server.Config.Uri.ToString();
        ApiUri = _server.Config.ApiUri;
        ApiKey = _server.Config.ApiKey;
        Usernames = _server.Config.UsernamesString;
        StartAutomatically = _server.Config.AutoStart;

        Expanded = expanded;

        _server.PropertyChanged += PropertyChangedHandler;
    }

    private void PropertyChangedHandler(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != "Status") return;

        OnPropertyChanged(nameof(Status));
    }

    private bool StartPrivate()
    {
        var val = false;
        try
        {
            val = _server.Start();
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            ErrorShow = true;
        }

        return val;
    }

    private bool StopPrivate()
    {
        return _server.Stop();
    }

    [RelayCommand]
    private void Start()
    {
        StartPrivate();
    }

    [RelayCommand]
    private void Stop()
    {
        StopPrivate();
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
                    ErrorMessage = e.Message;
                    ErrorShow = true;
                    return;
                }
                break;
            case ServerType.Bot:
                config.ApiUri = ApiUri;
                config.ApiKey = ApiKey;
                config.UsernamesString = Usernames;
                break;
            default:
                break;
        }

        _server.Config = config;

        if (shouldStart)
            _server.Start();
    }

    internal ServerConfig GetConfig() => _server.Config;

    public void Dispose()
    {
        _server.PropertyChanged -= PropertyChangedHandler;
    }

    [RelayCommand]
    private void ErrorClose()
    {
        ErrorShow = false;
    }
}