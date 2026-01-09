using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Modern wizard form with top progress bar
    /// Clean design with progress indicator (Image 5 Style)
    /// </summary>
    public class ModernWizardForm : BaseWizardForm
    {
        private BeepLabel _lblTitle;
        private BeepLabel _lblDescription;
        private BeepProgressBar _progressBar;
        private BeepLabel _lblProgress;

        public ModernWizardForm(WizardInstance instance) : base(instance)
        {
        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // Title
            _lblTitle = new BeepLabel
            {
                AutoSize = false,
                Location = new Point(20, 15),
                Size = new Size(600, 30),
                Text = _instance.Config.Title
            };
            _lblTitle.ApplyTheme();

            // Description
            _lblDescription = new BeepLabel
            {
                AutoSize = false,
                Location = new Point(20, 50),
                Size = new Size(600, 20),
                Text = _instance.Config.Description ?? ""
            };
            _lblDescription.ApplyTheme();

            // Progress bar
            _progressBar = new BeepProgressBar
            {
                Location = new Point(0, 75),
                Size = new Size(ClientSize.Width, 4),
                Height = 4,
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            _progressBar.ApplyTheme();

            // Progress percentage label
            _lblProgress = new BeepLabel
            {
                AutoSize = false,
                Location = new Point(ClientSize.Width - 80, 50),
                Size = new Size(60, 20),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _lblProgress.ApplyTheme();

            _headerPanel.Controls.Add(_lblTitle);
            _headerPanel.Controls.Add(_lblDescription);
            _headerPanel.Controls.Add(_lblProgress);
            _headerPanel.Controls.Add(_progressBar);

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

            // Update progress bar value
            if (_progressBar != null)
            {
                _progressBar.Maximum = totalSteps;
                _progressBar.Value = currentIndex + 1;
            }
        }
    }
}
