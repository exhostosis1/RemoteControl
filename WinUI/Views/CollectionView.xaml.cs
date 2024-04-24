using System;
using MainApp.Servers;
using WinUI.ViewModels;

namespace WinUI.Views;

internal sealed partial class CollectionView
{
    public event EventHandler<string>? Error;

    public ServerCollectionViewModel ViewModel { get; } = ServerCollectionViewModelProvider.Get();

    public CollectionView()
    {
        this.InitializeComponent();

        ViewModel.Error += (sender, message) => Error?.Invoke(sender, message);
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
}