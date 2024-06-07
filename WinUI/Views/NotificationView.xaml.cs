using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using MainApp.ViewModels;
using Microsoft.UI.Xaml;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Windows.UI.ViewManagement;

namespace WinUI.Views;

[ObservableObject]
public sealed partial class NotificationView
{
    public ServerCollectionViewModel CollectionViewModel { get; }
    private readonly App _app = Application.Current as App ?? throw new NullReferenceException();

    [ObservableProperty]
    private ServerViewModel? _firstServerViewModel;

    [ObservableProperty]
    private ServerViewModel? _secondServerViewModel;

    [ObservableProperty]
    private bool _isAutorun;

    private readonly Windows.UI.Color _colorBlack = Windows.UI.Color.FromArgb(255, 0, 0, 0);

    private readonly UISettings _settings = new();
    private bool IsDarkMode => _settings.GetColorValue(UIColorType.Background) == _colorBlack;

    private static readonly string DarkIconPath = Path.Combine(AppContext.BaseDirectory, "Icons\\Device.theme-light.ico");
    private static readonly string LightIconPath = Path.Combine(AppContext.BaseDirectory, "Icons\\Device.theme-dark.ico");
    private readonly Icon _darkIcon = new(DarkIconPath);
    private readonly Icon _lightIcon = new(LightIconPath);

    private readonly SynchronizationContext _context;

    public NotificationView()
    {
        this.InitializeComponent();

        _context = SynchronizationContext.Current ?? throw new NullReferenceException();

        CollectionViewModel = _app.Host.ServerCollectionViewModel;

        InitModels();

        _settings.ColorValuesChanged += (_, _) => ApplyTheme();
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        _context.Post((_) =>
        {
            TrayIcon.UpdateIcon(IsDarkMode ? _lightIcon : _darkIcon);
            (_app.MainWindow as MainWindow)?.SetIcon(IsDarkMode ? LightIconPath : DarkIconPath);
        }, null);
    }

    private void InitModels()
    {
        FirstServerViewModel = null;
        SecondServerViewModel = null;

        FirstServerViewModel = CollectionViewModel.Servers.FirstOrDefault();
        SecondServerViewModel = CollectionViewModel.Servers.Skip(1).FirstOrDefault();
    }

    [RelayCommand]
    private void Init()
    {
        InitModels();
    }

    [RelayCommand]
    private void LeftClick()
    {
        var window = _app.MainWindow;

        if(window.Visible)
            window.Hide();
        else
        {
            window.Show();
            window.Activate();
        }
    }

    [RelayCommand]
    private void Exit()
    {
        _app.HandleClosedEvents = false;
        TrayIcon.Dispose();
        _app.MainWindow.Close();
    }
}