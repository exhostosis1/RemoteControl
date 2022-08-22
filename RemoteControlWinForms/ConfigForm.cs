using Shared;
using Shared.Logging.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RemoteControlWinForms
{
    public partial class ConfigForm : Form
    {
        private readonly SynchronizationContext? _context;

        private readonly ToolStripItem[] _commonMenuItems;

        private readonly Uri _prefUri;
        private readonly IServer _server;

        private readonly IAutostartService _autostart;

        private readonly ILogger _logger;

        public ConfigForm(IServer server, IConfigProvider config, IAutostartService autostart, ILogger logger)
        {
            InitializeComponent();

            _server = server;
            _autostart = autostart;
            _logger = logger;

            _commonMenuItems = new ToolStripItem[]
            {
                    this.toolStripSeparator2,
                    this.autostartStripMenuItem,
                    this.closeToolStripMenuItem
            };

            _context = SynchronizationContext.Current;

            this.autostartStripMenuItem.Checked = autostart.CheckAutostart();
            _prefUri = config.GetConfig().UriConfig.Uri;
            
            StartListening(_prefUri);   
        }

       private void StartListening(Uri uri)
        {
            try
            {
                if (uri is null) throw new ArgumentNullException();
                _server.Start(uri);
            }
            catch (Exception e)
            {
                _logger.Log(e.Message);
            }
            finally
            {
                _context?.Send(_ =>
                {
                    SetContextMenu();
                }, null);
            }
        }

        private void SetContextMenu()
        {
            this.contextMenuStrip.Items.Clear();

            this.contextMenuStrip.Items.Add(_server.IsListening
                ? new ToolStripMenuItem(_server.GetListeningUri(), null, IpToolStripMenuItem_Click)
                : this.stoppedToolStripMenuItem);

            this.contextMenuStrip.Items.Add(this.toolStripSeparator1);

            this.contextMenuStrip.Items.Add(_server.IsListening
                ? this.stopToolStripMenuItem
                : this.startToolStripMenuItem);

            this.contextMenuStrip.Items.AddRange(_commonMenuItems);
        }

        private void CloseToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            _server.Stop();
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
            StartListening(_prefUri);
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _server.Stop();

            SetContextMenu();
        }

        private void ConfigForm_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        private void autostartStripMenuItem_Click(object sender, EventArgs e)
        {
            var value = !this.autostartStripMenuItem.Checked;

            _autostart.SetAutostart(value);
            this.autostartStripMenuItem.Checked = value;
        }
    }
}
