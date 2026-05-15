namespace TheTechIdea.Beep.Winform.Default.Views.Setup
{
    partial class uc_SetupWizard
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _rootPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            _statusPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            _lblStatus = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _actionsPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            _btnReset = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            _btnLaunch = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            _optionsPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            _chkAllowSkip = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            _chkAllowCancel = new TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool();
            _cmbWizardStyle = new TheTechIdea.Beep.Winform.Controls.BeepComboBox();
            _lblStyle = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _headerPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            _lblSubtitle = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblTitle = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _rootPanel.SuspendLayout();
            _statusPanel.SuspendLayout();
            _actionsPanel.SuspendLayout();
            _optionsPanel.SuspendLayout();
            _headerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _rootPanel
            // 
            _rootPanel.Controls.Add(_statusPanel);
            _rootPanel.Controls.Add(_actionsPanel);
            _rootPanel.Controls.Add(_optionsPanel);
            _rootPanel.Controls.Add(_headerPanel);
            _rootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            _rootPanel.Location = new System.Drawing.Point(0, 0);
            _rootPanel.Name = "_rootPanel";
            _rootPanel.Padding = new System.Windows.Forms.Padding(16);
            _rootPanel.Size = new System.Drawing.Size(900, 560);
            _rootPanel.TabIndex = 0;
            // 
            // _statusPanel
            // 
            _statusPanel.Controls.Add(_lblStatus);
            _statusPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            _statusPanel.Location = new System.Drawing.Point(16, 304);
            _statusPanel.Name = "_statusPanel";
            _statusPanel.Padding = new System.Windows.Forms.Padding(8);
            _statusPanel.Size = new System.Drawing.Size(868, 240);
            _statusPanel.TabIndex = 3;
            // 
            // _lblStatus
            // 
            _lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            _lblStatus.Location = new System.Drawing.Point(8, 8);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new System.Drawing.Size(852, 224);
            _lblStatus.TabIndex = 0;
            _lblStatus.Text = "Ready.";
            _lblStatus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // _actionsPanel
            // 
            _actionsPanel.Controls.Add(_btnReset);
            _actionsPanel.Controls.Add(_btnLaunch);
            _actionsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            _actionsPanel.Location = new System.Drawing.Point(16, 248);
            _actionsPanel.Name = "_actionsPanel";
            _actionsPanel.Padding = new System.Windows.Forms.Padding(4);
            _actionsPanel.Size = new System.Drawing.Size(868, 56);
            _actionsPanel.TabIndex = 2;
            // 
            // _btnReset
            // 
            _btnReset.Location = new System.Drawing.Point(220, 8);
            _btnReset.Name = "_btnReset";
            _btnReset.Size = new System.Drawing.Size(140, 36);
            _btnReset.TabIndex = 1;
            _btnReset.Text = "Reset Defaults";
            _btnReset.Click += BtnReset_Click;
            // 
            // _btnLaunch
            // 
            _btnLaunch.Location = new System.Drawing.Point(8, 8);
            _btnLaunch.Name = "_btnLaunch";
            _btnLaunch.Size = new System.Drawing.Size(200, 36);
            _btnLaunch.TabIndex = 0;
            _btnLaunch.Text = "Launch Setup Wizard";
            _btnLaunch.Click += BtnLaunch_Click;
            // 
            // _optionsPanel
            // 
            _optionsPanel.Controls.Add(_chkAllowSkip);
            _optionsPanel.Controls.Add(_chkAllowCancel);
            _optionsPanel.Controls.Add(_cmbWizardStyle);
            _optionsPanel.Controls.Add(_lblStyle);
            _optionsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            _optionsPanel.Location = new System.Drawing.Point(16, 112);
            _optionsPanel.Name = "_optionsPanel";
            _optionsPanel.Padding = new System.Windows.Forms.Padding(4, 12, 4, 4);
            _optionsPanel.Size = new System.Drawing.Size(868, 136);
            _optionsPanel.TabIndex = 1;
            // 
            // _chkAllowSkip
            // 
            _chkAllowSkip.Location = new System.Drawing.Point(176, 82);
            _chkAllowSkip.Name = "_chkAllowSkip";
            _chkAllowSkip.Size = new System.Drawing.Size(260, 28);
            _chkAllowSkip.TabIndex = 3;
            _chkAllowSkip.Text = "Allow optional step skipping";
            // 
            // _chkAllowCancel
            // 
            _chkAllowCancel.Location = new System.Drawing.Point(176, 52);
            _chkAllowCancel.Name = "_chkAllowCancel";
            _chkAllowCancel.Size = new System.Drawing.Size(180, 28);
            _chkAllowCancel.TabIndex = 2;
            _chkAllowCancel.Text = "Allow cancel";
            // 
            // _cmbWizardStyle
            // 
            //_cmbWizardStyle.sty = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _cmbWizardStyle.Location = new System.Drawing.Point(176, 10);
            _cmbWizardStyle.Name = "_cmbWizardStyle";
            _cmbWizardStyle.Size = new System.Drawing.Size(240, 32);
            _cmbWizardStyle.TabIndex = 1;
            // 
            // _lblStyle
            // 
            _lblStyle.Location = new System.Drawing.Point(8, 12);
            _lblStyle.Name = "_lblStyle";
            _lblStyle.Size = new System.Drawing.Size(160, 30);
            _lblStyle.TabIndex = 0;
            _lblStyle.Text = "Wizard Style";
            _lblStyle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _headerPanel
            // 
            _headerPanel.Controls.Add(_lblSubtitle);
            _headerPanel.Controls.Add(_lblTitle);
            _headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            _headerPanel.Location = new System.Drawing.Point(16, 16);
            _headerPanel.Name = "_headerPanel";
            _headerPanel.Padding = new System.Windows.Forms.Padding(4);
            _headerPanel.Size = new System.Drawing.Size(868, 96);
            _headerPanel.TabIndex = 0;
            // 
            // _lblSubtitle
            // 
            _lblSubtitle.Dock = System.Windows.Forms.DockStyle.Fill;
            _lblSubtitle.Location = new System.Drawing.Point(4, 40);
            _lblSubtitle.Name = "_lblSubtitle";
            _lblSubtitle.Size = new System.Drawing.Size(860, 52);
            _lblSubtitle.TabIndex = 1;
            _lblSubtitle.Text = "Configure driver provisioning, data connection, schema setup, and seeding in a guided flow.";
            _lblSubtitle.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // _lblTitle
            // 
            _lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            _lblTitle.Location = new System.Drawing.Point(4, 4);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new System.Drawing.Size(860, 36);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "WinForms Setup Wizard";
            _lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uc_SetupWizard
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(_rootPanel);
            Name = "uc_SetupWizard";
            Size = new System.Drawing.Size(900, 560);
            _rootPanel.ResumeLayout(false);
            _statusPanel.ResumeLayout(false);
            _actionsPanel.ResumeLayout(false);
            _optionsPanel.ResumeLayout(false);
            _headerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel _rootPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepPanel _headerPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepPanel _optionsPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepPanel _actionsPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepPanel _statusPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblTitle;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblSubtitle;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblStyle;
        private TheTechIdea.Beep.Winform.Controls.BeepComboBox _cmbWizardStyle;
        private TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool _chkAllowCancel;
        private TheTechIdea.Beep.Winform.Controls.CheckBoxes.BeepCheckBoxBool _chkAllowSkip;
        private TheTechIdea.Beep.Winform.Controls.BeepButton _btnLaunch;
        private TheTechIdea.Beep.Winform.Controls.BeepButton _btnReset;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblStatus;
    }
}
