using WinUI.ViewModels;

namespace WinUI.Views;

internal sealed partial class CollectionView
{
    public ServerCollectionViewModel ViewModel { get; } = ServerCollectionViewModelProvider.Get();

    public CollectionView()
    {
        this.InitializeComponent();
    }
}