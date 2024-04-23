using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Servers;
using System;
using System.Collections.ObjectModel;

namespace WinUI.Views;

internal sealed partial class CollectionView
{
    public event EventHandler<string>? Error;
    private static readonly AppHost AppHost = ApphostProvider.AppHost;

    public readonly ObservableCollection<IServer> Servers = AppHost.Servers;

    public CollectionView()
    {
        this.InitializeComponent();
        AppHost.Error += OnError;
    }

    [RelayCommand]
    private void Reload()
    {
        AppHost.ReloadServers();
    }

    [RelayCommand]
    private void Add(ServerType type)
    {
        AppHost.AddServer(type);
    }

    [RelayCommand]
    private void AddFirewallRules()
    {
        AppHost.AddFirewallRules();
    }

    private void OnError(object? sender, string message) => Error?.Invoke(sender, message);
}