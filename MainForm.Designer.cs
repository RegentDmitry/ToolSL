﻿namespace ToolSL
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.historyFolderTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.selectButton = new System.Windows.Forms.Button();
            this.autoFindButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.autoStartCheckBox = new System.Windows.Forms.CheckBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.processLabel = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openMainFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.startContextmenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopcontextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startupWindows = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 241);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(284, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(51, 17);
            this.statusLabel.Text = "Version: ";
            // 
            // historyFolderTextBox
            // 
            this.historyFolderTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.historyFolderTextBox.Location = new System.Drawing.Point(15, 25);
            this.historyFolderTextBox.Name = "historyFolderTextBox";
            this.historyFolderTextBox.ReadOnly = true;
            this.historyFolderTextBox.Size = new System.Drawing.Size(257, 20);
            this.historyFolderTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "hands folder";
            // 
            // selectButton
            // 
            this.selectButton.Location = new System.Drawing.Point(144, 51);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(128, 23);
            this.selectButton.TabIndex = 5;
            this.selectButton.Text = "Select manually";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // autoFindButton
            // 
            this.autoFindButton.Location = new System.Drawing.Point(15, 51);
            this.autoFindButton.Name = "autoFindButton";
            this.autoFindButton.Size = new System.Drawing.Size(123, 23);
            this.autoFindButton.TabIndex = 6;
            this.autoFindButton.Text = "Get from PT4";
            this.autoFindButton.UseVisualStyleBackColor = true;
            this.autoFindButton.Click += new System.EventHandler(this.autoFindButton_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(53, 94);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(172, 25);
            this.startButton.TabIndex = 7;
            this.startButton.Text = "Get Hands While Playing";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 205);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(257, 23);
            this.progressBar.TabIndex = 8;
            // 
            // autoStartCheckBox
            // 
            this.autoStartCheckBox.AutoSize = true;
            this.autoStartCheckBox.Location = new System.Drawing.Point(53, 182);
            this.autoStartCheckBox.Name = "autoStartCheckBox";
            this.autoStartCheckBox.Size = new System.Drawing.Size(101, 17);
            this.autoStartCheckBox.TabIndex = 9;
            this.autoStartCheckBox.Text = "auto start import";
            this.autoStartCheckBox.UseVisualStyleBackColor = true;
            this.autoStartCheckBox.CheckedChanged += new System.EventHandler(this.autoStartCheckBox_CheckedChanged);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(53, 125);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(172, 25);
            this.stopButton.TabIndex = 10;
            this.stopButton.Text = "Stop Getting Hands";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // processLabel
            // 
            this.processLabel.AutoSize = true;
            this.processLabel.Location = new System.Drawing.Point(110, 249);
            this.processLabel.Name = "processLabel";
            this.processLabel.Size = new System.Drawing.Size(0, 13);
            this.processLabel.TabIndex = 12;
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.BalloonTipText = "Hand sender tool is working minimized";
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "ToolSL is working";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMainFormToolStripMenuItem,
            this.toolStripSeparator2,
            this.startContextmenuItem,
            this.stopcontextMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 104);
            // 
            // openMainFormToolStripMenuItem
            // 
            this.openMainFormToolStripMenuItem.Name = "openMainFormToolStripMenuItem";
            this.openMainFormToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.openMainFormToolStripMenuItem.Text = "open main form";
            this.openMainFormToolStripMenuItem.Click += new System.EventHandler(this.openMainFormToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(157, 6);
            // 
            // startContextmenuItem
            // 
            this.startContextmenuItem.Name = "startContextmenuItem";
            this.startContextmenuItem.Size = new System.Drawing.Size(160, 22);
            this.startContextmenuItem.Text = "start";
            this.startContextmenuItem.Click += new System.EventHandler(this.startContextmenuItem_Click);
            // 
            // stopcontextMenuItem
            // 
            this.stopcontextMenuItem.Name = "stopcontextMenuItem";
            this.stopcontextMenuItem.Size = new System.Drawing.Size(160, 22);
            this.stopcontextMenuItem.Text = "stop";
            this.stopcontextMenuItem.Visible = false;
            this.stopcontextMenuItem.Click += new System.EventHandler(this.stopcontextMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.closeToolStripMenuItem.Text = "close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click_1);
            // 
            // startupWindows
            // 
            this.startupWindows.AutoSize = true;
            this.startupWindows.Location = new System.Drawing.Point(53, 159);
            this.startupWindows.Name = "startupWindows";
            this.startupWindows.Size = new System.Drawing.Size(169, 17);
            this.startupWindows.TabIndex = 13;
            this.startupWindows.Text = "run tool when startup windows";
            this.startupWindows.UseVisualStyleBackColor = true;
            this.startupWindows.CheckedChanged += new System.EventHandler(this.startupWindows_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(284, 263);
            this.Controls.Add(this.startupWindows);
            this.Controls.Add(this.processLabel);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.autoStartCheckBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.autoFindButton);
            this.Controls.Add(this.selectButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.historyFolderTextBox);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(300, 154);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hand sender tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.TextBox historyFolderTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Button autoFindButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox autoStartCheckBox;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label processLabel;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopcontextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startContextmenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openMainFormToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.CheckBox startupWindows;
    }
}

