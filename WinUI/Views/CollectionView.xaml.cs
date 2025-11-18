using Microsoft.UI.Xaml;
using System;
using WinUI.ViewModels;

namespace WinUI.Views;

internal sealed partial class CollectionView
{
    public ServerCollectionViewModel ViewModel { get; } = new ServerCollectionViewModel((Application.Current as App)?.Host ?? throw new NullReferenceException());

    public CollectionView()
    {
        this.InitializeComponent();
    }
}