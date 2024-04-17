using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using MainApp;
using Microsoft.UI.Xaml;

namespace WinUI.Views;

public sealed partial class NotificationView
{
    private readonly App _app;
    private readonly AppHost _appHost = ApphostProvider.AppHost;

    public bool IsAutostart
    {
        get => _appHost.GetAutostart();
        set => _appHost.SetAutostart(value);
    }

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
        _appHost.StartAllServers();
    }

    [RelayCommand]
    private void StopAll()
    {
        _appHost.StopAllServers();
    }

    [RelayCommand]
    private void AddFirewallRules()
    {
        _appHost.AddFirewallRules();
    }

    [RelayCommand]
    void Exit()
    {
        _app.HandleClosedEvents = false;
        TrayIcon.Dispose();
        _app.MainWindow?.Close();
    }
}