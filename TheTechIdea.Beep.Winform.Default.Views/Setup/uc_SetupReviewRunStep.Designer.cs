namespace TheTechIdea.Beep.Winform.Default.Views.Setup
{
    partial class uc_SetupReviewRunStep
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
            _contentPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            _btnRunSetup = new TheTechIdea.Beep.Winform.Controls.BeepButton();
            _lblProgressStatus = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _progressBar = new TheTechIdea.Beep.Winform.Controls.ProgressBars.BeepProgressBar();
            _lblLastRunSummary = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblExecutionPath = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblSummary = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _headerPanel = new TheTechIdea.Beep.Winform.Controls.BeepPanel();
            _lblDescription = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _lblTitle = new TheTechIdea.Beep.Winform.Controls.BeepLabel();
            _rootPanel.SuspendLayout();
            _contentPanel.SuspendLayout();
            _headerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _rootPanel
            // 
            _rootPanel.Controls.Add(_contentPanel);
            _rootPanel.Controls.Add(_headerPanel);
            _rootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            _rootPanel.Location = new System.Drawing.Point(0, 0);
            _rootPanel.Name = "_rootPanel";
            _rootPanel.Padding = new System.Windows.Forms.Padding(12);
            _rootPanel.Size = new System.Drawing.Size(700, 420);
            _rootPanel.TabIndex = 0;
            // 
            // _contentPanel
            // 
            _contentPanel.Controls.Add(_btnRunSetup);
            _contentPanel.Controls.Add(_lblProgressStatus);
            _contentPanel.Controls.Add(_progressBar);
            _contentPanel.Controls.Add(_lblLastRunSummary);
            _contentPanel.Controls.Add(_lblExecutionPath);
            _contentPanel.Controls.Add(_lblSummary);
            _contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            _contentPanel.Location = new System.Drawing.Point(12, 88);
            _contentPanel.Name = "_contentPanel";
            _contentPanel.Padding = new System.Windows.Forms.Padding(4);
            _contentPanel.Size = new System.Drawing.Size(676, 320);
            _contentPanel.TabIndex = 1;
            // 
            // _btnRunSetup
            // 
            _btnRunSetup.Dock = System.Windows.Forms.DockStyle.Top;
            _btnRunSetup.Location = new System.Drawing.Point(4, 254);
            _btnRunSetup.Name = "_btnRunSetup";
            _btnRunSetup.Size = new System.Drawing.Size(668, 34);
            _btnRunSetup.TabIndex = 3;
            _btnRunSetup.Text = "Run Setup";
            // 
            // _lblProgressStatus
            // 
            _lblProgressStatus.Dock = System.Windows.Forms.DockStyle.Top;
            _lblProgressStatus.Location = new System.Drawing.Point(4, 228);
            _lblProgressStatus.Name = "_lblProgressStatus";
            _lblProgressStatus.Size = new System.Drawing.Size(668, 26);
            _lblProgressStatus.TabIndex = 2;
            _lblProgressStatus.Text = "Progress: Waiting to run setup.";
            _lblProgressStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _progressBar
            // 
            _progressBar.Dock = System.Windows.Forms.DockStyle.Top;
            _progressBar.Location = new System.Drawing.Point(4, 188);
            _progressBar.Name = "_progressBar";
            _progressBar.Size = new System.Drawing.Size(668, 40);
            _progressBar.TabIndex = 1;
            _progressBar.Value = 0;
            // 
            // _lblLastRunSummary
            // 
            _lblLastRunSummary.Dock = System.Windows.Forms.DockStyle.Top;
            _lblLastRunSummary.Location = new System.Drawing.Point(4, 148);
            _lblLastRunSummary.Name = "_lblLastRunSummary";
            _lblLastRunSummary.Size = new System.Drawing.Size(668, 40);
            _lblLastRunSummary.TabIndex = 5;
            _lblLastRunSummary.Text = "Last Run: Not executed yet.";
            _lblLastRunSummary.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // _lblExecutionPath
            // 
            _lblExecutionPath.Dock = System.Windows.Forms.DockStyle.Top;
            _lblExecutionPath.Location = new System.Drawing.Point(4, 124);
            _lblExecutionPath.Name = "_lblExecutionPath";
            _lblExecutionPath.Size = new System.Drawing.Size(668, 24);
            _lblExecutionPath.TabIndex = 4;
            _lblExecutionPath.Text = "Execution Path: Not run yet.";
            _lblExecutionPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblSummary
            // 
            _lblSummary.Dock = System.Windows.Forms.DockStyle.Top;
            _lblSummary.Location = new System.Drawing.Point(4, 4);
            _lblSummary.Name = "_lblSummary";
            _lblSummary.Size = new System.Drawing.Size(668, 120);
            _lblSummary.TabIndex = 0;
            _lblSummary.Text = "Review selections before finalizing setup.";
            _lblSummary.TextAlign = System.Drawing.ContentAlignment.TopLeft;
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
            _lblDescription.Text = "Confirm selected settings and run setup execution.";
            _lblDescription.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // _lblTitle
            // 
            _lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            _lblTitle.Location = new System.Drawing.Point(0, 0);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new System.Drawing.Size(676, 26);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "Review and Run";
            _lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uc_SetupReviewRunStep
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(_rootPanel);
            Name = "uc_SetupReviewRunStep";
            Size = new System.Drawing.Size(700, 420);
            _rootPanel.ResumeLayout(false);
            _contentPanel.ResumeLayout(false);
            _headerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TheTechIdea.Beep.Winform.Controls.BeepPanel _rootPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepPanel _headerPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepPanel _contentPanel;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblTitle;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblDescription;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblSummary;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblExecutionPath;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblLastRunSummary;
        private TheTechIdea.Beep.Winform.Controls.ProgressBars.BeepProgressBar _progressBar;
        private TheTechIdea.Beep.Winform.Controls.BeepLabel _lblProgressStatus;
        private TheTechIdea.Beep.Winform.Controls.BeepButton _btnRunSetup;
    }
}
