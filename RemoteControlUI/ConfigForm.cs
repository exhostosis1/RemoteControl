using RemoteControl.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace RemoteControl
{
    public partial class ConfigForm : Form
    {
        private Main _program;
        private readonly SynchronizationContext _context;
        private readonly List<ToolStripMenuItem> _ipMenuItems = new List<ToolStripMenuItem>();
        private readonly ToolStripMenuItem _stoppedMenuItem = new ToolStripMenuItem
        {
            Text = @"Stopped",
            Enabled = false,
        };

        public ConfigForm()
        {
            InitializeComponent();

            _context = SynchronizationContext.Current;

            try
            {
                SetConfigTextBoxes();

                _program = new Main(AppConfig.Scheme, AppConfig.Port, AppConfig.Host is null);
                _program.IpChanged += IpChanged;
                _program.Start(AppConfig.Host);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, @"Error", MessageBoxButtons.OK);
            }
        }

        private void IpChanged(object sender, string[] uris)
        {
            _context.Send(o =>
            {
                _ipMenuItems.ForEach(x => this.contextMenuStrip.Items.Remove(x));
                _ipMenuItems.Clear();

                foreach (var uri in uris)
                {
                    var item = (new ToolStripMenuItem(uri, null, IpToolStripMenuItem_Click));
                    _ipMenuItems.Add(item);
                    this.contextMenuStrip.Items.Insert(0, item);
                }

                if (_ipMenuItems.Count == 0)
                {
                    _ipMenuItems.Add(_stoppedMenuItem);
                    this.contextMenuStrip.Items.Insert(0, _stoppedMenuItem);
                }
            }, null);
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

                AppConfig.Host = textHost.Text;
                AppConfig.Scheme = _program.Schema = textScheme.Text;
                AppConfig.Port = _program.Port = Convert.ToInt32(textPort.Text);

                _program.IpLookup = string.IsNullOrWhiteSpace(AppConfig.Host);

                _program.Start(AppConfig.Host);
                Minimize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK);
            }
        }
        private void SetConfigTextBoxes()
        {
            this.textScheme.Text = AppConfig.Scheme;
            this.textHost.Text = AppConfig.Host;
            this.textPort.Text = AppConfig.Port.ToString();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Minimize();

            SetConfigTextBoxes();
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible) Minimize(); else Maximize();
            }
        }

        private void ButtonGet_Click(object sender, EventArgs e)
        {
            SetConfigTextBoxes();
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
                _program.Start(AppConfig.Scheme, AppConfig.Port, AppConfig.Host);
            }
            catch
            {
                SetConfigTextBoxes();
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

        private void textApiPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar)) e.Handled = true;
        }
    }
}
