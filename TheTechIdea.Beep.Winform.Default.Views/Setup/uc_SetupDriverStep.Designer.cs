namespace TheTechIdea.Beep.Winform.Default.Views.Setup
{
    partial class uc_SetupDriverStep
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _rootPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            _contentHost = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            _lblFallback3 = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblFallback2 = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblFallback1 = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _headerPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            _lblDescription = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblTitle = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _rootPanel.SuspendLayout();
            _contentHost.SuspendLayout();
            _headerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _rootPanel
            // 
            _rootPanel.Controls.Add(_contentHost);
            _rootPanel.Controls.Add(_headerPanel);
            _rootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            _rootPanel.Location = new System.Drawing.Point(0, 0);
            _rootPanel.Name = "_rootPanel";
            _rootPanel.Padding = new System.Windows.Forms.Padding(12);
            _rootPanel.Size = new System.Drawing.Size(700, 420);
            _rootPanel.TabIndex = 0;
            // 
            // _contentHost
            // 
            _contentHost.Controls.Add(_lblFallback3);
            _contentHost.Controls.Add(_lblFallback2);
            _contentHost.Controls.Add(_lblFallback1);
            _contentHost.Dock = System.Windows.Forms.DockStyle.Fill;
            _contentHost.Location = new System.Drawing.Point(12, 88);
            _contentHost.Name = "_contentHost";
            _contentHost.Padding = new System.Windows.Forms.Padding(4);
            _contentHost.Size = new System.Drawing.Size(676, 320);
            _contentHost.TabIndex = 1;
            // 
            // _lblFallback3
            // 
            _lblFallback3.Dock = System.Windows.Forms.DockStyle.Top;
            _lblFallback3.Location = new System.Drawing.Point(4, 60);
            _lblFallback3.Name = "_lblFallback3";
            _lblFallback3.Size = new System.Drawing.Size(668, 28);
            _lblFallback3.TabIndex = 2;
            _lblFallback3.Text = "- Download missing packages from configured sources.";
            _lblFallback3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblFallback2
            // 
            _lblFallback2.Dock = System.Windows.Forms.DockStyle.Top;
            _lblFallback2.Location = new System.Drawing.Point(4, 32);
            _lblFallback2.Name = "_lblFallback2";
            _lblFallback2.Size = new System.Drawing.Size(668, 28);
            _lblFallback2.TabIndex = 1;
            _lblFallback2.Text = "- Resolve local driver assemblies.";
            _lblFallback2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblFallback1
            // 
            _lblFallback1.Dock = System.Windows.Forms.DockStyle.Top;
            _lblFallback1.Location = new System.Drawing.Point(4, 4);
            _lblFallback1.Name = "_lblFallback1";
            _lblFallback1.Size = new System.Drawing.Size(668, 28);
            _lblFallback1.TabIndex = 0;
            _lblFallback1.Text = "- Validate required package names.";
            _lblFallback1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _headerPanel
            // 
            _headerPanel.Controls.Add(_lblDescription);
            _headerPanel.Controls.Add(_lblTitle);
            _headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            _headerPanel.Location = new System.Drawing.Point(12, 12);
            _headerPanel.Name = "_headerPanel";
            _headerPanel.Size = new System.Drawing.Size(676, 76);
            _headerPanel.TabIndex = 0;
            // 
            // _lblDescription
            // 
            _lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            _lblDescription.Location = new System.Drawing.Point(0, 26);
            _lblDescription.Name = "_lblDescription";
            _lblDescription.Size = new System.Drawing.Size(676, 50);
            _lblDescription.TabIndex = 1;
            _lblDescription.Text = "Discover or install required drivers.";
            _lblDescription.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // _lblTitle
            // 
            _lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            _lblTitle.Location = new System.Drawing.Point(0, 0);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new System.Drawing.Size(676, 26);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "Driver Provision";
            _lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uc_SetupDriverStep
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(_rootPanel);
            Name = "uc_SetupDriverStep";
            Size = new System.Drawing.Size(700, 420);
            _rootPanel.ResumeLayout(false);
            _contentHost.ResumeLayout(false);
            _headerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel _rootPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepPanel _headerPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepPanel _contentHost;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblTitle;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblDescription;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblFallback1;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblFallback2;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblFallback3;
    }
}
