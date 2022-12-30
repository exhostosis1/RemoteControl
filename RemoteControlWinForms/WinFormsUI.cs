using Microsoft.Win32;
using Shared;
using Shared.Enums;

namespace RemoteControlWinForms;

// ReSharper disable once InconsistentNaming
public partial class WinFormsUI : Form, IUserInterface
{
    private readonly ToolStripItem[] _commonMenuItems;

    public event ProcessorEventHandler? StartEvent;
    public event ProcessorEventHandler? StopEvent;
    public event EmptyEventHandler? CloseEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event EmptyEventHandler? AddFirewallRuleEvent;
    public event ConfigEventHandler? ConfigChangedEvent;

    private readonly List<MyGroupBox> _groups = new();
    private const int GroupMargin = 5;
    public bool IsAutostart { get; set; }
    private const char UsernamesSeparator = ';';
    private static bool IsDarkMode => Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1) as int? == 1;
    private List<ControlProcessorDto> _model = new();

    public void SetViewModel(IEnumerable<ControlProcessorDto> model) => _model = model.ToList();

    public void SetAutostartValue(bool value) => IsAutostart = value;

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

    public void RunUI()
    {
        Application.EnableVisualStyles();
        Application.Run(this);
    }

    public void ShowError(string message)
    {
        MessageBox.Show(message, @"Error", MessageBoxButtons.OK);
    }

    private void SetContextMenu()
    {
        this.contextMenuStrip.Items.Clear();

        foreach (var processor in _model)
        {
            this.contextMenuStrip.Items.Add(new ToolStripMenuItem(processor.Name) { Enabled = false });

            if (processor.Running)
            {
                var item = new ToolStripMenuItem();
                
                switch (processor)
                {
                    case ServerDto s:
                        item.Text = s.ListeningUri;
                        item.Click += IpToolStripMenuItem_Click;
                        break;
                    case BotDto b:
                        item.Text = b.BotUsernames;
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

            var startstopitem = new MyToolStripMenuItem
                {
                    Text = processor.Running ? @"Stop" : @"Start",
                    ProcessorName = processor.Name,
                    ProcessorType = processor switch
                    {
                        ServerDto => ControlProcessorType.Server,
                        BotDto => ControlProcessorType.Bot,
                        _ => ControlProcessorType.Common
                    }
                };

            startstopitem.Click += processor.Running ? StartToolStripMenuItem_Click : StopToolStripMenuItem_Click;

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
        if (e is not MyEventArgs a)
            return;

        StartEvent?.Invoke(a.ProcessorName, a.ProcessorType);
    }

    private void StopToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        if (e is not MyEventArgs a)
            return;

        StopEvent?.Invoke(a.ProcessorName, a.ProcessorType);
    }
    private void StartAllToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        StartEvent?.Invoke(null, ControlProcessorType.Common);
    }

    private void StopAllToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        StopEvent?.Invoke(null, ControlProcessorType.Common);
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
            var result = new List<ControlProcessorDto>();

            foreach (var group in _groups)
            {
                if (group.ProcessorType == ControlProcessorType.Server)
                {
                    var item = new ServerDto
                    {
                        ListeningUri = group.ProcessorText,
                        Name = group.ProcessorName
                    };

                    result.Add(item);
                }
                else if (group.ProcessorType == ControlProcessorType.Bot)
                {
                    var botitem = new BotDto
                    {
                        BotUsernames = group.ProcessorText,
                        Name = group.Name
                    };

                    result.Add(botitem);
                }
            }

            ConfigChangedEvent?.Invoke(result);
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
        _groups.Clear();
        this.Controls.Clear();

        foreach (var processor in _model)
        {
            var type = ControlProcessorType.Common;
            var text = string.Empty;

            switch (processor)
            {
                case ServerDto s:
                    type = ControlProcessorType.Server;
                    text = s.ListeningUri;
                    break;
                case BotDto b:
                    type = ControlProcessorType.Bot;
                    text = b.BotUsernames;
                    break;
                default:
                    break;
            }

            var group = new MyGroupBox(processor.Name, type, text);
            group.Top = height;

            _groups.Add(group);

            height += group.Height + GroupMargin;
        }

        buttonOk.Top = height;
        this.Height = height + 70;

        this.Controls.AddRange(_groups.Select(x => x as Control).ToArray());
        this.Controls.Add(buttonOk);
    }

    private void TaskbarNotify_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        SetWindow();
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