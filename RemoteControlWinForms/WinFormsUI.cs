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
    public event EmptyEventHandler? AddFirewallRuleEvent;
    public event ConfigEventHandler? ConfigChangedEvent;
    public event ConfigEventHandler? ProcessorAddedEvent;

    private const int GroupMargin = 6;
    public bool IsAutostart { get; set; }
    private static bool IsDarkMode => Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1) as int? == 1;
    private List<IControlProcessor> _model = new();

    private List<MyPanel> _groups = new();

    public WinFormsUI()
    {
        InitializeComponent();

        var icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            IsDarkMode ? "Device.theme-light.ico" : "Device.theme-dark.ico"));

        this.taskbarNotify.Icon = icon;
        this.Icon = icon;

        _commonMenuItems = new ToolStripItem[]
        {
            new ToolStripMenuItem("Start all", null, StartAllToolStripMenuItem_Click),
            new ToolStripMenuItem("Stop all", null, StopAllToolStripMenuItem_Click),
            this.autostartStripMenuItem,
            this.addFirewallRuleToolStripMenuItem,
            this.closeToolStripMenuItem
        };

        this.autostartStripMenuItem.Checked = IsAutostart;

        this.Width = 550;
        this.Height = 40;
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
        this.contextMenuStrip.Items.Clear();

        for (var i = 0; i < _model.Count; i++)
        {
            var processor = _model[i];

            this.contextMenuStrip.Items.Add(new ToolStripMenuItem(processor.Name) { Enabled = false });

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

                this.contextMenuStrip.Items.Add(item);
            }
            else
            {
                this.contextMenuStrip.Items.Add(new ToolStripMenuItem("Stopped") { Enabled = false });
            }

            var startstopitem = new ToolStripMenuItemWithIndex
            {
                Text = processor.Working ? @"Stop" : @"Start",
                Index = i
            };

            startstopitem.Click += processor.Working ? StartToolStripMenuItem_Click : StopToolStripMenuItem_Click;

            this.contextMenuStrip.Items.Add(startstopitem);
            this.contextMenuStrip.Items.Add(new ToolStripSeparator());
        }

        this.contextMenuStrip.Items.AddRange(_commonMenuItems);

        this.autostartStripMenuItem.Checked = IsAutostart;
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
        AddFirewallRuleEvent?.Invoke();
    }

    private void ButtonOk_Click(object sender, EventArgs e)
    {
        try
        {
            var result = new List<CommonConfig>();

            ConfigChangedEvent?.Invoke(result);
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
            return;
        }

        Hide();
    }

    private void RedrawWindow()
    {
        var height = GroupMargin;

        _groups.Clear();
        this.Controls.Clear();

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
            
            _groups.Add(group);
            this.Controls.Add(group);

            height += group.Height + GroupMargin;
        }
        
        this.Height = height + 40;
    }

    private void TaskbarNotify_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        RedrawWindow();
        Show();
    }

    private void WinFormsUI_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    private void TaskbarNotify_Click(object sender, EventArgs e)
    {
        if ((e as MouseEventArgs)?.Button == System.Windows.Forms.MouseButtons.Right)
        {
            SetContextMenu();
        }
    }
}