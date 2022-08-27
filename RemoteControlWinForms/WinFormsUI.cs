using Shared;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

        public WinFormsUI()
        {
            InitializeComponent();

            _commonMenuItems = new ToolStripItem[]
            {
                    this.toolStripSeparator2,
                    this.autostartStripMenuItem,
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
            var address = (sender as ToolStripMenuItem)?.Text ?? string.Empty;

            try
            {
                Process.Start(address);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    address = address.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {address}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", address);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", address);
                }
                else
                {
                    throw;
                }
            }
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
    }
}
