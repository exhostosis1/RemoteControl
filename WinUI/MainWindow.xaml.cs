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
using System.Security.Principal;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using Servers;
using System.Diagnostics;

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
        private readonly ICommand _startCommand;
        private readonly ICommand _stopCommand;
        private readonly ICommand _autostartCommand;
        private readonly ICommand _addFirewallCommand;
        private readonly ICommand _openSiteCommand;

        private readonly MenuFlyoutItemBase[] _baseItems;
        
        public MainWindow()
        {
            this.InitializeComponent();

            _exitCommand = new RelayCommand(Exit);
            _startCommand = new RelayCommand<int?>(Start);
            _stopCommand = new RelayCommand<int?>(Stop);
            _autostartCommand = new RelayCommand<bool>(Autostart);
            _addFirewallCommand = new RelayCommand(AddFirewallRule);
            _openSiteCommand = new RelayCommand<string>(OpenSite);

            _baseItems =
            [
                new MenuFlyoutItem
                {
                    Text = "Start all",
                    Command = _startCommand,
                    CommandParameter = null
                },
                new MenuFlyoutItem
                {
                    Text = "Stop all",
                    Command = _stopCommand,
                    CommandParameter = null
                },
                new MenuFlyoutSeparator(),
                new ToggleMenuFlyoutItem
                {
                    Text = "Autostart",
                    IsChecked = _viewModel.IsAutostart,
                    IsEnabled = true,
                    Command = _autostartCommand,
                    CommandParameter = !_viewModel.IsAutostart
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

        private void Start(int? id)
        {
            _viewModel.Start(id);
        }

        private void Stop(int? id)
        {
            _viewModel.Stop(id);
        }

        static void RunCommand(string command, bool elevated = false)
        {
            var proc = new Process();

            proc.StartInfo.FileName = "cmd";
            proc.StartInfo.Arguments = $"/c \"{command}\"";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = elevated;

            if (elevated) proc.StartInfo.Verb = "runas";

            proc.Start();

            proc.WaitForExit();
        }

        private static void OpenSite(string input)
        {
            var address = input.Replace("&", "^&");

            RunCommand($"start {address}");
        }

        private void AddFirewallRule()
        {
            var uris = _viewModel.Servers.Where(x => x is { Type: ServerType.Web })
                .Select(x => x.Config.Uri);

            var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var translatedValue = sid.Translate(typeof(NTAccount)).Value;

            foreach (var uri in uris)
            {
                var command =
                    $"netsh advfirewall firewall add rule name=\"Remote Control\" dir=in action=allow profile=private localip={uri.Host} localport={uri.Port} protocol=tcp";

                RunCommand(command, true);

                command = $"netsh http add urlacl url={uri} user={translatedValue}";

                RunCommand(command, true);
            }
        }

        private void Autostart(bool value)
        {
            _viewModel.SetAutostart(value);
        }

        private static void Exit()
        {
            Environment.Exit(0);
        }

        private void OnOpening(object sender, object _)
        {
            if (sender is not MenuFlyout flyout) return;

            flyout.Items.Clear();

            foreach (var server in _viewModel.Servers)
            {
                flyout.Items.Add(new MenuFlyoutItem
                {
                    Text = server.Config.Name,
                    IsEnabled = false
                });

                switch (server.Type)
                {
                    case ServerType.Web:
                        flyout.Items.Add(new MenuFlyoutItem
                        {
                            Text = server.Config.Uri.ToString(),
                            IsEnabled = server.Status,
                            Command = _openSiteCommand,
                            CommandParameter = server.Config.Uri.ToString()
                        });
                        break;
                    case ServerType.Bot:
                        flyout.Items.Add(new MenuFlyoutItem
                        {
                            Text = server.Config.UsernamesString,
                            IsEnabled = server.Status
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(server.Type));
                }

                flyout.Items.Add(new MenuFlyoutItem
                {
                    Text = server.Status ? "Stop" : "Start",
                    Command = server.Status ? _stopCommand : _startCommand,
                    CommandParameter = server.Id
                });
                flyout.Items.Add(new MenuFlyoutSeparator());
            }

            foreach (var item in _baseItems)
            {
                flyout.Items.Add(item);
            }
        }
    }
}
