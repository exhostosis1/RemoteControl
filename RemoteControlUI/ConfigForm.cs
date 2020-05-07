using RemoteControlCore;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RemoteControlUI
{
    public partial class ConfigForm : Form
    {
        readonly Core program = new Core();

        public ConfigForm()
        {
            InitializeComponent();

            try
            {
                var conf = AppConfig.GetServerConfig();

                SetConfigTextBoxes(conf);
                program.Start(new UriBuilder(conf.Scheme, conf.Host, conf.Port), conf.Simple);

                EnableMenuItem();
            }
            catch(Exception e)
            {
                DisableMenuItem();
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            program.Stop();
            Application.Exit();
        }

        private void IpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(this.ipToolStripMenuItem.Text);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                program.Restart(new UriBuilder(this.textProtocol.Text, this.textHost.Text, Convert.ToInt32(this.textPort.Text)), this.checkBoxSimple.Checked);

                Minimize();
            }
            catch (Exception ex)
            {
                DisableMenuItem();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                return;
            }

            EnableMenuItem();
        }
        private void SetConfigTextBoxes(AppConfig config)
        {
            this.textProtocol.Text = config.Scheme;
            this.textHost.Text = config.Host;
            this.textPort.Text = config.Port.ToString();

            this.checkBoxSimple.Checked = config.Simple;
        }

        private void EnableMenuItem()
        {
            this.ipToolStripMenuItem.Enabled = true;
            this.ipToolStripMenuItem.Text = program.GetCurrentConfig().Item1.Uri.ToString();
        }

        private void DisableMenuItem()
        {
            this.ipToolStripMenuItem.Enabled = false;
            this.ipToolStripMenuItem.Text = "Stopped";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Minimize();
            var config = program.GetCurrentConfig();
            var ub = config.Item1;
            var simple = config.Item2;

            SetConfigTextBoxes(new AppConfig(ub.Scheme, ub.Host, ub.Port, simple));
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible) Minimize(); else Maximize();
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            SetConfigTextBoxes(AppConfig.GetServerConfig());
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
                program.Restart();
            }
            catch
            {
                var config = AppConfig.GetServerConfig();
                program.Restart(new UriBuilder(config.Scheme, config.Host, config.Port), config.Simple);

                SetConfigTextBoxes(config);
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
    }
}
