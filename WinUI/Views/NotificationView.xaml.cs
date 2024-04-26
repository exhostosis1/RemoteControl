using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using MainApp.ViewModels;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;
using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using System.Reflection.Metadata;
using System.Drawing.Drawing2D;
using System.Threading;

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

    private readonly Icon _darkIcon = new(Path.Combine(AppContext.BaseDirectory, "Icons\\Device.theme-light.ico"));
    private readonly Icon _lightIcon = new(Path.Combine(AppContext.BaseDirectory, "Icons\\Device.theme-dark.ico"));

    private readonly SynchronizationContext _context;

    public NotificationView()
    {
        this.InitializeComponent();

        _context = SynchronizationContext.Current ?? throw new NullReferenceException();

        CollectionViewModel = _app.Host.ServerCollectionViewModel;

        InitModels(null, null);

        _settings.ColorValuesChanged += (_, _) => ApplyTheme();
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        _context.Post((_) => TrayIcon.UpdateIcon(IsDarkMode ? _lightIcon : _darkIcon), null);
    }

    private void InitModels(object? _, NotifyCollectionChangedEventArgs? __)
    {
        FirstServerViewModel = null;
        SecondServerViewModel = null;

        FirstServerViewModel = CollectionViewModel.Servers.FirstOrDefault();
        SecondServerViewModel = CollectionViewModel.Servers.Skip(1).FirstOrDefault();
    }

    [RelayCommand]
    private void Init()
    {
        InitModels(null, null);
    }

    [RelayCommand]
    private void LeftClick()
    {
        var window = _app.MainWindow;

        if(window.Visible)
            window.Hide(true);
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
        _app.MainWindow?.Close();
    }
}