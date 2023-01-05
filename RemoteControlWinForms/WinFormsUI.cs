using Microsoft.Win32;
using Shared;
using Shared.Config;
using Shared.ControlProcessor;
using Shared.UI;

namespace RemoteControlWinForms;

// ReSharper disable once InconsistentNaming
public partial class WinFormsUI : Form, IUserInterface
{
    private readonly ToolStripItem[] _commonMenuItems;

    public event IntEventHandler? StartEvent;
    public event IntEventHandler? StopEvent;
    public event EmptyEventHandler? CloseEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event ConfigEventHandler? ConfigChangedEvent;
    public event StringEventHandler? ProcessorAddedEvent;

    private const int GroupMargin = 6;
    private bool IsAutostart { get; set; }
    private static bool IsDarkMode => Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1) as int? == 1;
    private List<IControlProcessor> _model = new();

    public WinFormsUI()
    {
        InitializeComponent();

        var icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            IsDarkMode ? "Device.theme-light.ico" : "Device.theme-dark.ico"));

        TaskbarNotifyIcon.Icon = icon;
        Icon = icon;

        _commonMenuItems = new ToolStripItem[]
        {
            new ToolStripMenuItem("Start all", null, StartAllToolStripMenuItem_Click),
            new ToolStripMenuItem("Stop all", null, StopAllToolStripMenuItem_Click),
            AutostartStripMenuItem,
            AddFirewallRuleToolStripMenuItem,
            CloseToolStripMenuItem
        };

        AutostartStripMenuItem.Checked = IsAutostart;

        Width = 550;
        Height = 40;

        MainContextMenuStrip.Items.AddRange(_commonMenuItems);
    }

    public void RunUI()
    {
        Application.EnableVisualStyles();
        Application.Run(this);
    }

    public void SetViewModel(List<IControlProcessor> model)
    {
        _model = model;
        RedrawWindow();
    }

    public void SetAutostartValue(bool value) => IsAutostart = value;

    public void ShowError(string message)
    {
        MessageBox.Show(message, @"Error", MessageBoxButtons.OK);
    }

    private void SetContextMenu()
    {
        MainContextMenuStrip.Items.Clear();

        for (var i = 0; i < _model.Count; i++)
        {
            var processor = _model[i];

            MainContextMenuStrip.Items.Add(new ToolStripMenuItem(processor.CurrentConfig.Name) { Enabled = false });

            if (processor.Working)
            {
                var item = new ToolStripMenuItem();
                
                switch (processor)
                {
                    case IServerProcessor s:
                        item.Text = s.CurrentConfig.Uri?.ToString();
                        item.Click += IpToolStripMenuItem_Click;
                        break;
                    case IBotProcessor b:
                        item.Text = b.CurrentConfig.UsernamesString;
                        break;
                    default:
                        continue;
                }

                MainContextMenuStrip.Items.Add(item);
            }
            else
            {
                MainContextMenuStrip.Items.Add(new ToolStripMenuItem("Stopped") { Enabled = false });
            }

            var startstopitem = new ToolStripMenuItemWithIndex
            {
                Text = processor.Working ? @"Stop" : @"Start",
                Index = i
            };

            startstopitem.Click += processor.Working ? StopToolStripMenuItem_Click : StartToolStripMenuItem_Click;

            MainContextMenuStrip.Items.Add(startstopitem);
            MainContextMenuStrip.Items.Add(new ToolStripSeparator());
        }

        AutostartStripMenuItem.Checked = IsAutostart;

        MainContextMenuStrip.Items.AddRange(_commonMenuItems);
    }

    private void CloseToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        CloseEvent?.Invoke();
    }

    private static void IpToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        var address = (sender as ToolStripMenuItem)?.Text?.Replace("&", "^&") ?? string.Empty;

        Utils.RunWindowsCommand($"start {address}", out _, out _);
    }

    private void StartToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItemWithIndex o)
            StartEvent?.Invoke(o.Index);
    }

    private void StopToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItemWithIndex o)
            StopEvent?.Invoke(o.Index);
    }
    private void StartAllToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        StartEvent?.Invoke(null);
    }

    private void StopAllToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        StopEvent?.Invoke(null);
    }

    private void ConfigForm_Shown(object sender, EventArgs e)
    {
        Hide();
    }

    private void AutostartStripMenuItem_Click(object sender, EventArgs e)
    {
        AutostartChangedEvent?.Invoke(!IsAutostart);
    }

    private void AddFirewallRuleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        foreach (var uri in _model.Where(x => x is IServerProcessor))
        {
            var command =
                $"netsh advfirewall firewall add rule name=\"Remote Control\" dir=in action=allow profile=private localip={(uri.CurrentConfig as ServerConfig)?.Host} localport={(uri.CurrentConfig as ServerConfig)?.Port} protocol=tcp";

            Utils.RunWindowsCommandAsAdmin(command);
        }
    }

    private void RedrawWindow()
    {
        var height = GroupMargin;
        Controls.Clear();

        for (var i = 0; i < _model.Count; i++)
        {
            var processor = _model[i];
            MyPanel group;

            switch (processor)
            {
                case IServerProcessor s:
                    group = new ServerPanel(s, i)
                    {
                        Top = height
                    };
                    break;
                case IBotProcessor b:
                    group = new BotPanel(b, i)
                    {
                        Top = height
                    };
                    break;
                default:
                    continue;
            }

            group.StartButtonClicked += x => StartEvent?.Invoke(x);
            group.StopButtonClicked += x => StopEvent?.Invoke(x);
            group.UpdateButtonClicked += (index, config) => ConfigChangedEvent?.Invoke(index, config);
            
            Controls.Add(group);
            height += group.Height + GroupMargin;
        }

        AddServerButton.Top = height;
        AddBotButton.Top = height;

        Controls.Add(AddServerButton);
        Controls.Add(AddBotButton);
        
        Height = height + AddServerButton.Height + GroupMargin + 40;
    }

    private void TaskbarNotify_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            RedrawWindow();
            Show();
        }
    }

    private void WinFormsUI_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    private void taskbarNotify_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            SetContextMenu();
        }
    }

    private void AddServerButton_Click(object sender, EventArgs e)
    {
        ProcessorAddedEvent?.Invoke("server");
    }

    private void AddBotButton_Click(object sender, EventArgs e)
    {
        ProcessorAddedEvent?.Invoke("bot");
    }
}