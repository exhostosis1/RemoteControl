using RemoteControl.App;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RemoteControl.Config;
using RemoteControl.Autostart;

namespace RemoteControl
{
    public partial class ConfigForm : Form
    {
        private readonly SynchronizationContext? _context;

        private readonly ToolStripItem[] _commonMenuItems;

        private readonly Uri? _prefUri = ConfigHelper.GetAppConfigFromFile().Uri?.Uri;
        private readonly RemoteControlApp _app;

        public ConfigForm(RemoteControlApp app)
        {
            InitializeComponent();

            _app = app;

            _commonMenuItems = new ToolStripItem[]
            {
                    this.toolStripSeparator2,
                    this.autostartStripMenuItem,
                    this.closeToolStripMenuItem
            };

            _context = SynchronizationContext.Current;

            this.autostartStripMenuItem.Checked = AutostartService.CheckAutostart();

            StartListening(_prefUri);   
        }

       private void StartListening(Uri uri)
        {
            try
            {
                _app.Start(uri);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
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

            this.contextMenuStrip.Items.Add(_app.IsUiListening
                ? new ToolStripMenuItem(_app.GetUiListeningUri(), null, IpToolStripMenuItem_Click)
                : this.stoppedToolStripMenuItem);

            this.contextMenuStrip.Items.Add(this.toolStripSeparator1);

            this.contextMenuStrip.Items.Add(_app.IsUiListening
                ? this.stopToolStripMenuItem
                : this.startToolStripMenuItem);

            this.contextMenuStrip.Items.AddRange(_commonMenuItems);
        }

        private void CloseToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            _app.Stop();
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
            _app.Stop();

            SetContextMenu();
        }

        private void ConfigForm_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        private void autostartStripMenuItem_Click(object sender, EventArgs e)
        {
            var value = !this.autostartStripMenuItem.Checked;

            AutostartService.SetAutostart(value);
            this.autostartStripMenuItem.Checked = value;
        }
    }
}
