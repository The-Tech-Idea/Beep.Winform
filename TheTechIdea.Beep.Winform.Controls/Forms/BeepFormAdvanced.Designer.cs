namespace TheTechIdea.Beep.Winform.Controls.Forms
{
    partial class BeepFormAdvanced
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
            _titleBar = new BeepPanel();
            _appIcon = new PictureBox();
            _titleLabel = new BeepLabel();
            _titleRightHost = new BeepPanel();
            _btnMin = new BeepButton();
            _btnMax = new BeepButton();
            _btnClose = new BeepButton();
            _contentHost = new BeepPanel();
            _statusBar = new BeepPanel();
            _statusLabel = new BeepLabel();
            _titleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_appIcon).BeginInit();
            _titleRightHost.SuspendLayout();
            _statusBar.SuspendLayout();
            SuspendLayout();
            // 
            // _titleBar
            // 
            _titleBar.BackColor = Color.FromArgb(40, 40, 40);
            _titleBar.BorderColor = Color.Transparent;
            _titleBar.BorderRadius = 0;
            _titleBar.Controls.Add(_titleRightHost);
            _titleBar.Controls.Add(_titleLabel);
            _titleBar.Controls.Add(_appIcon);
            _titleBar.Dock = DockStyle.Top;
            _titleBar.Location = new Point(0, 0);
            _titleBar.Name = "_titleBar";
            _titleBar.ShowTitle = false;
            _titleBar.Size = new Size(900, 44);
            _titleBar.TabIndex = 0;
            // 
            // _appIcon
            // 
            _appIcon.BackColor = Color.Transparent;
            _appIcon.Location = new Point(12, 10);
            _appIcon.Name = "_appIcon";
            _appIcon.Size = new Size(24, 24);
            _appIcon.SizeMode = PictureBoxSizeMode.CenterImage;
            _appIcon.TabIndex = 0;
            _appIcon.TabStop = false;
            // 
            // _titleLabel
            // 
            _titleLabel.AutoSize = false;
            _titleLabel.BackColor = Color.Transparent;
            _titleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            _titleLabel.ForeColor = Color.White;
            _titleLabel.Location = new Point(45, 0);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Size = new Size(300, 44);
            _titleLabel.TabIndex = 1;
            _titleLabel.Text = "Modern Form";
            _titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _titleRightHost
            // 
            _titleRightHost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _titleRightHost.BackColor = Color.Transparent;
            _titleRightHost.BorderColor = Color.Transparent;
            _titleRightHost.BorderRadius = 0;
            _titleRightHost.Controls.Add(_btnClose);
            _titleRightHost.Controls.Add(_btnMax);
            _titleRightHost.Controls.Add(_btnMin);
            _titleRightHost.Location = new Point(756, 0);
            _titleRightHost.Name = "_titleRightHost";
            _titleRightHost.ShowTitle = false;
            _titleRightHost.Size = new Size(144, 44);
            _titleRightHost.TabIndex = 2;
            // 
            // _btnMin
            // 
            _btnMin.BackColor = Color.Transparent;
            _btnMin.BorderColor = Color.Transparent;
            _btnMin.BorderRadius = 0;
            _btnMin.ForeColor = Color.White;
            _btnMin.HoverBackColor = Color.FromArgb(60, 60, 60);
            _btnMin.HoverForeColor = Color.White;
            _btnMin.Location = new Point(0, 0);
            _btnMin.Name = "_btnMin";
            _btnMin.PressedBackColor = Color.FromArgb(80, 80, 80);
            _btnMin.PressedForeColor = Color.White;
            _btnMin.Size = new Size(48, 44);
            _btnMin.TabIndex = 0;
            _btnMin.Text = "−";
            // 
            // _btnMax
            // 
            _btnMax.BackColor = Color.Transparent;
            _btnMax.BorderColor = Color.Transparent;
            _btnMax.BorderRadius = 0;
            _btnMax.ForeColor = Color.White;
            _btnMax.HoverBackColor = Color.FromArgb(60, 60, 60);
            _btnMax.HoverForeColor = Color.White;
            _btnMax.Location = new Point(48, 0);
            _btnMax.Name = "_btnMax";
            _btnMax.PressedBackColor = Color.FromArgb(80, 80, 80);
            _btnMax.PressedForeColor = Color.White;
            _btnMax.Size = new Size(48, 44);
            _btnMax.TabIndex = 1;
            _btnMax.Text = "□";
            // 
            // _btnClose
            // 
            _btnClose.BackColor = Color.Transparent;
            _btnClose.BorderColor = Color.Transparent;
            _btnClose.BorderRadius = 0;
            _btnClose.ForeColor = Color.White;
            _btnClose.HoverBackColor = Color.FromArgb(232, 17, 35);
            _btnClose.HoverForeColor = Color.White;
            _btnClose.Location = new Point(96, 0);
            _btnClose.Name = "_btnClose";
            _btnClose.PressedBackColor = Color.FromArgb(200, 15, 30);
            _btnClose.PressedForeColor = Color.White;
            _btnClose.Size = new Size(48, 44);
            _btnClose.TabIndex = 2;
            _btnClose.Text = "✕";
            // 
            // _contentHost
            // 
            _contentHost.BackColor = Color.FromArgb(250, 250, 250);
            _contentHost.BorderColor = Color.Transparent;
            _contentHost.BorderRadius = 0;
            _contentHost.Dock = DockStyle.Fill;
            _contentHost.Location = new Point(0, 44);
            _contentHost.Name = "_contentHost";
            _contentHost.Padding = new Padding(1);
            _contentHost.ShowTitle = false;
            _contentHost.Size = new Size(900, 580);
            _contentHost.TabIndex = 1;
            // 
            // _statusBar
            // 
            _statusBar.BackColor = Color.FromArgb(240, 240, 240);
            _statusBar.BorderColor = Color.Transparent;
            _statusBar.BorderRadius = 0;
            _statusBar.Controls.Add(_statusLabel);
            _statusBar.Dock = DockStyle.Bottom;
            _statusBar.Location = new Point(0, 624);
            _statusBar.Name = "_statusBar";
            _statusBar.ShowTitle = false;
            _statusBar.Size = new Size(900, 26);
            _statusBar.TabIndex = 2;
            _statusBar.Visible = false;
            // 
            // _statusLabel
            // 
            _statusLabel.BackColor = Color.Transparent;
            _statusLabel.Dock = DockStyle.Fill;
            _statusLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            _statusLabel.ForeColor = Color.FromArgb(96, 96, 96);
            _statusLabel.Location = new Point(0, 0);
            _statusLabel.Name = "_statusLabel";
            _statusLabel.Padding = new Padding(12, 0, 12, 0);
            _statusLabel.Size = new Size(900, 26);
            _statusLabel.TabIndex = 0;
            _statusLabel.Text = "Ready";
            _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // BeepFormAdvanced
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(250, 250, 250);
            ClientSize = new Size(900, 650);
            Controls.Add(_contentHost);
            Controls.Add(_statusBar);
            Controls.Add(_titleBar);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(400, 300);
            Name = "BeepFormAdvanced";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Modern Form";
            _titleBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_appIcon).EndInit();
            _titleRightHost.ResumeLayout(false);
            _statusBar.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BeepPanel _titleBar;
        private PictureBox _appIcon;
        private BeepLabel _titleLabel;
        private BeepPanel _titleRightHost;
        private BeepButton _btnMin;
        private BeepButton _btnMax;
        private BeepButton _btnClose;
        private BeepPanel _contentHost;
        private BeepPanel _statusBar;
        private BeepLabel _statusLabel;
    }
}