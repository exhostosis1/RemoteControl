using RemoteControl.App;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RemoteControl.Config;

namespace RemoteControl
{
    public partial class ConfigForm : Form
    {
        private readonly SynchronizationContext? _context;

        private readonly ToolStripItem[] _startedMenuItems;
        private readonly ToolStripItem[] _stoppedMenuItems;

        private readonly Uri _prefUri = ConfigHelper.GetAppConfigFromFile().Uri.Uri;

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

            StartListening(_prefUri);   
        }

       private void StartListening(Uri uri)
        {
            try
            {
                RemoteControlApp.Start(uri);
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

            if (RemoteControlApp.IsUiListening)
            {
                this.contextMenuStrip.Items.Add(new ToolStripMenuItem(RemoteControlApp.GetUiListeningUri(), null, IpToolStripMenuItem_Click));
                this.contextMenuStrip.Items.AddRange(_startedMenuItems);

            }
            else
            {
                this.contextMenuStrip.Items.AddRange(_stoppedMenuItems);
            }
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

        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartListening(_prefUri);
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoteControlApp.Stop();

            SetContextMenu();
        }

        private void ConfigForm_Shown(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
