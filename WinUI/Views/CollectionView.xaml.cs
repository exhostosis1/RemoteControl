using WinUI.ViewModels;

namespace WinUI.Views;

internal sealed partial class CollectionView
{
    public ServersCollectionViewModel ViewModel { get; set; } = new();

    public CollectionView()
    {
        this.InitializeComponent();
    }
}