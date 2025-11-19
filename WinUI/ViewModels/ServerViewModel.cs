using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Interfaces;
using MainApp.Workers;
using System;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.ViewModels;

public partial class ServerViewModel : ObservableObject, IDisposable
{
    private readonly IWorker _server;

    [ObservableProperty] public partial WorkerType Type { get; set; }
    [ObservableProperty] public partial string Name { get; set; }
    [ObservableProperty] public partial string ListeningUri  {get; set; }
    [ObservableProperty] public partial string ApiUri  {get; set; }
    [ObservableProperty] public partial string ApiKey  {get; set; }
    [ObservableProperty] public partial string Usernames  {get; set; }
    [ObservableProperty] public partial bool StartAutomatically  {get; set; }
    [ObservableProperty] public partial bool IsSwitchEnabled  {get; set; } = true;
                                 
    [ObservableProperty] public partial bool ErrorShow  {get; set; }
    [ObservableProperty] public partial string ErrorMessage  {get; set; } = "";
    [ObservableProperty] public partial bool Expanded  {get; set; }

    private bool _status;

    public bool Status
    {
        get => _status = _server.Status;
        set
        {
            if (value == _status) return;

            IsSwitchEnabled = false;
            _status = value ? StartPrivate() : StopPrivate();

            IsSwitchEnabled = true;

            OnPropertyChanged();
        }
    }

    private readonly RelayCommand<ServerViewModel> _removeCommand;

    internal ServerViewModel(IWorker server, RelayCommand<ServerViewModel> removeCommand, bool expanded = false)
    {
        _removeCommand = removeCommand;

        _server = server;

        Type = _server.Type;
        Name = _server.Name;
        ListeningUri = _server.Uri;
        ApiUri = _server.ApiUri;
        ApiKey = _server.ApiKey;
        Usernames = _server.UsernamesString;
        StartAutomatically = _server.AutoStart;

        Expanded = expanded;

        _server.PropertyChanged += PropertyChangedHandler;
    }

    private void PropertyChangedHandler(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(_server.Status)) return;

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

        var config = new WorkerConfig(_server.Type)
        {
            Name = Name,
            AutoStart = StartAutomatically
        };

        switch (config.Type)
        {
            case WorkerType.Web:
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
            case WorkerType.Bot:
                config.ApiUri = ApiUri;
                config.ApiKey = ApiKey;
                config.UsernamesString = Usernames;
                break;
        }

        _server.Config = config;

        if (shouldStart)
            _server.Start();
    }

    internal WorkerConfig GetConfig() => _server.Config;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _server.PropertyChanged -= PropertyChangedHandler;
    }

    [RelayCommand]
    private void ErrorClose()
    {
        ErrorShow = false;
    }
}