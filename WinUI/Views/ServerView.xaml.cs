using MainApp.Servers;
using System;
using WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.Views;

internal sealed partial class ServerView
{
    public IServer Server
    {
        get => ViewModel.Server;
        set
        {
            ViewModel.Server = value;
            value.Error += (sender, message) => Error?.Invoke(sender, message);
        }
    }

    public event EventHandler<string>? Error;

    public ServerViewModel ViewModel { get; set; } = new();

    public ServerView()
    {
        this.InitializeComponent();
    }
}