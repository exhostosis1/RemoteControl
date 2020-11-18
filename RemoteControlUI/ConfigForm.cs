using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using RemoteControl.Core;

namespace RemoteControl
{
    public partial class ConfigForm : Form
    {
        private Main _program;
        private string[] _httpHosts;

        private readonly Timer _ipLookupTimer = new Timer();

        public ConfigForm()
        {
            InitializeComponent();

            _ipLookupTimer.Enabled = false;
            _ipLookupTimer.Interval = 1000;
            _ipLookupTimer.Tick += IpLookupTimerTick;

            if (AppConfig.Hosts.Length == 0)
            {
                _ipLookupTimer.Start();
            }

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

        private void IpLookupTimerTick(object sender, EventArgs args)
        {
            
        }

        private void SetProgram()
        {
            _httpHosts = AppConfig.Hosts;

            _program = new Main();
            _program.Start(AppConfig.Scheme, AppConfig.Hosts, AppConfig.Port);
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

                AppConfig.Hosts = new []{textHost.Text};
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
            this.textHost.Text = AppConfig.Hosts.First();
            this.textPort.Text = AppConfig.Port.ToString();
        }

        private void EnableMenuItem()
        {
            this.ipToolStripMenuItem.Enabled = true;
            this.ipToolStripMenuItem.Text = _httpHosts.First();
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
                _program.Start(AppConfig.Scheme, AppConfig.Hosts, AppConfig.Port);
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
