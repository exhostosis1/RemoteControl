using MainApp.ViewModels;
using Microsoft.UI.Xaml;
using System;

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