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
using Windows.Foundation;
using Windows.Foundation.Collections;
using ABI.System.Windows.Input;

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

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void StartAll(object _, RoutedEventArgs __)
        {
            _viewModel.StartAll();
        }

        private void StopAll(object _, RoutedEventArgs __)
        {
            _viewModel.StopAll();
        }

        private void AddFirewallRule(object _, RoutedEventArgs __)
        {

        }

        private void Autostart(object sender, RoutedEventArgs __)
        {
            _viewModel.SetAutostart((sender as ToggleMenuFlyoutItem)?.IsChecked ?? false);
        }

        private void Exit(object _, RoutedEventArgs __)
        {
            Environment.Exit(0);
        }
    }
}
