using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
{
    private const int Width = 860;
    private const int Height = 480;

    private readonly AppWindow _appWindow;

    public void SetIcon(string icon)
    {
        _appWindow.SetIcon(icon);
    }

    public MainWindow()
    {
        this.InitializeComponent();

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);

        var clientSize = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
        var x = clientSize.WorkArea.Width / 2 - Width / 2;
        var y = clientSize.WorkArea.Height / 2 - Height / 2;

        var rect = new RectInt32(x, y, Width, Height);

        _appWindow.MoveAndResize(rect);
    }
}