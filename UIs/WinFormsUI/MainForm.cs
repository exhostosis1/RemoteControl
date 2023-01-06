using Shared;
using Shared.Config;
using Shared.ControlProcessor;
using Shared.UI;
using Windows.UI.ViewManagement;
using WinFormsUI.CustomControls;

namespace WinFormsUI;

// ReSharper disable once InconsistentNaming
public sealed partial class MainForm : Form, IUserInterface
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
    
    private List<IControlProcessor> _model = new();

    private readonly Theme _lightTheme = new()
    {
        BackgroundColor = SystemColors.Control,
        ForegroundColor = SystemColors.ControlText,
        TextBoxBackgroundColor = SystemColors.ControlLightLight
    };

    private readonly Theme _darkTheme = new()
    {
        BackgroundColor = Color.FromArgb(40, 40, 40),
        ForegroundColor = SystemColors.Control,
        TextBoxBackgroundColor = Color.FromArgb(45, 45, 45)
    };

    private readonly Windows.UI.Color _colorBlack = Windows.UI.Color.FromArgb(255, 0, 0, 0);

    private readonly UISettings _settings = new();
    private bool IsDarkMode => _settings.GetColorValue(UIColorType.Background) == _colorBlack;

    private readonly Icon _darkIcon = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons\\Device.theme-light.ico"));
    private readonly Icon _lightIcon = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons\\Device.theme-dark.ico"));

    public MainForm()
    {
        InitializeComponent();

        _commonMenuItems = new ToolStripItem[]
        {
            new ToolStripMenuItem("Start all", null, StartAllToolStripMenuItem_Click),
            new ToolStripMenuItem("Stop all", null, StopAllToolStripMenuItem_Click),
            AutostartStripMenuItem,
            AddFirewallRuleToolStripMenuItem,
            CloseToolStripMenuItem
        };

        Width = 550;
        Height = 40;

        MainContextMenuStrip.Items.AddRange(_commonMenuItems);
        
        _settings.ColorValuesChanged += (_, _) => ApplyTheme();
        ApplyTheme();
    }

    public void RunUI()
    {
        Application.EnableVisualStyles();
        Application.Run(this);
    }

    private void ApplyTheme()
    {
        var darkMode = IsDarkMode;
        var theme = darkMode ? _darkTheme : _lightTheme;

        TaskbarNotifyIcon.Icon = darkMode ? _lightIcon : _darkIcon;

        if (InvokeRequired)
        {
            Invoke(() =>
            {
                Icon = darkMode ? _lightIcon : _darkIcon;
                DarkTitleBar.UseImmersiveDarkMode(Handle, darkMode);
            });
        }
        else
        {
            Icon = darkMode ? _lightIcon : _darkIcon;
            DarkTitleBar.UseImmersiveDarkMode(Handle, darkMode);
        }

        theme.ApplyTheme(this);
        theme.ApplyTheme(MainContextMenuStrip);

        var textBoxes =
            (from Control control in Controls
                where control is MyPanel
                from Control controlControl in control.Controls
                select controlControl).OfType<TextBox>().Cast<Control>().ToList();

        textBoxes.ForEach(x => theme.ApplyTheme(x));
    }

    public void SetViewModel(List<IControlProcessor> model)
    {
        _model = model;
    }

    public void SetAutostartValue(bool value) => IsAutostart = value;

    public void ShowError(string message)
    {
        MessageBox.Show(message, @"Error", MessageBoxButtons.OK);
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

        Location = new Point(Screen.PrimaryScreen?.WorkingArea.Width / 2 - Width / 2 ?? 0,
            Screen.PrimaryScreen?.WorkingArea.Height / 2 - Height / 2 ?? 0);

        ApplyTheme();
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

    private void AddServerButton_Click(object sender, EventArgs e)
    {
        ProcessorAddedEvent?.Invoke("server");
    }

    private void AddBotButton_Click(object sender, EventArgs e)
    {
        ProcessorAddedEvent?.Invoke("bot");
    }

    private void TaskbarNotifyIcon_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            SetContextMenu();
            MainContextMenuStrip.Location = new Point(MousePosition.X - MainContextMenuStrip.Width, MousePosition.Y - MainContextMenuStrip.Height);
        }
    }
}