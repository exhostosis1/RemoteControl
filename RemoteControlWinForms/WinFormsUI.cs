using Microsoft.Win32;
using Shared;

namespace RemoteControlWinForms
{
    // ReSharper disable once InconsistentNaming
    public partial class WinFormsUI : Form, IUserInterface
    {
        private readonly ToolStripItem[] _commonMenuItems;

        public event EmptyEventHandler? StartEvent;
        public event EmptyEventHandler? StopEvent;
        public event BoolEventHandler? AutostartChangeEvent;
        public event EmptyEventHandler? CloseEvent;
        public Uri? Uri { get; set; }
        public bool IsListening { get; set; }
        public bool IsAutostart { get; set; }

        private static bool IsDarkMode => Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1) as int? == 1;

        public WinFormsUI()
        {
            InitializeComponent();

            this.taskbarNotify.Icon = IsDarkMode ? new Icon("Device.theme-light.ico") : new Icon("Device.theme-dark.ico");

            _commonMenuItems = new ToolStripItem[]
            {
                    this.toolStripSeparator2,
                    this.autostartStripMenuItem,
                    this.addFirewallRuleToolStripMenuItem,
                    this.closeToolStripMenuItem
            };

            this.autostartStripMenuItem.Checked = IsAutostart;
        }

        public void RunUI()
        {
            SetContextMenu();
            Application.Run(this);
        }

        private void SetContextMenu()
        {
            this.contextMenuStrip.Items.Clear();

            this.contextMenuStrip.Items.Add(IsListening
                ? new ToolStripMenuItem(Uri?.ToString(), null, IpToolStripMenuItem_Click)
                : this.stoppedToolStripMenuItem);

            this.contextMenuStrip.Items.Add(this.toolStripSeparator1);

            this.contextMenuStrip.Items.Add(IsListening
                ? this.stopToolStripMenuItem
                : this.startToolStripMenuItem);

            this.contextMenuStrip.Items.AddRange(_commonMenuItems);

            this.autostartStripMenuItem.Checked = IsAutostart;
        }

        private void CloseToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            CloseEvent?.Invoke();
            Application.Exit();
        }

        private static void IpToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var address = (sender as ToolStripMenuItem)?.Text?.Replace("&", "^&") ?? string.Empty;

            Utils.RunWindowsCommand($"start {address}", out _, out _);
        }

        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartEvent?.Invoke();
            SetContextMenu();
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopEvent?.Invoke();
            SetContextMenu();
        }

        private void ConfigForm_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        private void autostartStripMenuItem_Click(object sender, EventArgs e)
        {
            AutostartChangeEvent?.Invoke(!IsAutostart);
            SetContextMenu();
        }

        private void addFirewallRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var command =
                $"netsh advfirewall firewall add rule name=\"Remote Control\" dir=in action=allow profile=private localip={Uri?.Host} localport={Uri?.Port} protocol=tcp";

            Utils.RunWindowsCommandAsAdmin(command);
        }
    }
}
