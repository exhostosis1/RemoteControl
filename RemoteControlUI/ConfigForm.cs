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
        private readonly List<ToolStripMenuItem> _ipMenuItems = new List<ToolStripMenuItem>();
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
            catch(Exception e)
            {
                SetStopped();
            }
        }

        private static IEnumerable<string> GetCurrentIPs => Dns.GetHostAddresses(Dns.GetHostName())
            .Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(x => x.ToString());

        private static IEnumerable<Uri> GetCurrentUris(int port) =>
            GetCurrentIPs.Select(x => new UriBuilder(AppConfig.Scheme, x, port).Uri);

        private void UpdateUris(object sender, EventArgs e)
        {
            try
            {
                _program.Stop();

                var uris = GetCurrentUris(AppConfig.Port).ToList();
                if (uris.Count == 0) return;

                _context.Send(_ =>
                {
                    comboBoxUris.DataSource = uris.Select(x => x.ToString()).ToList();
                }, null);

                var uri = new UriBuilder(AppConfig.Scheme, AppConfig.Host, AppConfig.Port).Uri;

                if (uris.All(x => x.Host != uri.Host))
                {
                    uri = uris.First();
                    AppConfig.Host = uri.Host;
                    AppConfig.Port = uri.Port;
                }

                _program.Start(uri);
                IpChanged(AppConfig.Uri.ToString());
            }
            catch (Exception ex)
            {
                _context.Send(_ =>
                {
                    SetStopped();
                }, null);
            }
        }

        private void SetStopped()
        {
            _ipMenuItems.ForEach(x => this.contextMenuStrip.Items.Remove(x));
            _ipMenuItems.Add(_stoppedMenuItem);
            this.contextMenuStrip.Items.Insert(0, _stoppedMenuItem);
        }

        private void IpChanged(string uri) 
        {
            _ipMenuItems.ForEach(x => this.contextMenuStrip.Items.Remove(x));
            _ipMenuItems.Clear();

            var item = (new ToolStripMenuItem(uri, null, IpToolStripMenuItem_Click));
            _ipMenuItems.Add(item);
            this.contextMenuStrip.Items.Insert(0, item);

            if (_ipMenuItems.Count == 0)
            {
                _ipMenuItems.Add(_stoppedMenuItem);
                this.contextMenuStrip.Items.Insert(0, _stoppedMenuItem);
            }
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

                AppConfig.SetFromString(comboBoxUris.Text);

                _program.Start(AppConfig.Uri);
                IpChanged(AppConfig.Uri.ToString());
                Minimize();
            }
            catch (Exception ex)
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
                _program.Restart(AppConfig.Uri);
                IpChanged(AppConfig.Uri.ToString());
            }
            catch (Exception ex)
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
