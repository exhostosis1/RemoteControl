namespace RemoteControlWinForms
{
    partial class WinFormsUI
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinFormsUI));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.taskbarNotify = new System.Windows.Forms.NotifyIcon(this.components);
            this.addFirewallRuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autostartStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // taskbarNotify
            // 
            this.taskbarNotify.ContextMenuStrip = this.contextMenuStrip;
            this.taskbarNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("taskbarNotify.Icon")));
            this.taskbarNotify.Text = "Remote Control";
            this.taskbarNotify.Visible = true;
            this.taskbarNotify.Click += new System.EventHandler(this.TaskbarNotify_Click);
            this.taskbarNotify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TaskbarNotify_MouseDoubleClick);
            // 
            // addFirewallRuleToolStripMenuItem
            // 
            this.addFirewallRuleToolStripMenuItem.Name = "addFirewallRuleToolStripMenuItem";
            this.addFirewallRuleToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.addFirewallRuleToolStripMenuItem.Text = "Add firewall rule";
            this.addFirewallRuleToolStripMenuItem.Click += new System.EventHandler(this.AddFirewallRuleToolStripMenuItem_Click);
            // 
            // autostartStripMenuItem
            // 
            this.autostartStripMenuItem.Name = "autostartStripMenuItem";
            this.autostartStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.autostartStripMenuItem.Text = "Autostart";
            this.autostartStripMenuItem.Click += new System.EventHandler(this.AutostartStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // WinFormsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(532, 198);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "WinFormsUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remote Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WinFormsUI_FormClosing);
            this.Shown += new System.EventHandler(this.ConfigForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private ContextMenuStrip contextMenuStrip;
        private NotifyIcon taskbarNotify;
        private ToolStripMenuItem addFirewallRuleToolStripMenuItem;
        private ToolStripMenuItem autostartStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem startToolStripMenuItem;
    }
}

