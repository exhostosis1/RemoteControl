using RemoteControlCore;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RemoteControlUI
{
    public partial class ConfigForm : Form
    {
        Core program = new Core();

        public ConfigForm()
        {
            InitializeComponent();

            try
            {
                var conf = AppConfig.GetServerConfig();

                SetConfigTextBoxes(conf);
                program.Start(conf);

                EnableMenuItem();
            }
            catch(Exception e)
            {
                DisableMenuItem();
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            program.Stop();
            Application.Exit();
        }

        private void ipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(this.ipToolStripMenuItem.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                program.Restart((this.textProtocol.Text, this.textHost.Text, this.textPort.Text));

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
        private void SetConfigTextBoxes((string, string, string) values)
        {
            this.textProtocol.Text = values.Item1;
            this.textHost.Text = values.Item2;
            this.textPort.Text = values.Item3;
        }

        private void SetConfigTextBoxes(UriBuilder ub)
        {
            SetConfigTextBoxes((ub.Scheme, ub.Host, ub.Port.ToString()));
        }

        private void EnableMenuItem()
        {
            this.ipToolStripMenuItem.Enabled = true;
            this.ipToolStripMenuItem.Text = program.GetUriBuilder().Uri.ToString();
        }

        private void DisableMenuItem()
        {
            this.ipToolStripMenuItem.Enabled = false;
            this.ipToolStripMenuItem.Text = "Stopped";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Minimize();
            SetConfigTextBoxes(program.GetUriBuilder());
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visible) Minimize(); else Maximize();
            }
        }

        private void button3_Click(object sender, EventArgs e)
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

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                program.Restart();
            }
            catch
            {
                var config = AppConfig.GetServerConfig();
                program.Restart(config);

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
