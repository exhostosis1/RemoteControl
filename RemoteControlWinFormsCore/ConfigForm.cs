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

        private readonly SynchronizationContext? _context;

        private readonly ToolStripItem[] _startedMenuItems;
        private readonly ToolStripItem[] _stoppedMenuItems;

        public ConfigForm()
        {
            _logger = Logger.GetFileLogger(this.GetType());

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

                var IPs = new HashSet<string>(GetCurrentIPs);
                if (IPs.Count == 0) return;
                Uri[] uris;

                if (AppConfig.PrefUris.Length > 0)
                    uris = AppConfig.PrefUris.Where(x => IPs.Contains(x.Host)).ToArray();
                else
                    uris = IPs.Select(x => new UriBuilder(AppConfig.DefaultScheme, x, AppConfig.DefaultPort).Uri).ToArray();

                RemoteControlApp.Start(uris);
            }
            catch(Exception ex)
            {
                _logger.Log(ErrorLevel.Error, ex.Message);

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

        private void Form1_Shown(object? sender, EventArgs e)
        {
            Hide();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UrisUpdated(null, null);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoteControlApp.Stop();
            SetContextMenu();
        }
    }
}
