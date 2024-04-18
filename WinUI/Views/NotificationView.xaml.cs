using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using MainApp;
using MainApp.Servers;
using Microsoft.UI.Xaml;
using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUI.Views;

[ObservableObject]
public sealed partial class NotificationView
{
    private readonly App _app;
    public readonly AppHost AppHost = ApphostProvider.AppHost;

    [ObservableProperty]
    private IServer? _firstServer;
    [ObservableProperty]
    private IServer? _secondServer;

    public NotificationView()
    {
        this.InitializeComponent();

        _app = Application.Current as App ?? throw new NullReferenceException();

        FirstServer = AppHost.Servers.FirstOrDefault();
        SecondServer = AppHost.Servers.Skip(1).FirstOrDefault();

        AppHost.Servers.CollectionChanged += (_, _) =>
        {
            FirstServer = AppHost.Servers.FirstOrDefault();
            SecondServer = AppHost.Servers.Skip(1).FirstOrDefault();
        };
    }

    [RelayCommand]
    void LeftClick()
    {
        var window = _app.MainWindow;

        if(window.Visible)
            window.Hide();
        else
            window.Show();
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
        foreach (var appHostServer in AppHost.Servers)
        {
            appHostServer.Start();
        }
    }

    [RelayCommand]
    private void StopAll()
    {
        foreach (var appHostServer in AppHost.Servers)
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