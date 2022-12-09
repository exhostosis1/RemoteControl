using Microsoft.Win32;
using Shared;
using Shared.Config;
using Shared.Enums;

namespace RemoteControlWinForms;

// ReSharper disable once InconsistentNaming
public partial class WinFormsUI : Form, IUserInterface
{
    private readonly ToolStripItem[] _commonMenuItems;

    public event StringEventHandler? StartEvent;
    public event StringEventHandler? StopEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event EmptyEventHandler? AddFirewallRuleEvent;
    public event EmptyEventHandler? CloseEvent;
    public event ConfigEventHandler? ConfigChangedEvent;

    public IList<IControlProcessor> ControlProcessors { get; set; } = new List<IControlProcessor>();

    private AppConfig CurrentConfig { get; set; } = new AppConfig();

    private const int GroupMargin = 5;

    public bool IsAutostart { get; set; }

    private static bool IsDarkMode => Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1) as int? == 1;

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
    }

    public void RunUI(AppConfig config)
    {
        CurrentConfig = config;
        Application.Run(this);
    }

    public void ShowError(string message)
    {
        MessageBox.Show(message, @"Error", MessageBoxButtons.OK);
    }

    private void SetContextMenu()
    {
        this.contextMenuStrip.Items.Clear();

        foreach(var processor in ControlProcessors)
        {
            this.contextMenuStrip.Items.Add(new ToolStripMenuItem(processor.Name) { Enabled = false });

            switch (processor.Status)
            {
                case ControlProcessorStatus.Working:
                {
                    var item = new ToolStripMenuItem(processor.Info);
                    this.contextMenuStrip.Items.Add(item);

                    if (processor.Type == ControlProcessorType.Server)
                    {
                        item.Click += IpToolStripMenuItem_Click;
                    }

                    var stopitem = new ToolStripItemWithIntEvent
                    {
                        Text = @"Stop",
                        ClickStringValue = processor.Name
                    };
                    stopitem.Click += StopToolStripMenuItem_Click;

                    this.contextMenuStrip.Items.Add(stopitem);
                    break;
                }
                case ControlProcessorStatus.Stopped:
                    this.contextMenuStrip.Items.Add(new ToolStripMenuItem("Stopped") { Enabled = false });

                    var startitem = new ToolStripItemWithIntEvent()
                    {
                        Text = @"Start",
                        ClickStringValue = processor.Name
                    };
                    startitem.Click += StartToolStripMenuItem_Click;

                    this.contextMenuStrip.Items.Add(startitem);
                    break;
                default:
                    break;
            }

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
        if (e is not StringArgs a)
            return;

        StartEvent?.Invoke(a.Value);
    }

    private void StopToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        if (e is not StringArgs a)
            return;

        StopEvent?.Invoke(a.Value);
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

    private void autostartStripMenuItem_Click(object sender, EventArgs e)
    {
        AutostartChangedEvent?.Invoke(!IsAutostart);
    }

    private void addFirewallRuleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        AddFirewallRuleEvent?.Invoke();
    }

    private void buttonOk_Click(object sender, EventArgs e)
    {
        try
        {
            ConfigChangedEvent?.Invoke(("sdf", "asdf"));
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
            return;
        }

        Hide();
    }

    private void SetWindow()
    {
        var height = GroupMargin;

        foreach (var config in CurrentConfig.ProcessorConfigs)
        {
            var group = new GroupBox
            {
                Text = config.Name,
                Width = 252,
                Height = 55,
                Left = 12,
                Top = height
            };
            group.Controls.Add(new TextBox
            {
                Width = 238,
                Height = 23,
                Top = 22,
                Left = 6
            });

            this.Controls.Add(group);

            height += group.Height + GroupMargin;
        }

        buttonOk.Top = height;
        this.Height = height + 70;
    }

    private void taskbarNotify_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        SetWindow();
        Show();
    }

    private void WinFormsUI_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    private void taskbarNotify_Click(object sender, EventArgs e)
    {
        if ((e as MouseEventArgs)?.Button == System.Windows.Forms.MouseButtons.Right)
        {
            SetContextMenu();
        }
    }
}