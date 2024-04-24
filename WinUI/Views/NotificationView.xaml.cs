using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using MainApp;
using MainApp.Servers;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
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

    public bool IsAutorun
    {
        get => _app.Host.GetAutorun();
        set
        {
            _app.Host.SetAutorun(value);
            OnPropertyChanged(nameof(IsAutorun));
        }
    }

    public NotificationView()
    {
        this.InitializeComponent();

        _viewModel.Servers.CollectionChanged += InitModels;

        InitModels(null, null!);
    }

    private void InitModels(object? _, NotifyCollectionChangedEventArgs __)
    {
        FirstServerViewModel = InitModel(_viewModel.Servers.FirstOrDefault());
        SecondServerViewModel = InitModel(_viewModel.Servers.Skip(1).FirstOrDefault());
    }

    private ServerViewModel? InitModel(IServer? server)
    {
        return server == null ? null : new ServerViewModel(server);
    }

    [RelayCommand]
    void LeftClick()
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
    private void FirstStart()
    {
        FirstServerViewModel?.Server?.Start();
    }

    [RelayCommand]
    private void FirstStop()
    {
        FirstServerViewModel?.Server?.Stop();
    }

    [RelayCommand]
    private void SecondStart()
    {
        SecondServerViewModel?.Server?.Start();
    }

    [RelayCommand]
    private void SecondStop()
    {
        SecondServerViewModel?.Server?.Stop();
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
    void Exit()
    {
        _app.HandleClosedEvents = false;
        TrayIcon.Dispose();
        _app.MainWindow?.Close();
    }
}