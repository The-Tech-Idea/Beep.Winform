using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Vertical stepper wizard form (Image 2 Style)
    /// Timeline-Style vertical stepper on the left
    /// </summary>
    public class VerticalStepperWizardForm : BaseWizardForm
    {
        private BeepPanel _verticalStepperPanel;

        public VerticalStepperWizardForm(WizardInstance instance) : base(instance)
        {
        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // Vertical stepper on left side
            _verticalStepperPanel = new BeepPanel
            {
                Dock = DockStyle.Left,
                Width = 300,
                Padding = new Padding(30, 40, 20, 40),
                AutoScroll = true,
                ShowTitle = false,  // No title for vertical stepper panel
                ShowTitleLine = false
            };
            _verticalStepperPanel.ApplyTheme();
            _verticalStepperPanel.Paint += VerticalStepperPanel_Paint;
        }

        protected override void LayoutControls()
        {
            base.LayoutControls();
            Controls.Add(_verticalStepperPanel);
            _verticalStepperPanel.BringToFront();
        }

        private void VerticalStepperPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var totalSteps = _instance.Config.Steps.Count;
            if (totalSteps == 0) return;

            var stepHeight = 80;
            var radius = 18;
            var iconX = _verticalStepperPanel.Padding.Left + radius;

            for (int i = 0; i < totalSteps; i++)
            {
                var step = _instance.Config.Steps[i];
                var iconY = _verticalStepperPanel.Padding.Top + (stepHeight * i) + radius;

                // Draw connector line (except for last step)
                if (i < totalSteps - 1)
                {
                    var lineStart = new Point(iconX, iconY + radius);
                    var lineEnd = new Point(iconX, iconY + stepHeight - radius);

                    using (var pen = new Pen(i < _instance.CurrentStepIndex ? 
                        (_instance.Config.Theme?.SuccessColor ?? Color.Green) : 
                        Color.FromArgb(200, 200, 200), 2))
                    {
                        g.DrawLine(pen, lineStart, lineEnd);
                    }
                }

                // Draw step indicator
                Helpers.WizardHelpers.DrawStepIndicator(
                    g,
                    new Point(iconX, iconY),
                    radius,
                    i + 1,
                    i == _instance.CurrentStepIndex,
                    i < _instance.CurrentStepIndex,
                    _instance.Config.Theme);

                // Draw step info (title and description)
                var textX = iconX + radius + 20;
                var textY = iconY - radius;
                var textWidth = _verticalStepperPanel.Width - textX - _verticalStepperPanel.Padding.Right;

                using (var titleFont = new Font("Segoe UI", 9.5f, FontStyle.Bold))
                using (var descFont = new Font("Segoe UI", 8f))
                using (var titleBrush = new SolidBrush(i == _instance.CurrentStepIndex ?
                    (_instance.Config.Theme?.ForeColor ?? Color.Black) :
                    Color.FromArgb(150, 150, 150)))
                using (var descBrush = new SolidBrush(Color.FromArgb(120, 120, 120)))
                {
                    // Title
                    g.DrawString(step.Title, titleFont, titleBrush, new RectangleF(textX, textY, textWidth, 20));

                    // Description
                    if (!string.IsNullOrEmpty(step.Description))
                    {
                        g.DrawString(step.Description, descFont, descBrush,
                            new RectangleF(textX, textY + 22, textWidth, 40));
                    }
                }
            }
        }

        protected override void UpdateHeader()
        {
            _verticalStepperPanel.Invalidate();
        }
    }
}
