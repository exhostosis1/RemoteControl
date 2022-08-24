using Shared.Logging.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RemoteControlWinForms
{
    public partial class ConfigForm : Form
    {
        private readonly SynchronizationContext? _context;

        private readonly ToolStripItem[] _commonMenuItems;

        private ViewModel? _model;

        private readonly ILogger _logger;

        public Func<ViewModel>? StartEvent;
        public Func<ViewModel>? StopEvent;
        public Func<ViewModel>? AutostartEvent;

        public ConfigForm(ViewModel model, ILogger logger)
        {
            InitializeComponent();
            
            _logger = logger;
            _model = model;

            _commonMenuItems = new ToolStripItem[]
            {
                    this.toolStripSeparator2,
                    this.autostartStripMenuItem,
                    this.closeToolStripMenuItem
            };

            _context = SynchronizationContext.Current;

            this.autostartStripMenuItem.Checked = model.Autostart; 

            SetContextMenu();
        }

        private void SetContextMenu()
        {
            this.contextMenuStrip.Items.Clear();

            if (_model == null) return;

            this.contextMenuStrip.Items.Add(_model.IsListening
                ? new ToolStripMenuItem(_model.Uri, null, IpToolStripMenuItem_Click)
                : this.stoppedToolStripMenuItem);

            this.contextMenuStrip.Items.Add(this.toolStripSeparator1);

            this.contextMenuStrip.Items.Add(_model.IsListening
                ? this.stopToolStripMenuItem
                : this.startToolStripMenuItem);

            this.contextMenuStrip.Items.AddRange(_commonMenuItems);

            this.autostartStripMenuItem.Checked = _model.Autostart;
        }

        private void CloseToolStripMenuItem_Click(object? sender, EventArgs e)
        {
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
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
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
            _model = StartEvent?.Invoke();
            SetContextMenu();
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _model = StopEvent?.Invoke();
            SetContextMenu();
        }

        private void ConfigForm_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        private void autostartStripMenuItem_Click(object sender, EventArgs e)
        {
            _model = AutostartEvent?.Invoke();
            SetContextMenu();
        }
    }
}
