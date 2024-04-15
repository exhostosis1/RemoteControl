namespace WinUI;

public static class ViewModelProvider
{
    private static ViewModel? _viewModel = null;

    public static ViewModel GetViewModel()
    {
        return _viewModel ??= new ViewModel();
    }
}