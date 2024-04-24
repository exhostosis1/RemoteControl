using WinUI.ViewModels;

namespace WinUI;

internal static class ServerCollectionViewModelProvider
{
    private static ServerCollectionViewModel? _viewModel;

    public static ServerCollectionViewModel Get() => _viewModel ??= new ServerCollectionViewModel();
}