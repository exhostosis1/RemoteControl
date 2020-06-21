using System;
using System.Diagnostics;
using System.Windows.Forms;
using RemoteControl.Core;

namespace RemoteControl
{
    public partial class ConfigForm : Form
    {
        private Main _program;
        private UriBuilder _httpHost;

        public ConfigForm()
        {
            InitializeComponent();

            try
            {
                SetConfigTextBoxes();
                SetProgram();
                EnableMenuItem();
            }
            catch(Exception e)
            {
                DisableMenuItem();
                MessageBox.Show(e.Message, @"Error", MessageBoxButtons.OK);
            }
        }

        private void SetProgram()
        {
            _httpHost = new UriBuilder(AppConfig.Scheme, AppConfig.Host, AppConfig.Port);

            _program = new Main(_httpHost);
            _program.Start();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _program.Stop();
            Application.Exit();
        }

        private void IpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(this.ipToolStripMenuItem.Text);
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                _program.Stop();

                AppConfig.Host = textHost.Text;
                AppConfig.Scheme = textScheme.Text;
                AppConfig.Port = Convert.ToInt32(textPort.Text);

                SetProgram();
                Minimize();
            }
            catch (Exception ex)
            {
                DisableMenuItem();
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK);
                return;
            }

            EnableMenuItem();
        }
        private void SetConfigTextBoxes()
        {
            this.textScheme.Text = AppConfig.Scheme;
            this.textHost.Text = AppConfig.Host;
            this.textPort.Text = AppConfig.Port.ToString();
        }

        private void EnableMenuItem()
        {
            this.ipToolStripMenuItem.Enabled = true;
            this.ipToolStripMenuItem.Text = _httpHost.Uri.ToString();
        }

        private void DisableMenuItem()
        {
            this.ipToolStripMenuItem.Enabled = false;
            this.ipToolStripMenuItem.Text = @"Stopped";
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
                _program.Restart();
            }
            catch
            {
                SetConfigTextBoxes();
                EnableMenuItem();
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
