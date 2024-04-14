using MainApp;
using MainUI.CustomControls.MenuItems;
using MainUI.CustomControls.Panels;
using Servers;
using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;

namespace MainUI;

// ReSharper disable once InconsistentNaming
public sealed partial class MainForm : Form
{
    private readonly ToolStripItem[] _commonMenuItems;

    private const int GroupMargin = 6;

    private readonly ObservableCollection<Server> _servers;
    private readonly List<ServerMenuItemGroup> _toolStripGroups = [];
    private readonly List<ServerPanel> _windowPanels = [];

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

    private readonly Icon _darkIcon = new(Path.Combine(AppContext.BaseDirectory, "Icons\\Device.theme-light.ico"));
    private readonly Icon _lightIcon = new(Path.Combine(AppContext.BaseDirectory, "Icons\\Device.theme-dark.ico"));

    private readonly AppHost _app;

    public MainForm(AppHost app)
    {
        InitializeComponent();

        _commonMenuItems =
        [
            new ToolStripMenuItem("Start all", null, StartAllToolStripMenuItem_Click),
            new ToolStripMenuItem("Stop all", null, StopAllToolStripMenuItem_Click),
            AutostartStripMenuItem,
            AddFirewallRuleToolStripMenuItem,
            CloseToolStripMenuItem
        ];

        Width = 550;
        Height = 40;

        _settings.ColorValuesChanged += (_, _) => ApplyTheme();
        _app = app;

        app.RunAll();

        _servers = app.Servers;

        _servers.CollectionChanged += InitUI;

        InitUI();

        ApplyTheme();
    }

    private void InitUI(object? sender = null, EventArgs? args = null)
    {
        _windowPanels.Clear();
        _windowPanels.AddRange(_servers.Select(CreatePanel).ToList());

        _toolStripGroups.Clear();
        _toolStripGroups.AddRange(_servers.Select(CreateMenuItemGroup).ToList());

        DrawWindow();
        SetContextMenu();
    }

    private ServerPanel CreatePanel(Server server)
    {
        ServerPanel panel = server.Type switch
        {
            ServerType.Web => new HttpPanel(server),
            ServerType.Bot => new BotPanel(server),
            _ => throw new NotSupportedException()
        };

        var menu = new ContextMenuStrip
        {
            RenderMode = ToolStripRenderMode.System
        };
        menu.Items.Add(new ToolStripMenuItem("Remove", null, (_, _) => _servers.Remove(server)));

        panel.ContextMenuStrip = menu;
        panel.UpdateButtonClicked += (_, _) => _app.SaveConfig();

        return panel;
    }

    private ServerMenuItemGroup CreateMenuItemGroup(Server server)
    {
        ServerMenuItemGroup group = server.Type switch
        {
            ServerType.Web => new HttpMenuItemGroup(server),
            ServerType.Bot => new BotMenuItemGroup(server),
            _ => throw new NotSupportedException()
        };

        group.OnDescriptionClick += IpToolStripMenuItem_Click;

        return group;
    }

    private void DrawWindow()
    {
        Controls.Clear();

        var height = GroupMargin;

        AddServerButton.Top = height;
        AddBotButton.Top = height;

        Controls.Add(AddServerButton);
        Controls.Add(AddBotButton);

        height += AddServerButton.Height + GroupMargin;

        foreach (var panel in _windowPanels)
        {
            panel.Top = height;
            Controls.Add(panel);

            height += panel.Height + GroupMargin;
        }

        Height = height + 40;
    }

    private void SetContextMenu()
    {
        MainContextMenuStrip.Items.Clear();

        foreach (var toolStripMenuItemGroup in _toolStripGroups)
        {
            MainContextMenuStrip.Items.AddRange(toolStripMenuItemGroup.ItemsArray);
        }

        AutostartStripMenuItem.Checked = _app.IsAutostart;
        MainContextMenuStrip.Items.AddRange(_commonMenuItems);
    }

    private void CloseToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }

    private static void IpToolStripMenuItem_Click(object? sender, string input)
    {
        var address = input.Replace("&", "^&");

        AppHost.RunCommand($"start {address}");
    }

    private void StartAllToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        foreach (var server in _servers)
        {
            server.Start();
        }
    }

    private void StopAllToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        foreach (var server in _servers)
        {
            server.Stop();
        }
    }

    private void ConfigForm_Shown(object sender, EventArgs e)
    {
        Hide();
    }

    private void AutoStartStripMenuItem_Click(object sender, EventArgs e)
    {
        _app.IsAutostart = !_app.IsAutostart;
    }

    private void AddFirewallRuleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _app.AddFirewallRules();
    }

    private void TaskbarNotify_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
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
        _servers.Add(_app.ServerFactory.GetServer());
    }

    private void AddBotButton_Click(object sender, EventArgs e)
    {
        _servers.Add(_app.ServerFactory.GetBot());
    }

    private void ApplyTheme()
    {
        var darkMode = IsDarkMode;
        var theme = darkMode ? _darkTheme : _lightTheme;

        TaskbarNotifyIcon.Icon = darkMode ? _lightIcon : _darkIcon;

        if (InvokeRequired)
        {
            Invoke(SetIconAndBarLocal);
        }
        else
        {
            SetIconAndBarLocal();
        }

        theme.ApplyTheme(this);
        theme.ApplyTheme(MainContextMenuStrip);

        foreach (Control control in Controls)
        {
            if (control is ServerPanel panel)
                panel.ApplyTheme(theme);
        }

        return;

        void SetIconAndBarLocal()
        {
            Icon = darkMode ? _lightIcon : _darkIcon;
            MainUI.DarkTitleBar.UseImmersiveDarkMode(Handle, darkMode);
        }
    }
}