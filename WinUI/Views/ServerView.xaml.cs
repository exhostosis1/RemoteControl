using WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.Views;

internal sealed partial class ServerView
{
    public ServerViewModel ViewModel { get; set; } = null!;

    public ServerView()
    {
        InitializeComponent();
    }
}