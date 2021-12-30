using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using RemoteControl.App;

namespace RemoteControl
{
    public partial class ConfigForm : Form
    {
        private readonly SynchronizationContext? _context;

        private readonly ToolStripItem[] _startedMenuItems;
        private readonly ToolStripItem[] _stoppedMenuItems;

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

            NetworkChange.NetworkAddressChanged += UrisUpdated;

            _context = SynchronizationContext.Current;
                        
            UrisUpdated(null, null);
        }

        private static IEnumerable<string> GetCurrentIPs => Dns.GetHostAddresses(Dns.GetHostName())
            .Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(x => x.ToString());

        private void UrisUpdated(object? sender, EventArgs? e)
        {
            try
            {
                if(RemoteControlApp.IsListening)
                    RemoteControlApp.Stop();

                var ips = new HashSet<string>(GetCurrentIPs);
                if (ips.Count == 0) return;

                var uris = AppConfig.PrefUris.Length > 0 ? 
                    AppConfig.PrefUris.Where(x => ips.Contains(x.Host)).ToArray() : 
                    ips.Select(x => new UriBuilder(AppConfig.DefaultScheme, x, AppConfig.DefaultPort).Uri).ToArray();

                RemoteControlApp.Start(uris);
            }
            catch
            {
                RemoteControlApp.Stop();               
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

            this.contextMenuStrip.Items.AddRange(RemoteControlApp.IsListening ? RemoteControlApp.GetCurrentUris
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
            UrisUpdated(null, null);
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoteControlApp.Stop();
            SetContextMenu();
        }
    }
}
