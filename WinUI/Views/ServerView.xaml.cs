using MainApp.Servers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.Views;

internal sealed partial class ServerView
{
    public IServer Server { 
        get => ViewModel.Server;
        set => ViewModel.Server = value;
    }

    public ServerViewModel ViewModel { get; set; } = new();

    public ServerView()
    {
        this.InitializeComponent();
    }
}