namespace WinUI;

internal class ViewModelProvider
{
    private static ViewModel? _viewModel = null;

    public ViewModel ServersViewModel => _viewModel ??= new ViewModel();
}