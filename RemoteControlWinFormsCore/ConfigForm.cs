using RemoteControl.App;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using RemoteControl;

namespace RemoteControlWinFormsCore
{
    public partial class ConfigForm : Form
    {
        private readonly ILogger _logger;

        private readonly ToolStripMenuItem _stoppedMenuItem = new()
        {
            Text = @"Stopped",
            Enabled = false,
        };

        private readonly SynchronizationContext _context = SynchronizationContext.Current ?? new();

        public ConfigForm()
        {
            _logger = Logger.GetFileLogger(this.GetType());

            InitializeComponent();

            NetworkChange.NetworkAddressChanged += UpdateUris;

            try
            {
                UpdateUris(null, null);
            }
            catch (Exception ex)
            {
                _logger.Log(ErrorLevel.Error, ex.Message);

                SetStopped();
            }
        }

        private static IEnumerable<string> GetCurrentIPs => Dns.GetHostAddresses(Dns.GetHostName())
            .Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(x => x.ToString());

        private static IEnumerable<Uri> GetCurrentUris(int port) =>
            GetCurrentIPs.Select(x => new UriBuilder(AppConfig.DefaultScheme, x, port).Uri);

        private void UpdateUris(object? sender, EventArgs? e)
        {
            try
            {
                RemoteControlApp.Stop();

                var uris = GetCurrentUris(AppConfig.DefaultPort).ToList();
                if (uris.Count == 0) return;
                
                AppConfig.CurrentUri = uris.All(x => x.Host != AppConfig.PrefUri?.Host) ? uris.First() : AppConfig.PrefUri;

                RemoteControlApp.Start(AppConfig.CurrentUri ?? new UriBuilder().Uri);

                _context.Send(_ =>
                {
                    comboBoxUris.DataSource = uris.Select(x => x.ToString()).ToList();
                    comboBoxUris.Text = AppConfig.CurrentUri?.ToString();

                    IpChanged(AppConfig.CurrentUri?.ToString() ?? string.Empty);
                }, null);
            }
            catch(Exception ex)
            {
                _logger.Log(ErrorLevel.Error, ex.Message);

                RemoteControlApp.Stop();

                _context.Send(_ =>
                {
                    SetStopped();
                }, null);
            }
        }

        private void SetStopped()
        {
            this.contextMenuStrip.Items.RemoveAt(0);
            this.contextMenuStrip.Items.Insert(0, _stoppedMenuItem);
        }

        private void IpChanged(string uri) 
        {
            this.contextMenuStrip.Items.RemoveAt(0);
            this.contextMenuStrip.Items.Insert(0, new ToolStripMenuItem(uri, null, IpToolStripMenuItem_Click));
        }

        private void CloseToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            RemoteControlApp.Stop();
            Application.Exit();
        }

        private void IpToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var address = (sender as ToolStripMenuItem)?.Text ?? string.Empty;

            try
            {
                Process.Start(address);
            }
            catch (Exception ex)
            {
                _logger.Log(ErrorLevel.Error, ex.Message);

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

        private void ButtonSave_Click(object? sender, EventArgs e)
        {
            try
            {
                RemoteControlApp.Stop();

                AppConfig.PrefUri = new Uri(comboBoxUris.Text);

                RemoteControlApp.Start(AppConfig.PrefUri);
                AppConfig.CurrentUri = AppConfig.PrefUri;

                IpChanged(AppConfig.CurrentUri.ToString());
                Minimize();
            }
            catch (Exception ex)
            {
                _logger.Log(ErrorLevel.Error, ex.Message);

                SetStopped();
            }
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible) Minimize(); else Maximize();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Minimize();
                e.Cancel = true;
            }
        }

        private void RestartToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            try
            {
                RemoteControlApp.Restart();
            }
            catch (Exception ex)
            {
                _logger.Log(ErrorLevel.Error, ex.Message);

                SetStopped();
            }
        }

        private void Form1_Shown(object? sender, EventArgs e)
        {
            Hide();
        }

        private void Minimize()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            Hide();
        }

        private void Maximize()
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }
    }
}
