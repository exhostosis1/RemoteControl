﻿using Shared;
using Shared.Config;
using Shared.Server;
using Shared.UI;
using Windows.UI.ViewManagement;
using Shared.Enums;
using Shared.Observable;
using WinFormsUI.CustomControls.MenuItems;
using WinFormsUI.CustomControls.Panels;

namespace WinFormsUI;

// ReSharper disable once InconsistentNaming
public sealed partial class MainForm : Form, IUserInterface
{
    private readonly ToolStripItem[] _commonMenuItems;

    public IObservable<int?> ServerStart => _serverStart;
    public IObservable<int?> ServerStop => _serverStop;
    public IObservable<object?> AppClose => _appClose;
    public IObservable<bool> AutoStartChange => _autoStartChange;
    public IObservable<(int, CommonConfig)> ConfigChange => _configChange;
    public IObservable<ServerType> ServerAdd => _serverAdd;
    public IObservable<int> ServerRemove => _serverRemove;

    private readonly MyObservable<int?> _serverStart = new();
    private readonly MyObservable<int?> _serverStop = new();
    private readonly MyObservable<object?> _appClose = new();
    private readonly MyObservable<bool> _autoStartChange = new();
    private readonly MyObservable<(int, CommonConfig)> _configChange = new();
    private readonly MyObservable<ServerType> _serverAdd = new();
    private readonly MyObservable<int> _serverRemove = new();

    private const int GroupMargin = 6;
    private bool IsAutoStart { get; set; }

    private List<IServer> _model = [];
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

    public MainForm()
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
    }

    public void RunUI(List<IServer> servers)
    {
        _model = servers;

        PopulateWindowPanels();
        PopulateContextMenuGroups();

        DrawWindow();
        SetContextMenu();

        ApplyTheme();

        Application.EnableVisualStyles();
        Application.Run(this);
    }

    private void PopulateWindowPanels() => _windowPanels.AddRange(_model.Select(CreatePanel));
    private void PopulateContextMenuGroups() => _toolStripGroups.AddRange(_model.Select(CreateMenuItemGroup));

    private ServerPanel CreatePanel(IServer server)
    {
        ServerPanel panel = server switch
        {
            IServer<WebConfig> s => new HttpPanel(s),
            IServer<BotConfig> b => new BotPanel(b),
            _ => throw new NotSupportedException()
        };

        panel.StartButtonClicked += (_, id) => _serverStart.Next(id);
        panel.StopButtonClicked += (_, id) => _serverStop.Next(id);
        panel.UpdateButtonClicked += (_, config) => _configChange.Next(config);

        var menu = new ContextMenuStrip
        {
            RenderMode = ToolStripRenderMode.System
        };
        menu.Items.Add(new ToolStripMenuItem("Remove", null, (_, _) => RemoveClicked(server.Id)));

        panel.ContextMenuStrip = menu;

        return panel;
    }

    private ServerMenuItemGroup CreateMenuItemGroup(IServer server)
    {
        ServerMenuItemGroup group = server switch
        {
            IServer<WebConfig> s => new HttpMenuItemGroup(s),
            IServer<BotConfig> b => new BotMenuItemGroup(b),
            _ => throw new NotSupportedException()
        };

        group.OnDescriptionClick += IpToolStripMenuItem_Click;
        group.OnStartClick += (_, id) => _serverStart.Next(id);
        group.OnStopClick += (_, id) => _serverStop.Next(id);

        return group;
    }

    public void AddServer(IServer server)
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
        _serverRemove.Next(id);

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
        _appClose.Next(null);
    }

    private static void IpToolStripMenuItem_Click(object? sender, string input)
    {
        var address = input.Replace("&", "^&");

        Utils.RunWindowsCommand($"start {address}", out _, out _);
    }

    private void StartAllToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        _serverStart.Next(null);
    }

    private void StopAllToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        _serverStop.Next(null);
    }

    private void ConfigForm_Shown(object sender, EventArgs e)
    {
        Hide();
    }

    private void AutoStartStripMenuItem_Click(object sender, EventArgs e)
    {
        _autoStartChange.Next(!IsAutoStart);
    }

    private void AddFirewallRuleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var uris = _model.Where(x => x.Status.Working && x is IServer<WebConfig>)
            .Select(x => ((IServer<WebConfig>)x).CurrentConfig.Uri);
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
        _serverAdd.Next(ServerType.Http);
    }

    private void AddBotButton_Click(object sender, EventArgs e)
    {
        _serverAdd.Next(ServerType.Bot);
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
            DarkTitleBar.UseImmersiveDarkMode(Handle, darkMode);
        }
    }

    public void SetAutoStartValue(bool value) => IsAutoStart = value;

    public void ShowError(string message)
    {
        MessageBox.Show(message, @"Error", MessageBoxButtons.OK);
    }
}