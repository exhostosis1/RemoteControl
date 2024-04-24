using System;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using Windows.Graphics;
using MainApp;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
[ObservableObject]
public sealed partial class MainWindow : Window
{
    private const int Width = 860;
    private const int Height = 480;

    [ObservableProperty] private bool _showError;
    [ObservableProperty] private string _errorMessage;

    private readonly SynchronizationContext _context;

    public MainWindow()
    {
        this.InitializeComponent();

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        var clientSize = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
        var x = clientSize.WorkArea.Width / 2 - Width / 2;
        var y = clientSize.WorkArea.Height / 2 - Height / 2;

        var rect = new RectInt32(x, y, Width, Height);

        appWindow.MoveAndResize(rect);

        _context = SynchronizationContext.Current ?? throw new Exception("No synchronization context found");
    }

    private void OnError(object? _, string message)
    {
        ErrorMessage = message;
        ShowError = true;

        Task.Run(async () =>
        {
            await Task.Delay(5_000);
            
            _context.Post((_) => ShowError = false, null);
        });
    }
}