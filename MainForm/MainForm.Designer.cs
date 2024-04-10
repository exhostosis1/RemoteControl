namespace MainUI
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            MainContextMenuStrip = new ContextMenuStrip(components);
            TaskbarNotifyIcon = new NotifyIcon(components);
            AddFirewallRuleToolStripMenuItem = new ToolStripMenuItem();
            AutostartStripMenuItem = new ToolStripMenuItem();
            CloseToolStripMenuItem = new ToolStripMenuItem();
            StartToolStripMenuItem = new ToolStripMenuItem();
            AddServerButton = new Button();
            AddBotButton = new Button();
            SuspendLayout();
            // 
            // MainContextMenuStrip
            // 
            MainContextMenuStrip.Name = "contextMenuStrip1";
            MainContextMenuStrip.RenderMode = ToolStripRenderMode.System;
            MainContextMenuStrip.Size = new Size(181, 26);
            // 
            // TaskbarNotifyIcon
            // 
            TaskbarNotifyIcon.ContextMenuStrip = MainContextMenuStrip;
            TaskbarNotifyIcon.Icon = (Icon)resources.GetObject("TaskbarNotifyIcon.Icon");
            TaskbarNotifyIcon.Text = "Remote Control";
            TaskbarNotifyIcon.Visible = true;
            TaskbarNotifyIcon.MouseDoubleClick += TaskbarNotify_MouseDoubleClick;
            // 
            // AddFirewallRuleToolStripMenuItem
            // 
            AddFirewallRuleToolStripMenuItem.Name = "AddFirewallRuleToolStripMenuItem";
            AddFirewallRuleToolStripMenuItem.Size = new Size(160, 22);
            AddFirewallRuleToolStripMenuItem.Text = "Add firewall rule";
            AddFirewallRuleToolStripMenuItem.Click += AddFirewallRuleToolStripMenuItem_Click;
            // 
            // AutostartStripMenuItem
            // 
            AutostartStripMenuItem.Name = "AutostartStripMenuItem";
            AutostartStripMenuItem.Size = new Size(160, 22);
            AutostartStripMenuItem.Text = "Autostart";
            AutostartStripMenuItem.Click += AutoStartStripMenuItem_Click;
            // 
            // CloseToolStripMenuItem
            // 
            CloseToolStripMenuItem.Name = "CloseToolStripMenuItem";
            CloseToolStripMenuItem.Size = new Size(160, 22);
            CloseToolStripMenuItem.Text = "Close";
            CloseToolStripMenuItem.Click += CloseToolStripMenuItem_Click;
            // 
            // StartToolStripMenuItem
            // 
            StartToolStripMenuItem.Name = "StartToolStripMenuItem";
            StartToolStripMenuItem.Size = new Size(32, 19);
            // 
            // AddServerButton
            // 
            AddServerButton.FlatStyle = FlatStyle.Flat;
            AddServerButton.Location = new Point(144, 163);
            AddServerButton.Name = "AddServerButton";
            AddServerButton.Size = new Size(75, 23);
            AddServerButton.TabIndex = 1;
            AddServerButton.Text = "Add Server";
            AddServerButton.UseVisualStyleBackColor = false;
            AddServerButton.Click += AddServerButton_Click;
            // 
            // AddBotButton
            // 
            AddBotButton.FlatStyle = FlatStyle.Flat;
            AddBotButton.Location = new Point(289, 163);
            AddBotButton.Name = "AddBotButton";
            AddBotButton.Size = new Size(75, 23);
            AddBotButton.TabIndex = 2;
            AddBotButton.Text = "Add Bot";
            AddBotButton.UseVisualStyleBackColor = false;
            AddBotButton.Click += AddBotButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(532, 198);
            ContextMenuStrip = MainContextMenuStrip;
            Controls.Add(AddBotButton);
            Controls.Add(AddServerButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Remote Control";
            TopMost = true;
            FormClosing += WinFormsUI_FormClosing;
            Shown += ConfigForm_Shown;
            ResumeLayout(false);
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

