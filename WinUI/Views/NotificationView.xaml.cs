using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using MainApp;
using Microsoft.UI.Xaml;
using System;

namespace WinUI.Views;

public sealed partial class NotificationView
{
    private readonly App _app;
    public readonly AppHost AppHost = ApphostProvider.AppHost;

    public NotificationView()
    {
        this.InitializeComponent();

        _app = Application.Current as App ?? throw new NullReferenceException();
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
    private void StartAll()
    {
        AppHost.StartAllServers();
    }

    [RelayCommand]
    private void StopAll()
    {
        AppHost.StopAllServers();
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