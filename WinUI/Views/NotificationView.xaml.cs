using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using MainApp;
using MainApp.Servers;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Specialized;
using System.Linq;
using WinUI.ViewModels;

namespace WinUI.Views;

[ObservableObject]
public sealed partial class NotificationView
{
    private readonly ServerCollectionViewModel _viewModel = ServerCollectionViewModelProvider.Get();
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

        _viewModel.Servers.CollectionChanged += InitModels;
        InitModels(null, null);
    }

    private void InitModels(object? _, NotifyCollectionChangedEventArgs? __)
    {
        FirstServerViewModel?.Dispose();
        SecondServerViewModel?.Dispose();

        FirstServerViewModel = InitModel(_viewModel.Servers.FirstOrDefault());
        SecondServerViewModel = InitModel(_viewModel.Servers.Skip(1).FirstOrDefault());
    }

    private ServerViewModel? InitModel(IServer? server)
    {
        return server == null ? null : new ServerViewModel(server);
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
    private void StartAll()
    {
        foreach (var appHostServer in _viewModel.Servers.Where(x => !x.Status))
        {
            appHostServer.Start();
        }
    }

    [RelayCommand]
    private void StopAll()
    {
        foreach (var appHostServer in _viewModel.Servers.Where(x => x.Status))
        {
            appHostServer.Stop();
        }
    }

    [RelayCommand]
    private void AddFirewallRules()
    {
        AppHost.AddFirewallRules(_viewModel.Servers.Where(x => x.Status).Select(x => x.Config.Uri));
    }

    [RelayCommand]
    private void Exit()
    {
        _app.HandleClosedEvents = false;
        TrayIcon.Dispose();
        _app.MainWindow?.Close();
    }

    [RelayCommand]
    private void Opening()
    {
        IsAutorun = _app.Host.GetAutorun();
    }

    [RelayCommand]
    private void SetAutorun()
    {
        _app.Host.SetAutorun(!IsAutorun);
    }
}