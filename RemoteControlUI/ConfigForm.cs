using RemoteControl.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace RemoteControl
{
    public partial class ConfigForm : Form
    {
        private readonly Main _program;
        private readonly ToolStripMenuItem _stoppedMenuItem = new ToolStripMenuItem
        {
            Text = @"Stopped",
            Enabled = false,
        };

        private readonly SynchronizationContext _context;

        public ConfigForm()
        {
            InitializeComponent();

            _context = SynchronizationContext.Current;

            _program = new Main();

            NetworkChange.NetworkAddressChanged += UpdateUris;

            try
            {
                UpdateUris(null, null);
            }
            catch
            {
                SetStopped();
            }
        }

        private static IEnumerable<string> GetCurrentIPs => Dns.GetHostAddresses(Dns.GetHostName())
            .Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(x => x.ToString());

        private static IEnumerable<Uri> GetCurrentUris(int port) =>
            GetCurrentIPs.Select(x => new UriBuilder(AppConfig.DefaultScheme, x, port).Uri);

        private void UpdateUris(object sender, EventArgs e)
        {
            try
            {
                _program.Stop();

                var uris = GetCurrentUris(AppConfig.DefaultPort).ToList();
                if (uris.Count == 0) return;
                
                AppConfig.CurrentUri = uris.All(x => x.Host != AppConfig.PrefUri.Host) ? uris.First() : AppConfig.PrefUri;

                _program.Start(AppConfig.CurrentUri);

                _context.Send(_ =>
                {
                    comboBoxUris.DataSource = uris.Select(x => x.ToString()).ToList();
                    comboBoxUris.Text = AppConfig.CurrentUri.ToString();

                    IpChanged(AppConfig.CurrentUri.ToString());
                }, null);
            }
            catch
            {
                _program.Stop();

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

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _program.Stop();
            Application.Exit();
        }

        private void IpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start((sender as ToolStripMenuItem)?.Text ?? "");
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                _program.Stop();

                AppConfig.PrefUri = new Uri(comboBoxUris.Text);
                
                _program.Start(AppConfig.PrefUri);
                AppConfig.CurrentUri = AppConfig.PrefUri;

                IpChanged(AppConfig.CurrentUri.ToString());
                Minimize();
            }
            catch
            {
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

        private void RestartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _program.Restart();
            }
            catch
            {
                SetStopped();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
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
