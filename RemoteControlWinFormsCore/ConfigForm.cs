using RemoteControl.App;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RemoteControl
{
    public partial class ConfigForm : Form
    {
        private readonly SynchronizationContext? _context;

        private readonly ToolStripItem[] _startedMenuItems;
        private readonly ToolStripItem[] _stoppedMenuItems;

        private readonly ICollection<Uri> _prefUris = AppConfig.GetConfigUris().ToList();

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

            StartListening(_prefUris);   
        }

        private void SetLabelRunning()
        {
            this.labelRunning.Text = @"Server on";
            this.labelRunning.ForeColor = Color.LimeGreen;
        }

        private void SetLabelStopped()
        {
            this.labelRunning.Text = @"Server off";
            this.labelRunning.ForeColor = Color.DarkRed;
        }

        private void GenerateCheckboxes()
        {
            this.panel1.Controls.Clear();
            var y = 0;

            foreach (var uri in _prefUris.Union(RemoteControlApp.GetCurrentIPs()))
            {
                var cb = new CheckBox()
                {
                    AutoSize = true,
                    Location = new Point(12, y),
                    Text = uri.ToString(),
                    Checked = RemoteControlApp.GetListeningUris().Any(x => x.ToString() == uri.ToString()),
                };

                cb.CheckedChanged += UriChecked;

                this.panel1.Controls.Add(cb);

                y += 20;
            }

            this.Height = this.panel1.Height + 70;
        }

        private void UriChecked(object? sender, EventArgs args)
        {
            if (sender == null) return;

            var obj = (CheckBox) sender;

            if (obj.Checked)
            {
                _prefUris.Add(new Uri(obj.Text));
            }
            else
            {
                _prefUris.Remove(_prefUris.First(x => x.ToString() == obj.Text));
            }

            RemoteControlApp.Start(_prefUris);
            UpdateUI();
        }

        private void StartListening(ICollection<Uri> uris)
        {
            try
            {
                RemoteControlApp.Start(uris);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                this.labelRunning.Text = e.Message;
                this.labelRunning.ForeColor = Color.Red;
            }
            finally
            {
                _context?.Send(_ =>
                {
                    UpdateUI();
                }, null);
            }
        }

        private void SetContextMenu()
        {
            this.contextMenuStrip.Items.Clear();

            this.contextMenuStrip.Items.AddRange(RemoteControlApp.IsListening ? RemoteControlApp.GetListeningUris()
                .Select(x => new ToolStripMenuItem(x.ToString(), null, IpToolStripMenuItem_Click) as ToolStripItem).Concat(_startedMenuItems).ToArray() : _stoppedMenuItems);  
        }

        private void UpdateUI()
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                if (RemoteControlApp.IsListening)
                    SetLabelRunning();
                else
                    SetLabelStopped();

                GenerateCheckboxes();
            }

            SetContextMenu();
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
            StartListening(_prefUris);
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoteControlApp.Stop();

            UpdateUI();
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
            UpdateUI();
        }

        private void taskbarNotify_Click(object sender, EventArgs e)
        {
            if ((e as MouseEventArgs)?.Button == MouseButtons.Left)
            {
                if (this.Visible) Minimize(); else Maximize();
            }
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Minimize();
                e.Cancel = true;
            }
        }

        private void ConfigForm_Shown(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
