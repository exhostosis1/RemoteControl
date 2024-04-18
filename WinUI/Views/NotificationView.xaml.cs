using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using MainApp;
using Microsoft.UI.Xaml;
using System;
using System.Linq;
using MainApp.Servers;
using Microsoft.UI.Xaml.Controls;

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
    private void OpenUri(Uri uri)
    {
        AppHost.OpenSite(uri);
    }

    [RelayCommand]
    void PopulateServers()
    {
        var menuItems = (TrayIcon.ContextFlyout as MenuFlyout)?.Items;

        if (menuItems == null) return;

        var oldItems = menuItems.Where(x => x.AccessKey != "generated").ToList();

        menuItems.Clear();

        foreach (var server in AppHost.Servers)
        {
            menuItems.Add(new MenuFlyoutItem
            {
                AccessKey = "generated",
                Text = server.Config.Name
            });

            switch (server.Config.Type)
            {
                case ServerType.Web:
                    menuItems.Add(new MenuFlyoutItem
                    {
                        AccessKey = "generated",
                        Text = server.Config.Uri.ToString(),
                        IsEnabled = server.Status,
                        Command = OpenUriCommand,
                        CommandParameter = server.Config.Uri
                    });
                    break;
                case ServerType.Bot:
                    menuItems.Add(new MenuFlyoutItem
                    {
                        AccessKey = "generated",
                        Text = server.Config.UsernamesString,
                        IsEnabled = server.Status
                    });
                    break;
                default:
                    break;
            }

            menuItems.Add(new MenuFlyoutItem
            {
                Text = server.Status ? "Stop" : "Start",
                AccessKey = "generated",
                Command = server.Status ? StopCommand : StartCommand,
                CommandParameter = server
            });

            menuItems.Add(new MenuFlyoutSeparator
            {
                AccessKey = "generated"
            });
        }

        foreach (var oldItem in oldItems)
        {
            menuItems.Add(oldItem);
        }
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

    private void FlyoutBase_OnOpening(object? sender, object e)
    {
        var a = 5;
    }
}