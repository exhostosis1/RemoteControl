using RemoteControl.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly char[] _uriSeparators = {'\r', '\n', ' '};

        public ConfigForm()
        {
            InitializeComponent();

            try
            {
                this.textBoxUris.Text = string.Join("\n", AppConfig.Uris);

                _program = new Main(AppConfig.Uris.Length == 0);
                _program.IpChanged += IpChanged;
                _program.Start(AppConfig.Uris);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, @"Error", MessageBoxButtons.OK);
                SetStopped();
            }
        }

        private void SetStopped()
        {
            _ipMenuItems.ForEach(x => this.contextMenuStrip.Items.Remove(x));
            _ipMenuItems.Add(_stoppedMenuItem);
            this.contextMenuStrip.Items.Insert(0, _stoppedMenuItem);
        }

        private void IpChanged(object sender, IEnumerable<string> uris)
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

                AppConfig.ReadConfig(textBoxUris.Text.Split(_uriSeparators, StringSplitOptions.RemoveEmptyEntries));

                _program.IpLookup = AppConfig.Uris.Length == 0;
                _program.Start(AppConfig.Uris);
                Minimize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK);
                SetStopped();
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Minimize();

            textBoxUris.Text = string.Join("\n", AppConfig.Uris);
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
            textBoxUris.Text = AppConfig.GetFileConfig();
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
                _program.Stop();
                _program.Start(AppConfig.Uris);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK);
                textBoxUris.Text = AppConfig.GetFileConfig();
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
