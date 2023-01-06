namespace WinFormsUI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TaskbarNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.AddFirewallRuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutostartStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddServerButton = new System.Windows.Forms.Button();
            this.AddBotButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MainContextMenuStrip
            // 
            this.MainContextMenuStrip.Name = "contextMenuStrip1";
            this.MainContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // TaskbarNotifyIcon
            // 
            this.TaskbarNotifyIcon.ContextMenuStrip = this.MainContextMenuStrip;
            this.TaskbarNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TaskbarNotifyIcon.Icon")));
            this.TaskbarNotifyIcon.Text = "Remote Control";
            this.TaskbarNotifyIcon.Visible = true;
            this.TaskbarNotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.taskbarNotify_MouseClick);
            this.TaskbarNotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TaskbarNotify_MouseDoubleClick);
            // 
            // AddFirewallRuleToolStripMenuItem
            // 
            this.AddFirewallRuleToolStripMenuItem.Name = "AddFirewallRuleToolStripMenuItem";
            this.AddFirewallRuleToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.AddFirewallRuleToolStripMenuItem.Text = "Add firewall rule";
            this.AddFirewallRuleToolStripMenuItem.Click += new System.EventHandler(this.AddFirewallRuleToolStripMenuItem_Click);
            // 
            // AutostartStripMenuItem
            // 
            this.AutostartStripMenuItem.Name = "AutostartStripMenuItem";
            this.AutostartStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.AutostartStripMenuItem.Text = "Autostart";
            this.AutostartStripMenuItem.Click += new System.EventHandler(this.AutostartStripMenuItem_Click);
            // 
            // CloseToolStripMenuItem
            // 
            this.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem";
            this.CloseToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.CloseToolStripMenuItem.Text = "Close";
            this.CloseToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // StartToolStripMenuItem
            // 
            this.StartToolStripMenuItem.Name = "StartToolStripMenuItem";
            this.StartToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // AddServerButton
            // 
            this.AddServerButton.Location = new System.Drawing.Point(144, 163);
            this.AddServerButton.Name = "AddServerButton";
            this.AddServerButton.Size = new System.Drawing.Size(75, 23);
            this.AddServerButton.TabIndex = 1;
            this.AddServerButton.Text = "Add Server";
            this.AddServerButton.UseVisualStyleBackColor = true;
            this.AddServerButton.Click += new System.EventHandler(this.AddServerButton_Click);
            // 
            // AddBotButton
            // 
            this.AddBotButton.Location = new System.Drawing.Point(289, 163);
            this.AddBotButton.Name = "AddBotButton";
            this.AddBotButton.Size = new System.Drawing.Size(75, 23);
            this.AddBotButton.TabIndex = 2;
            this.AddBotButton.Text = "Add Bot";
            this.AddBotButton.UseVisualStyleBackColor = true;
            this.AddBotButton.Click += new System.EventHandler(this.AddBotButton_Click);
            // 
            // WinFormsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(532, 198);
            this.Controls.Add(this.AddBotButton);
            this.Controls.Add(this.AddServerButton);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "WinFormsUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remote Control";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WinFormsUI_FormClosing);
            this.Shown += new System.EventHandler(this.ConfigForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private ContextMenuStrip MainContextMenuStrip;
        private NotifyIcon TaskbarNotifyIcon;
        private ToolStripMenuItem AddFirewallRuleToolStripMenuItem;
        private ToolStripMenuItem AutostartStripMenuItem;
        private ToolStripMenuItem CloseToolStripMenuItem;
        private ToolStripMenuItem StartToolStripMenuItem;
        private Button AddServerButton;
        private Button AddBotButton;
    }
}

