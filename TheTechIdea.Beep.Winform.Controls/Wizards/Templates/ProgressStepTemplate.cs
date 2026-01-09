using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.Wizards;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Templates
{
    /// <summary>
    /// Template for progress/loading wizard steps
    /// Displays a progress indicator with optional status message
    /// </summary>
    public class ProgressStepTemplate : WizardStepTemplate
    {
        private readonly string _stepKey;
        private readonly string _stepTitle;
        private readonly string _statusMessage;
        private readonly bool _showProgressBar;

        public ProgressStepTemplate(string stepKey, string stepTitle, string statusMessage = "Processing...", bool showProgressBar = true)
        {
            _stepKey = stepKey;
            _stepTitle = stepTitle;
            _statusMessage = statusMessage;
            _showProgressBar = showProgressBar;
        }

        public override UserControl CreateStepControl()
        {
            var panel = new UserControl
            {
                Padding = new Padding(40)
            };

            // Status message
            var statusLabel = new BeepLabel
            {
                Text = _statusMessage,
                Location = new Point(40, 100),
                Size = new Size(panel.Width - 80, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11f),
                AutoSize = false
            };
            statusLabel.ApplyTheme();
            panel.Controls.Add(statusLabel);

            // Progress bar (if enabled)
            if (_showProgressBar)
            {
                var progressBar = new BeepProgressBar
                {
                    Location = new Point(40, 150),
                    Size = new Size(panel.Width - 80, 8),
                    Minimum = 0,
                    Maximum = 100,
                    Value = 0,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };
                progressBar.ApplyTheme();
                progressBar.Name = "ProgressBar"; // For easy access
                panel.Controls.Add(progressBar);
            }

            return panel;
        }

        public override WizardStep GetStepConfig()
        {
            return new WizardStep
            {
                Key = _stepKey,
                Title = _stepTitle,
                Description = _statusMessage
            };
        }

        /// <summary>
        /// Update progress (if progress bar exists)
        /// </summary>
        public void UpdateProgress(UserControl control, int value, int maximum = 100)
        {
            var progressBar = control.Controls.Find("ProgressBar", false);
            if (progressBar.Length > 0 && progressBar[0] is BeepProgressBar bar)
            {
                bar.Maximum = maximum;
                bar.Value = value;
            }
        }
    }
}
