using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Servers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class NotificationPage : Page
{
    public ViewModel ViewModel { get; set; }
    private readonly Utils _utils = new();
    private MenuFlyoutItemBase[] _baseItems = null;

    public NotificationPage()
    {
        this.InitializeComponent();
    }

    private MenuFlyoutItemBase[] GenerateBaseItems()
    {
        _baseItems =
        [
            new MenuFlyoutItem
            {
                Text = "Start all",
                Command = ViewModel.StartCommand,
                CommandParameter = null
            },
            new MenuFlyoutItem
            {
                Text = "Stop all",
                Command = ViewModel.StopCommand,
                CommandParameter = null
            },
            new MenuFlyoutSeparator(),
            new ToggleMenuFlyoutItem
            {
                Text = "Autostart",
                IsChecked = ViewModel.IsAutostart,
                IsEnabled = true,
                Command = ViewModel.SetAutostartCommand,
                CommandParameter = !ViewModel.IsAutostart
            },
            new MenuFlyoutItem
            {
                Text = "Add firewall rule",
                Command = _utils.AddFirewallRuleCommand,
                CommandParameter = ViewModel.Servers.Where(x => x.Type == ServerType.Web && x.Status).Select(x => new UriBuilder(x.Schema, x.Host, x.Port).Uri)
            },
            new MenuFlyoutItem
            {
                Text = "Exit",
                Command = _utils.ExitCommand
            }
        ];

        return _baseItems;
    }

    private void OnOpening(object sender, object _)
    {
        if (sender is not MenuFlyout flyout) return;

        flyout.Items.Clear();

        foreach (var server in ViewModel.Servers)
        {
            flyout.Items.Add(new MenuFlyoutItem
            {
                Text = server.Name,
                IsEnabled = false
            });

            switch (server.Type)
            {
                case ServerType.Web:
                    flyout.Items.Add(new MenuFlyoutItem
                    {
                        Text = server.Host,
                        IsEnabled = server.Status,
                        Command = _utils.OpenSiteCommand,
                        CommandParameter = server.Host
                    });
                    break;
                case ServerType.Bot:
                    flyout.Items.Add(new MenuFlyoutItem
                    {
                        Text = server.Usernames,
                        IsEnabled = server.Status
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(server.Type));
            }

            flyout.Items.Add(new MenuFlyoutItem
            {
                Text = server.Status ? "Stop" : "Start",
                Command = server.Status ? ViewModel.StopCommand : ViewModel.StartCommand,
                CommandParameter = server
            });
            flyout.Items.Add(new MenuFlyoutSeparator());
        }

        foreach (var item in _baseItems ?? GenerateBaseItems())
        {
            flyout.Items.Add(item);
        }
    }
}