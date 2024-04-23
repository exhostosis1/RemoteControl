using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using MainApp;
using MainApp.Servers;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI.ViewModels;

namespace WinUI.Views;

[ObservableObject]
public sealed partial class NotificationView
{
    private readonly App _app;
    public readonly AppHost AppHost = ApphostProvider.AppHost;

    [ObservableProperty]
    private bool _firstServerExists;
    [ObservableProperty]
    private bool _secondServerExists;

    internal ServerViewModel FirstServerViewModel;
    internal ServerViewModel SecondServerViewModel;

    public NotificationView()
    {
        this.InitializeComponent();

        _app = Application.Current as App ?? throw new NullReferenceException();

        AppHost.Servers.CollectionChanged += InitModels;

        InitModels(null, null!);
    }

    private void InitModels(object? _, NotifyCollectionChangedEventArgs __)
    {
        (FirstServerExists, FirstServerViewModel) = InitModel(AppHost.Servers.FirstOrDefault());
        (SecondServerExists, SecondServerViewModel) = InitModel(AppHost.Servers.Skip(1).FirstOrDefault());
    }

    private (bool, ServerViewModel?) InitModel(IServer? server)
    {
        return server == null ? (false, null) : (true, new ServerViewModel(server));
    }

    [RelayCommand]
    void LeftClick()
    {
        var window = _app.MainWindow;

        if(window.Visible)
            window.Hide(true);
        else
        {
            window.Show();
            window.Activate();
        }
    }

    [RelayCommand]
    private void OpenUri(string uri)
    {
        AppHost.OpenSite(uri);
    }

    [RelayCommand]
    private void Start(IServer server)
    {
        server.Start();
    }

    [RelayCommand]
    private void Stop(IServer server)
    {
        server.Stop();
    }

    [RelayCommand]
    private void StartAll()
    {
        foreach (var appHostServer in AppHost.Servers.Where(x => !x.Status))
        {
            appHostServer.Start();
        }
    }

    [RelayCommand]
    private void StopAll()
    {
        foreach (var appHostServer in AppHost.Servers.Where(x => x.Status))
        {
            appHostServer.Stop();
        }
    }

    [RelayCommand]
    private void AddFirewallRules()
    {
        AppHost.AddFirewallRules();
    }

    [RelayCommand]
    void Exit()
    {
        _app.HandleClosedEvents = false;
        TrayIcon.Dispose();
        _app.MainWindow?.Close();
    }
}