namespace TheTechIdea.Beep.Winform.Default.Views.Setup
{
    partial class uc_SetupSeedingStep
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
            _lblTask3 = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblTask2 = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblTask1 = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblDescription = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblTitle = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _rootPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _rootPanel
            // 
            _rootPanel.Controls.Add(_lblTask3);
            _rootPanel.Controls.Add(_lblTask2);
            _rootPanel.Controls.Add(_lblTask1);
            _rootPanel.Controls.Add(_lblDescription);
            _rootPanel.Controls.Add(_lblTitle);
            _rootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            _rootPanel.Location = new System.Drawing.Point(0, 0);
            _rootPanel.Name = "_rootPanel";
            _rootPanel.Padding = new System.Windows.Forms.Padding(16);
            _rootPanel.Size = new System.Drawing.Size(700, 420);
            _rootPanel.TabIndex = 0;
            // 
            // _lblTask3
            // 
            _lblTask3.Dock = System.Windows.Forms.DockStyle.Top;
            _lblTask3.Location = new System.Drawing.Point(16, 134);
            _lblTask3.Name = "_lblTask3";
            _lblTask3.Size = new System.Drawing.Size(668, 28);
            _lblTask3.TabIndex = 4;
            _lblTask3.Text = "- Persist completed seeder IDs for resume.";
            _lblTask3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblTask2
            // 
            _lblTask2.Dock = System.Windows.Forms.DockStyle.Top;
            _lblTask2.Location = new System.Drawing.Point(16, 106);
            _lblTask2.Name = "_lblTask2";
            _lblTask2.Size = new System.Drawing.Size(668, 28);
            _lblTask2.TabIndex = 3;
            _lblTask2.Text = "- Skip already-applied seeders.";
            _lblTask2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblTask1
            // 
            _lblTask1.Dock = System.Windows.Forms.DockStyle.Top;
            _lblTask1.Location = new System.Drawing.Point(16, 78);
            _lblTask1.Name = "_lblTask1";
            _lblTask1.Size = new System.Drawing.Size(668, 28);
            _lblTask1.TabIndex = 2;
            _lblTask1.Text = "- Resolve seeder dependencies.";
            _lblTask1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblDescription
            // 
            _lblDescription.Dock = System.Windows.Forms.DockStyle.Top;
            _lblDescription.Location = new System.Drawing.Point(16, 42);
            _lblDescription.Name = "_lblDescription";
            _lblDescription.Size = new System.Drawing.Size(668, 36);
            _lblDescription.TabIndex = 1;
            _lblDescription.Text = "Run ordered seeders for baseline data.";
            _lblDescription.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // _lblTitle
            // 
            _lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            _lblTitle.Location = new System.Drawing.Point(16, 16);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new System.Drawing.Size(668, 26);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "Seed Initial Data";
            _lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uc_SetupSeedingStep
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(_rootPanel);
            Name = "uc_SetupSeedingStep";
            Size = new System.Drawing.Size(700, 420);
            _rootPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel _rootPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblTitle;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblDescription;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblTask1;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblTask2;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblTask3;
    }
}
