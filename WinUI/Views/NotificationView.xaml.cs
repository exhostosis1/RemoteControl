using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using MainApp.ViewModels;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Specialized;
using System.Linq;

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

    public NotificationView()
    {
        this.InitializeComponent();

        CollectionViewModel = _app.Host.ServerCollectionViewModel;

        InitModels(null, null);
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