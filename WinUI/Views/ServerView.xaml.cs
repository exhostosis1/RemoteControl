using CommunityToolkit.Mvvm.ComponentModel;
using MainApp.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.Views;

[ObservableObject]
internal sealed partial class ServerView
{
    [ObservableProperty]
    private ServerViewModel _viewModel;

    public ServerView()
    {
        InitializeComponent();
    }
}