using RemoteControl.App;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace RemoteControl
{
    public partial class ConfigForm : Form
    {
        private readonly SynchronizationContext? _context;

        private readonly ToolStripItem[] _startedMenuItems;
        private readonly ToolStripItem[] _stoppedMenuItems;

        private bool _waitingForIp = AppConfig.IpLookup;

        public ConfigForm()
        {
            InitializeComponent();

            _startedMenuItems = new ToolStripItem[]
            {
                    this.toolStripSeparator1,
                    this.stopToolStripMenuItem,
                    this.toolStripSeparator2,
                    this.closeToolStripMenuItem
            };
            _stoppedMenuItems = new ToolStripItem[]
            {
                    this.stoppedToolStripMenuItem,
                    this.toolStripSeparator1,
                    this.startToolStripMenuItem,
                    this.toolStripSeparator2,
                    this.closeToolStripMenuItem
            };

            _context = SynchronizationContext.Current;
            NetworkChange.NetworkAddressChanged += IpChanged;

            StartListening(GetUris());
        }

        private void IpChanged(object? sender, EventArgs args)
        {
            if (!_waitingForIp) return;

            StartListening(GetUris());
        }

        private static IEnumerable<string> GetCurrentIPs() => Dns.GetHostAddresses(Dns.GetHostName())
            .Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(x => x.ToString());

        private Uri[] GetUris()
        {
            if (AppConfig.IpLookup)
            {
                return GetCurrentIPs().Select(x =>
                    new UriBuilder(AppConfig.DefaultScheme, x, AppConfig.DefaultPort).Uri).ToArray();
            }
            else
            {
                var ips = GetCurrentIPs().ToHashSet();

                var hosts = AppConfig.PrefUris.Where(x => ips.Contains(x.Host)).ToArray();

                _waitingForIp = hosts.Length != AppConfig.PrefUris.Length;

                return hosts;
            }
        }

        private void StartListening(Uri[] uris)
        {
            try
            {
                RemoteControlApp.Start(uris);
            }
            catch(Exception e)
            {
                File.AppendAllText(AppContext.BaseDirectory + "error.log", $"{DateTime.Now:G} {e.Message}\n");      
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

            this.contextMenuStrip.Items.AddRange(RemoteControlApp.IsListening ? RemoteControlApp.CurrentUris
                .Select(x => new ToolStripMenuItem(x, null, IpToolStripMenuItem_Click) as ToolStripItem).Concat(_startedMenuItems).ToArray() : _stoppedMenuItems);  
        }

        private void CloseToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            RemoteControlApp.Stop();
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

        private void Form1_Shown(object? sender, EventArgs e)
        {
            Hide();
        }

        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartListening(GetUris());
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoteControlApp.Stop();

            SetContextMenu();
        }
    }
}
