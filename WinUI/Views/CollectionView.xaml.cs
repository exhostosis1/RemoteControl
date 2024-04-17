using CommunityToolkit.Mvvm.Input;
using MainApp;
using MainApp.Servers;

namespace WinUI.Views;

internal sealed partial class CollectionView
{
    public readonly AppHost AppHost = ApphostProvider.AppHost;

    public CollectionView()
    {
        this.InitializeComponent();
    }

    [RelayCommand]
    private void ReloadServers()
    {
        AppHost.ReloadServers();
    }

    [RelayCommand]
    private void AddServer(ServerType type)
    {
        AppHost.AddServer(type);
    }
}