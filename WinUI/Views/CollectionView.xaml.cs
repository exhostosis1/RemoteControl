using System;
using WinUI.ViewModels;

namespace WinUI.Views;

internal sealed partial class CollectionView
{
    public ServersCollectionViewModel ViewModel { get; set; } = new();

    public event EventHandler<string>? Error; 

    public CollectionView()
    {
        this.InitializeComponent();
    }

    private void OnError(object? sender, string message) => Error?.Invoke(sender, message);
}