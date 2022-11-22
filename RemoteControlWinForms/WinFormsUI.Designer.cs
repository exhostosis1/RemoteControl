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
            this.taskbarNotify = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.placeholderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stoppedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.autostartStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFirewallRuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonOk = new System.Windows.Forms.Button();
            this.textBoxUri = new System.Windows.Forms.TextBox();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // taskbarNotify
            // 
            this.taskbarNotify.ContextMenuStrip = this.contextMenuStrip;
            this.taskbarNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("taskbarNotify.Icon")));
            this.taskbarNotify.Text = "Remote Control";
            this.taskbarNotify.Visible = true;
            this.taskbarNotify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.taskbarNotify_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.placeholderToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.stoppedToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripSeparator2,
            this.autostartStripMenuItem,
            this.addFirewallRuleToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(161, 148);
            // 
            // placeholderToolStripMenuItem
            // 
            this.placeholderToolStripMenuItem.Name = "placeholderToolStripMenuItem";
            this.placeholderToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.placeholderToolStripMenuItem.Text = "placeholder";
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.StopToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // stoppedToolStripMenuItem
            // 
            this.stoppedToolStripMenuItem.Enabled = false;
            this.stoppedToolStripMenuItem.Name = "stoppedToolStripMenuItem";
            this.stoppedToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.stoppedToolStripMenuItem.Text = "Stopped";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(157, 6);
            // 
            // autostartStripMenuItem
            // 
            this.autostartStripMenuItem.Name = "autostartStripMenuItem";
            this.autostartStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.autostartStripMenuItem.Text = "Autostart";
            this.autostartStripMenuItem.Click += new System.EventHandler(this.autostartStripMenuItem_Click);
            // 
            // addFirewallRuleToolStripMenuItem
            // 
            this.addFirewallRuleToolStripMenuItem.Name = "addFirewallRuleToolStripMenuItem";
            this.addFirewallRuleToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.addFirewallRuleToolStripMenuItem.Text = "Add firewall rule";
            this.addFirewallRuleToolStripMenuItem.Click += new System.EventHandler(this.addFirewallRuleToolStripMenuItem_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.StartToolStripMenuItem_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(71, 42);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // textBoxUri
            // 
            this.textBoxUri.Location = new System.Drawing.Point(12, 11);
            this.textBoxUri.Name = "textBoxUri";
            this.textBoxUri.Size = new System.Drawing.Size(196, 23);
            this.textBoxUri.TabIndex = 2;
            // 
            // WinFormsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(219, 73);
            this.Controls.Add(this.textBoxUri);
            this.Controls.Add(this.buttonOk);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "WinFormsUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Config";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WinFormsUI_FormClosing);
            this.Shown += new System.EventHandler(this.ConfigForm_Shown);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NotifyIcon taskbarNotify;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem placeholderToolStripMenuItem;
        private ToolStripMenuItem stopToolStripMenuItem;
        private ToolStripMenuItem stoppedToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem startToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem autostartStripMenuItem;
        private ToolStripMenuItem addFirewallRuleToolStripMenuItem;
        private Button buttonOk;
        private TextBox textBoxUri;
    }
}

