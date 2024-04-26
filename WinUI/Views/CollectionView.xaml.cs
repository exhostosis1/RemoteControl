using MainApp.ViewModels;
using System;
using Microsoft.UI.Xaml;

namespace WinUI.Views;

internal sealed partial class CollectionView
{
    public ServerCollectionViewModel ViewModel { get; } =
        (Application.Current as App)?.Host.ServerCollectionViewModel ?? throw new NullReferenceException();

    public CollectionView()
    {
        this.InitializeComponent();
    }
}