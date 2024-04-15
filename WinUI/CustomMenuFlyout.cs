using Microsoft.UI.Xaml.Controls;
using Servers;
using System;
using System.Collections.Generic;

namespace WinUI;

public class CustomMenuFlyout: MenuFlyout
{
    private readonly ViewModel _viewModel = ViewModelProvider.GetViewModel();
    private MenuFlyoutItemBase[]? _baseItems = null;

    public CustomMenuFlyout()
    {
        PopulateContextMenu();

        _viewModel.App.Servers.CollectionChanged += (_, _) =>
        {
            PopulateContextMenu();
        };
    }

    private void PopulateContextMenu()
    {
        Items.Clear();

        foreach (var item in GenerateContextMenu())
        {
            Items.Add(item);
        }
    }

    private MenuFlyoutItemBase[] GenerateBaseItems()
    {
        _baseItems =
        [
            new MenuFlyoutItem
            {
                Text = "Start all",
                Command = _viewModel.StartAllCommand
            },
            new MenuFlyoutItem
            {
                Text = "Stop all",
                Command = _viewModel.StopAllCommand
            },
            new MenuFlyoutSeparator(),
            new ToggleMenuFlyoutItem
            {
                Text = "Autostart",
                IsChecked = _viewModel.App.IsAutostart,
                IsEnabled = true,
                Command = _viewModel.SetAutostartCommand,
                CommandParameter = !_viewModel.App.IsAutostart
            },
            new MenuFlyoutItem
            {
                Text = "Add firewall rule",
                Command = _viewModel.AddFirewallRulesCommand
            },
            new MenuFlyoutItem
            {
                Text = "Exit",
                Command = _viewModel.ExitCommand
            }
        ];

        return _baseItems;
    }

    private IEnumerable<MenuFlyoutItemBase> GenerateContextMenu()
    {
        foreach (var server in _viewModel.App.Servers)
        {
            yield return new MenuFlyoutItem
            {
                Text = server.Config.Name,
                IsEnabled = false
            };

            yield return server.Type switch
            {
                ServerType.Web => new MenuFlyoutItem
                {
                    Text = server.Config.Host,
                    IsEnabled = server.Status,
                    Command = _viewModel.OpenSiteCommand,
                    CommandParameter = server
                },
                ServerType.Bot => new MenuFlyoutItem
                {
                    Text = server.Config.UsernamesString,
                    IsEnabled = server.Status
                },
                _ => throw new ArgumentOutOfRangeException(nameof(server.Type))
            };

            yield return new MenuFlyoutItem
            {
                Text = server.Status ? "Stop" : "Start",
                Command = server.Status ? _viewModel.StopCommand : _viewModel.StartCommand,
                CommandParameter = server
            };
            yield return new MenuFlyoutSeparator();
        }

        foreach (var item in _baseItems ?? GenerateBaseItems())
        {
            yield return item;
        }
    }
}