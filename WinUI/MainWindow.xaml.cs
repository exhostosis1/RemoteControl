using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CommunityToolkit.Mvvm.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly ViewModel _viewModel = new();

        private readonly ICommand _exitCommand;
        private readonly ICommand _startAllCommand;
        private readonly ICommand _stopAllCommand;
        private readonly ICommand _autostartCommand;
        private readonly ICommand _addFirewallCommand;

        private FlyoutBase Flyout = new MenuFlyout();

        public MainWindow()
        {
            this.InitializeComponent();

            _exitCommand = new RelayCommand(Exit, () => true);
            _startAllCommand = new RelayCommand(StartAll, () => true);
            _stopAllCommand = new RelayCommand(StopAll, () => true);
            _autostartCommand = new RelayCommand(Autostart, () => true);
            _addFirewallCommand = new RelayCommand(AddFirewallRule, () => true);
        }

        private void StartAll()
        {
            _viewModel.StartAll();
        }

        private void StopAll()
        {
            _viewModel.StopAll();
        }

        private void AddFirewallRule()
        {

        }

        private void Autostart()
        {
            _viewModel.SetAutostart(!_viewModel.IsAutostart);
        }

        private void Exit()
        {
            Environment.Exit(0);
        }

        private IList<MenuFlyoutItemBase> GetFlyoutItems()
        {

            //< MenuFlyoutSeparator ></ MenuFlyoutSeparator >
            //    < MenuFlyoutItem Text = "Start all" Click = "StartAll" />
            //    < MenuFlyoutItem Text = "Stop all" Click = "StopAll" />
            //    < MenuFlyoutSeparator ></ MenuFlyoutSeparator >
            //    < ToggleMenuFlyoutItem Text = "Autostart" Click = "Autostart" IsChecked = "{x:Bind _viewModel.IsAutostart}" />
            //    < MenuFlyoutItem Text = "Add firewall rule" Click = "AddFirewallRule" />
            //    < MenuFlyoutItem Text = "Exit" Click = "Exit" />

            return
            [
                new MenuFlyoutSeparator(),
                new MenuFlyoutItem
                {
                    Text = "Start all",
                    Command = _startAllCommand
                },
                new MenuFlyoutItem
                {
                    Text = "Stop all",
                    Command = _stopAllCommand
                },
                new MenuFlyoutSeparator(),
                new ToggleMenuFlyoutItem
                {
                    Text = "Autostart",
                    Command = _autostartCommand,
                    IsChecked = _viewModel.IsAutostart
                },
                new MenuFlyoutItem
                {
                    Text = "Add firewall rule",
                    Command = _addFirewallCommand
                },
                new MenuFlyoutItem
                {
                    Text = "Exit",
                    Command = _exitCommand
                }

            ];
        }
    }
}
