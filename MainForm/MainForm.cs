using MainUI.CustomControls.MenuItems;
using MainUI.CustomControls.Panels;
using Servers;
using Shared;
using Shared.Config;
using Windows.UI.ViewManagement;

namespace MainUI;

// ReSharper disable once InconsistentNaming
public sealed partial class MainForm : Form
{
    private readonly ToolStripItem[] _commonMenuItems;

    private const int GroupMargin = 6;
    private bool IsAutoStart { get; set; }

    private List<Server> _model = [];
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

    private readonly AppHost.AppHost _viewModel;

    public MainForm(AppHost.AppHost app)
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
        _viewModel = app;

        app.ServersReady += (_, servers) => _model = servers;
        app.ServerAdded += AddServer;
        app.AutostartChanged += SetAutoStartValue;

        app.Run();

        PopulateWindowPanels();
        PopulateContextMenuGroups();

        DrawWindow();
        SetContextMenu();

        ApplyTheme();
    }

    private void PopulateWindowPanels() => _windowPanels.AddRange(_model.Select(CreatePanel));
    private void PopulateContextMenuGroups() => _toolStripGroups.AddRange(_model.Select(CreateMenuItemGroup));

    private ServerPanel CreatePanel(Server server)
    {
        ServerPanel panel = server.Type switch
        {
            ServerType.Web => new HttpPanel(server),
            ServerType.Bot => new BotPanel(server),
            _ => throw new NotSupportedException()
        };

        panel.StartButtonClicked += (_, id) => _viewModel.ServerStart(id);
        panel.StopButtonClicked += (_, id) => _viewModel.ServerStop(id);
        panel.UpdateButtonClicked += (_, config) => _viewModel.ConfigChange(config);

        var menu = new ContextMenuStrip
        {
            RenderMode = ToolStripRenderMode.System
        };
        menu.Items.Add(new ToolStripMenuItem("Remove", null, (_, _) => RemoveClicked(server.Id)));

        panel.ContextMenuStrip = menu;

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
        group.OnStartClick += (_, id) => _viewModel.ServerStart(id);
        group.OnStopClick += (_, id) => _viewModel.ServerStop(id);

        return group;
    }

    private void AddServer(object? _, Server server)
    {
        _windowPanels.Add(CreatePanel(server));
        _toolStripGroups.Add(CreateMenuItemGroup(server));

        DrawWindow();
        SetContextMenu();

        ApplyTheme();
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

        AutostartStripMenuItem.Checked = IsAutoStart;
        MainContextMenuStrip.Items.AddRange(_commonMenuItems);
    }

    private void RemoveClicked(int id)
    {
        _viewModel.ServerRemove(id);

        var p = _windowPanels.FirstOrDefault(x => x.Id == id);
        if (p != null)
        {
            _windowPanels.Remove(p);
            p.Dispose();
        }

        var t = _toolStripGroups.FirstOrDefault(x => x.Id == id);
        if (t != null)
        {
            _toolStripGroups.Remove(t);
            t.Dispose();
        }

        DrawWindow();
        SetContextMenu();
    }

    private void CloseToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        _viewModel.AppClose(null);
    }

    private static void IpToolStripMenuItem_Click(object? sender, string input)
    {
        var address = input.Replace("&", "^&");

        Utils.RunWindowsCommand($"start {address}", out _, out _);
    }

    private void StartAllToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        _viewModel.ServerStart(null);
    }

    private void StopAllToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        _viewModel.ServerStop(null);
    }

    private void ConfigForm_Shown(object sender, EventArgs e)
    {
        Hide();
    }

    private void AutoStartStripMenuItem_Click(object sender, EventArgs e)
    {
        _viewModel.AutoStartChange(!IsAutoStart);
    }

    private void AddFirewallRuleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var uris = _model.Where(x => x.Status)
            .Select(x => x.Config.Uri);
        Utils.AddFirewallRule(uris);
    }

    private void TaskbarNotify_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
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
        _viewModel.ServerAdd(ServerType.Web);
    }

    private void AddBotButton_Click(object sender, EventArgs e)
    {
        _viewModel.ServerAdd(ServerType.Bot);
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

    private void SetAutoStartValue(object? _, bool value) => IsAutoStart = value;
}