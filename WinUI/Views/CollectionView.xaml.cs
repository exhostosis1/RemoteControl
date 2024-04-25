using System;
using System.Collections.Generic;
using System.Linq;
using MainApp.Servers;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.Win32;
using WinUI.ViewModels;

namespace WinUI.Views;

internal sealed partial class CollectionView
{
    public event EventHandler<string>? Error;
    private readonly ILogger _logger = (Application.Current as App)?.Host.Logger ?? throw new NullReferenceException();

    public ServerCollectionViewModel ViewModel { get; } = ServerCollectionViewModelProvider.Get();

    public CollectionView()
    {
        this.InitializeComponent();

        ViewModel.Error += (sender, message) => Error?.Invoke(sender, message);
        SetSystemEvents();
    }

    private void OnError(object? sender, string message) => Error?.Invoke(sender, message);

    private void ServerView_OnRemoveServer(object? _, IServer server)
    {
        ViewModel.RemoveServer(server);
    }

    private void ServerView_OnUpdateConfig(object? _, EventArgs __)
    {
        ViewModel.UpdateConfigCommand.Execute(null);
    }

    private void SetSystemEvents()
    {

        SystemEvents.SessionSwitch += SessionSwitchHandler;
    }

    private List<IServer> _runningServers = [];

    private void SessionSwitchHandler(object sender, SessionSwitchEventArgs args)
    {
        switch (args.Reason)
        {
            case SessionSwitchReason.SessionLock:
            {
                _logger.LogInformation("Stopping servers due to logout");

                _runningServers = ViewModel.Servers.Where(x => x.Status).ToList();
                _runningServers.ForEach(x => x.Stop());

                break;
            }
            case SessionSwitchReason.SessionUnlock:
            {
                _logger.LogInformation("Restoring servers");

                _runningServers.ForEach(x => x.Start());
                break;
            }
            case SessionSwitchReason.ConsoleConnect:
            case SessionSwitchReason.ConsoleDisconnect:
            case SessionSwitchReason.RemoteConnect:
            case SessionSwitchReason.RemoteDisconnect:
            case SessionSwitchReason.SessionLogon:
            case SessionSwitchReason.SessionLogoff:
            case SessionSwitchReason.SessionRemoteControl:
            default:
                break;

        }
    }
}