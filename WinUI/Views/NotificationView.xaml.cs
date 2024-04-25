using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using MainApp;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Specialized;
using System.Linq;
using WinUI.ViewModels;

namespace WinUI.Views;

[ObservableObject]
public sealed partial class NotificationView
{
    public readonly ServerCollectionViewModel CollectionViewModel = ServerCollectionViewModelProvider.Get();
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

        CollectionViewModel.Servers.CollectionChanged += InitModels;
        InitModels(null, null);
    }

    private void InitModels(object? _, NotifyCollectionChangedEventArgs? __)
    {
        FirstServerViewModel = CollectionViewModel.Servers.FirstOrDefault();
        SecondServerViewModel = CollectionViewModel.Servers.Skip(1).FirstOrDefault();
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
    private void OpenUri(string uri)
    {
        AppHost.OpenSite(uri);
    }

    [RelayCommand]
    private void Exit()
    {
        _app.HandleClosedEvents = false;
        TrayIcon.Dispose();
        _app.MainWindow?.Close();
    }
}