namespace TheTechIdea.Beep.Winform.Default.Views.Setup
{
    partial class uc_SetupConnectionStep
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
            _headerPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            _lblDescription = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblTitle = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _rootPanel.SuspendLayout();
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
            _contentHost.Dock = System.Windows.Forms.DockStyle.Fill;
            _contentHost.Location = new System.Drawing.Point(12, 88);
            _contentHost.Name = "_contentHost";
            _contentHost.Padding = new System.Windows.Forms.Padding(4);
            _contentHost.Size = new System.Drawing.Size(676, 320);
            _contentHost.TabIndex = 1;
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
            _lblDescription.Text = "Create and verify the target datasource connection.";
            _lblDescription.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // _lblTitle
            // 
            _lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            _lblTitle.Location = new System.Drawing.Point(0, 0);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new System.Drawing.Size(676, 26);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "Connection Configuration";
            _lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uc_SetupConnectionStep
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(_rootPanel);
            Name = "uc_SetupConnectionStep";
            Size = new System.Drawing.Size(700, 420);
            _rootPanel.ResumeLayout(false);
            _headerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel _rootPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepPanel _headerPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepPanel _contentHost;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblTitle;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblDescription;
    }
}
