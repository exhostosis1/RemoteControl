using System;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using Microsoft.UI.Xaml;

namespace WinUI.Views;

public sealed partial class NotificationView
{
    private readonly App _app;

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
    void Exit()
    {
        _app.HandleClosedEvents = false;
        TrayIcon.Dispose();
        _app.MainWindow?.Close();
    }
}