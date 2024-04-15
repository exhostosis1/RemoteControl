using Microsoft.UI.Xaml.Controls;
using Servers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace WinUI;

public class CustomMenuFlyout: MenuFlyout
{
    private readonly ViewModel _viewModel = ViewModelProvider.GetViewModel();
    private MenuFlyoutItemBase[]? _baseItems = null;

    private readonly SynchronizationContext _context;

    public CustomMenuFlyout()
    {
        PopulateContextMenu();
        
        _viewModel.App.Servers.CollectionChanged += (_, args) =>
        {
            PopulateContextMenu();

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Server argsNewItem in args.NewItems ?? Array.Empty<object>())
                    {
                        argsNewItem.PropertyChanged += ProcessServerStatusChanged;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Server argsNewItem in args.OldItems ?? Array.Empty<object>())
                    {
                        argsNewItem.PropertyChanged -= ProcessServerStatusChanged;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                default:
                    break;
            }
        };

        _context = SynchronizationContext.Current ?? throw new Exception("No context found");
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

            server.PropertyChanged += ProcessServerStatusChanged;
        }

        foreach (var item in _baseItems ?? GenerateBaseItems())
        {
            yield return item;
        }
    }

    private void ProcessServerStatusChanged(object? sender, PropertyChangedEventArgs args)
    {
        _context.Post(_ => PopulateContextMenu(), null);
    }
}