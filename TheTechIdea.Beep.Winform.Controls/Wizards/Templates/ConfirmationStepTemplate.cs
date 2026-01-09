using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Wizards;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Templates
{
    /// <summary>
    /// Template for confirmation wizard steps
    /// Displays a confirmation message with optional details
    /// </summary>
    public class ConfirmationStepTemplate : WizardStepTemplate
    {
        private readonly string _stepKey;
        private readonly string _stepTitle;
        private readonly string _message;
        private readonly string _details;

        public ConfirmationStepTemplate(string stepKey, string stepTitle, string message, string details = "")
        {
            _stepKey = stepKey;
            _stepTitle = stepTitle;
            _message = message;
            _details = details;
        }

        public override UserControl CreateStepControl()
        {
            var panel = new UserControl
            {
                Padding = new Padding(40)
            };

            // Message label
            var messageLabel = new BeepLabel
            {
                Text = _message,
                Location = new Point(40, 40),
                Size = new Size(panel.Width - 80, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12f, FontStyle.Regular),
                AutoSize = false
            };
            messageLabel.ApplyTheme();
            panel.Controls.Add(messageLabel);

            // Details label (if provided)
            if (!string.IsNullOrEmpty(_details))
            {
                var detailsLabel = new BeepLabel
                {
                    Text = _details,
                    Location = new Point(40, 120),
                    Size = new Size(panel.Width - 80, 100),
                    TextAlign = ContentAlignment.TopCenter,
                    Font = new Font("Segoe UI", 9f),
                    AutoSize = false
                };
                detailsLabel.ApplyTheme();
                panel.Controls.Add(detailsLabel);
            }

            return panel;
        }

        public override WizardStep GetStepConfig()
        {
            return new WizardStep
            {
                Key = _stepKey,
                Title = _stepTitle,
                Description = _message
            };
        }
    }
}
