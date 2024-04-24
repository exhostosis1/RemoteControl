using System;
using MainApp.Servers;
using WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.Views;

internal sealed partial class ServerView
{
    public ServerViewModel ViewModel { get; set; }
 
    private IServer? _server;
    public IServer? Server
    {
        get => _server;
        set
        {
            _server = value;
            if (_server == null) return;

            if (ViewModel != null)
            {
                ViewModel.Error -= ErrorHandler;
                ViewModel.RemoveServer -= RemoveServerHandler;
                ViewModel.UpdateConfig -= UpdateConfigHandler;
            }

            ViewModel = new ServerViewModel(_server);

            ViewModel.Error += ErrorHandler;
            ViewModel.RemoveServer += RemoveServerHandler;
            ViewModel.UpdateConfig += UpdateConfigHandler;
        }
    }

    private void ErrorHandler(object? sender, string message) => Error?.Invoke(sender, message);
    private void RemoveServerHandler(object? sender, IServer server) => RemoveServer?.Invoke(sender, server);
    private void UpdateConfigHandler(object? sender, EventArgs args) => UpdateConfig?.Invoke(sender, args);

    public event EventHandler<string>? Error;
    public event EventHandler<IServer>? RemoveServer;
    public event EventHandler? UpdateConfig;

    public ServerView()
    {
        InitializeComponent();
    }
}