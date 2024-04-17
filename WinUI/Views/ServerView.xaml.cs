using CommunityToolkit.Mvvm.Input;
using MainApp.Servers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.Views;

internal sealed partial class ServerView
{
    public Server ViewModel { get; set; }

    public ServerView()
    {
        this.InitializeComponent();
    }

    [RelayCommand]
    private void Update()
    {

    }

    [RelayCommand]
    private void Remove()
    {

    }
}