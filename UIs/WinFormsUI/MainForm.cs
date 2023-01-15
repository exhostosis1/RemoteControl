﻿using Shared;
using Shared.ControlProcessor;
using Shared.UI;
using Windows.UI.ViewManagement;
using WinFormsUI.CustomControls.MenuItems;
using WinFormsUI.CustomControls.Panels;

namespace WinFormsUI;

// ReSharper disable once InconsistentNaming
public sealed partial class MainForm : Form, IUserInterface
{
    private readonly ToolStripItem[] _commonMenuItems;

    public event NullableIntEventHandler? StartEvent;
    public event NullableIntEventHandler? StopEvent;
    public event EmptyEventHandler? CloseEvent;
    public event BoolEventHandler? AutostartChangedEvent;
    public event ConfigWithIdEventHandler? ConfigChangedEvent;
    public event StringEventHandler? ProcessorAddedEvent;
    public event IntEventHandler? ProcessorRemovedEvent;

    private const int GroupMargin = 6;
    private bool IsAutostart { get; set; }
    
    private List<AbstractControlProcessor> _model = new();
    private readonly List<ProcessorMenuItemGroup> _toolStripGroups = new();
    private readonly List<ProcessorPanel> _windowPanels = new();

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

        _settings.ColorValuesChanged += (_, _) => ApplyTheme();
    }

    public void RunUI(List<AbstractControlProcessor> processors)
    {
        _model = processors;

        PopulateWindowPanels();
        PopulateContextMenuGroups();

        DrawWindow();
        SetContextMenu();

        ApplyTheme();

        Application.EnableVisualStyles();
        Application.Run(this);
    }

    private void PopulateWindowPanels() => _windowPanels.AddRange(_model.Select(CreatePanel));
    private void PopulateContextMenuGroups() => _toolStripGroups.AddRange(_model.Select(CreateMenuItemGrup));

    private ProcessorPanel CreatePanel(AbstractControlProcessor processor)
    {
        ProcessorPanel panel = processor switch
        {
            ServerProcessor s => new ServerPanel(s),
            BotProcessor b => new BotPanel(b),
            _ => throw new NotSupportedException()
        };

        panel.StartButtonClicked += id => StartEvent?.Invoke(id);
        panel.StopButtonClicked += id => StopEvent?.Invoke(id);
        panel.UpdateButtonClicked += (id, config) => ConfigChangedEvent?.Invoke(id, config);

        var menu = new ContextMenuStrip();
        menu.RenderMode = ToolStripRenderMode.System;
        menu.Items.Add(new ToolStripMenuItem("Remove", null, (_, _) => RemoveClicked(processor.Id)));

        panel.ContextMenuStrip = menu;

        return panel;
    }

    private ProcessorMenuItemGroup CreateMenuItemGrup(AbstractControlProcessor processor)
    {
        ProcessorMenuItemGroup group = processor switch
        {
            ServerProcessor s => new ServerMenuItemGroup(s),
            BotProcessor b => new BotMenuItemGroup(b),
            _ => throw new NotSupportedException()
        };

        group.OnDescriptionClick += IpToolStripMenuItem_Click;
        group.OnStartClick += id => StartEvent?.Invoke(id);
        group.OnStopClick += id => StopEvent?.Invoke(id);

        return group;
    }

    public void AddProcessor(AbstractControlProcessor processor)
    {
        _windowPanels.Add(CreatePanel(processor));
        _toolStripGroups.Add(CreateMenuItemGrup(processor));

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

        AutostartStripMenuItem.Checked = IsAutostart;
        MainContextMenuStrip.Items.AddRange(_commonMenuItems);
    }

    private void RemoveClicked(int id)
    {
        ProcessorRemovedEvent?.Invoke(id);

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
        CloseEvent?.Invoke();
    }

    private static void IpToolStripMenuItem_Click(string input)
    {
        var address = input.Replace("&", "^&");

        Utils.RunWindowsCommand($"start {address}", out _, out _);
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
        var uris = _model.Where(x => x.Working && x is ServerProcessor)
            .Select(x => ((ServerProcessor)x).CurrentConfig.Uri);
        Utils.AddFirewallRule(uris);
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
        ProcessorAddedEvent?.Invoke("server");
    }

    private void AddBotButton_Click(object sender, EventArgs e)
    {
        ProcessorAddedEvent?.Invoke("bot");
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

        void SetIconAndBarLocal()
        {
            Icon = darkMode ? _lightIcon : _darkIcon;
            DarkTitleBar.UseImmersiveDarkMode(Handle, darkMode);
        }

        theme.ApplyTheme(this);
        theme.ApplyTheme(MainContextMenuStrip);

        foreach (Control control in Controls)
        {
            if (control is ProcessorPanel panel)
                panel.ApplyTheme(theme);
        }
    }

    public void SetAutostartValue(bool value) => IsAutostart = value;

    public void ShowError(string message)
    {
        MessageBox.Show(message, @"Error", MessageBoxButtons.OK);
    }
}