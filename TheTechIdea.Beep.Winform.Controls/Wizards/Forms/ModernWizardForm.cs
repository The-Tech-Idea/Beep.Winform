using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Modern wizard form with top progress bar
    /// Clean design with progress indicator (Image 5 style)
    /// </summary>
    public class ModernWizardForm : BaseWizardForm
    {
        private Label _lblTitle;
        private Label _lblDescription;
        private Panel _progressPanel;
        private Label _lblProgress;

        public ModernWizardForm(WizardInstance instance) : base(instance)
        {
        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // Title
            _lblTitle = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 15),
                Size = new Size(600, 30),
                Text = _instance.Config.Title
            };

            // Description
            _lblDescription = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 50),
                Size = new Size(600, 20),
                Text = _instance.Config.Description ?? ""
            };

            // Progress panel
            _progressPanel = new Panel
            {
                Location = new Point(0, 75),
                Size = new Size(ClientSize.Width, 4),
                BackColor = Color.FromArgb(230, 230, 230),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Progress percentage label
            _lblProgress = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(ClientSize.Width - 80, 50),
                Size = new Size(60, 20),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            _headerPanel.Controls.Add(_lblTitle);
            _headerPanel.Controls.Add(_lblDescription);
            _headerPanel.Controls.Add(_lblProgress);
            _headerPanel.Controls.Add(_progressPanel);

            // Adjust header height
            _headerPanel.Height = 80;
        }

        protected override void UpdateHeader()
        {
            var currentIndex = _instance.CurrentStepIndex;
            var totalSteps = _instance.Config.Steps.Count;

            if (currentIndex >= 0 && currentIndex < totalSteps)
            {
                var currentStep = _instance.Config.Steps[currentIndex];
                _lblTitle.Text = currentStep.Title ?? _instance.Config.Title;
                _lblDescription.Text = currentStep.Description ?? _instance.Config.Description ?? "";
            }

            // Update progress
            var percentage = WizardHelpers.GetCompletionPercentage(currentIndex, totalSteps);
            _lblProgress.Text = $"{percentage}%";

            // Redraw progress bar
            _progressPanel.Invalidate();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Custom paint for progress bar
            _progressPanel.Paint += (s, pe) =>
            {
                var currentIndex = _instance.CurrentStepIndex;
                var totalSteps = _instance.Config.Steps.Count;
                var accentColor = _instance.Config.Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);

                WizardHelpers.DrawProgressBar(
                    pe.Graphics,
                    _progressPanel.ClientRectangle,
                    currentIndex + 1,
                    totalSteps,
                    accentColor);
            };
        }
    }
}
