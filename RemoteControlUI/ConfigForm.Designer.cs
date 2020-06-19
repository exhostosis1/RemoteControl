namespace RemoteControl
{
    partial class ConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.taskbarNotify = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonSave = new System.Windows.Forms.Button();
            this.textScheme = new System.Windows.Forms.TextBox();
            this.textHost = new System.Windows.Forms.TextBox();
            this.textPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonGet = new System.Windows.Forms.Button();
            this.checkBoxSimple = new System.Windows.Forms.CheckBox();
            this.checkBoxSocket = new System.Windows.Forms.CheckBox();
            this.textApiScheme = new System.Windows.Forms.TextBox();
            this.textApiHost = new System.Windows.Forms.TextBox();
            this.textApiPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // taskbarNotify
            // 
            this.taskbarNotify.ContextMenuStrip = this.contextMenuStrip;
            this.taskbarNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("taskbarNotify.Icon")));
            this.taskbarNotify.Text = "Remote Control";
            this.taskbarNotify.Visible = true;
            this.taskbarNotify.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon1_MouseClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ipToolStripMenuItem,
            this.restartToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(119, 70);
            // 
            // ipToolStripMenuItem
            // 
            this.ipToolStripMenuItem.Enabled = false;
            this.ipToolStripMenuItem.Name = "ipToolStripMenuItem";
            this.ipToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.ipToolStripMenuItem.Text = "Stopped";
            this.ipToolStripMenuItem.Click += new System.EventHandler(this.IpToolStripMenuItem_Click);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.RestartToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(74, 64);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(80, 23);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // textScheme
            // 
            this.textScheme.Location = new System.Drawing.Point(12, 12);
            this.textScheme.MaxLength = 5;
            this.textScheme.Name = "textScheme";
            this.textScheme.Size = new System.Drawing.Size(30, 20);
            this.textScheme.TabIndex = 5;
            // 
            // textHost
            // 
            this.textHost.Location = new System.Drawing.Point(74, 12);
            this.textHost.MaxLength = 255;
            this.textHost.Name = "textHost";
            this.textHost.Size = new System.Drawing.Size(80, 20);
            this.textHost.TabIndex = 6;
            // 
            // textPort
            // 
            this.textPort.Location = new System.Drawing.Point(176, 12);
            this.textPort.MaxLength = 6;
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(40, 20);
            this.textPort.TabIndex = 7;
            this.textPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textApiPort_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "://";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = ":";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(222, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "/";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(160, 64);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(68, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // buttonGet
            // 
            this.buttonGet.Location = new System.Drawing.Point(12, 64);
            this.buttonGet.Name = "buttonGet";
            this.buttonGet.Size = new System.Drawing.Size(56, 23);
            this.buttonGet.TabIndex = 12;
            this.buttonGet.Text = "Get";
            this.buttonGet.UseVisualStyleBackColor = true;
            this.buttonGet.Click += new System.EventHandler(this.ButtonGet_Click);
            // 
            // checkBoxSimple
            // 
            this.checkBoxSimple.AutoSize = true;
            this.checkBoxSimple.Location = new System.Drawing.Point(241, 14);
            this.checkBoxSimple.Name = "checkBoxSimple";
            this.checkBoxSimple.Size = new System.Drawing.Size(57, 17);
            this.checkBoxSimple.TabIndex = 13;
            this.checkBoxSimple.Text = "Simple";
            this.checkBoxSimple.UseVisualStyleBackColor = true;
            // 
            // checkBoxSocket
            // 
            this.checkBoxSocket.AutoSize = true;
            this.checkBoxSocket.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.checkBoxSocket.Location = new System.Drawing.Point(241, 43);
            this.checkBoxSocket.Name = "checkBoxSocket";
            this.checkBoxSocket.Size = new System.Drawing.Size(60, 17);
            this.checkBoxSocket.TabIndex = 14;
            this.checkBoxSocket.Text = "Socket";
            this.checkBoxSocket.UseVisualStyleBackColor = false;
            // 
            // textApiScheme
            // 
            this.textApiScheme.Location = new System.Drawing.Point(12, 38);
            this.textApiScheme.Name = "textApiScheme";
            this.textApiScheme.Size = new System.Drawing.Size(30, 20);
            this.textApiScheme.TabIndex = 15;
            // 
            // textApiHost
            // 
            this.textApiHost.Location = new System.Drawing.Point(74, 38);
            this.textApiHost.Name = "textApiHost";
            this.textApiHost.Size = new System.Drawing.Size(80, 20);
            this.textApiHost.TabIndex = 16;
            // 
            // textApiPort
            // 
            this.textApiPort.Location = new System.Drawing.Point(176, 39);
            this.textApiPort.Name = "textApiPort";
            this.textApiPort.Size = new System.Drawing.Size(40, 20);
            this.textApiPort.TabIndex = 17;
            this.textApiPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textApiPort_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "://";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(160, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = ":";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(222, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(12, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "/";
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(301, 93);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textApiPort);
            this.Controls.Add(this.textApiHost);
            this.Controls.Add(this.textApiScheme);
            this.Controls.Add(this.checkBoxSocket);
            this.Controls.Add(this.checkBoxSimple);
            this.Controls.Add(this.buttonGet);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textPort);
            this.Controls.Add(this.textHost);
            this.Controls.Add(this.textScheme);
            this.Controls.Add(this.buttonSave);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ConfigForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Config";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NotifyIcon taskbarNotify;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ipToolStripMenuItem;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textScheme;
        private System.Windows.Forms.TextBox textHost;
        private System.Windows.Forms.TextBox textPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonGet;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxSimple;
        private System.Windows.Forms.CheckBox checkBoxSocket;
        private System.Windows.Forms.TextBox textApiScheme;
        private System.Windows.Forms.TextBox textApiHost;
        private System.Windows.Forms.TextBox textApiPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}

