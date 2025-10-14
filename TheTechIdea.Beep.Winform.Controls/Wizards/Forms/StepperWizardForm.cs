using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Horizontal stepper wizard form (Image 3 style)
    /// Shows all steps in a horizontal line with connectors
    /// </summary>
    public class StepperWizardForm : BaseWizardForm
    {
        private Panel _stepperPanel;

        public StepperWizardForm(WizardInstance instance) : base(instance)
        {
        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // Stepper panel at top
            _stepperPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.White,
                Padding = new Padding(40, 20, 40, 20)
            };

            _stepperPanel.Paint += StepperPanel_Paint;

            _headerPanel.Height = 100;
            _headerPanel.Controls.Clear();
            _headerPanel.Controls.Add(_stepperPanel);
        }

        private void StepperPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var totalSteps = _instance.Config.Steps.Count;
            if (totalSteps == 0) return;

            var stepWidth = (_stepperPanel.Width - _stepperPanel.Padding.Horizontal) / totalSteps;
            var centerY = _stepperPanel.Height / 2;
            var radius = 20;

            for (int i = 0; i < totalSteps; i++)
            {
                var step = _instance.Config.Steps[i];
                var centerX = _stepperPanel.Padding.Left + (stepWidth * i) + (stepWidth / 2);

                // Draw connector line (except for first step)
                if (i > 0)
                {
                    var prevCenterX = _stepperPanel.Padding.Left + (stepWidth * (i - 1)) + (stepWidth / 2);
                    var lineStart = new Point(prevCenterX + radius, centerY);
                    var lineEnd = new Point(centerX - radius, centerY);
                    
                    WizardHelpers.DrawConnectorLine(
                        g,
                        lineStart,
                        lineEnd,
                        i <= _instance.CurrentStepIndex,
                        _instance.Config.Theme);
                }

                // Draw step indicator
                WizardHelpers.DrawStepIndicator(
                    g,
                    new Point(centerX, centerY),
                    radius,
                    i + 1,
                    i == _instance.CurrentStepIndex,
                    i < _instance.CurrentStepIndex,
                    _instance.Config.Theme);

                // Draw step title below circle
                using (var font = new Font("Segoe UI", 8f))
                using (var brush = new SolidBrush(i == _instance.CurrentStepIndex ? 
                    (_instance.Config.Theme?.AccentColor ?? Color.Black) : 
                    Color.FromArgb(100, 100, 100)))
                {
                    var titleSize = TextUtils.MeasureText(g,step.Title, font);
                    var titleX = centerX - titleSize.Width / 2;
                    var titleY = centerY + radius + 10;

                    g.DrawString(step.Title, font, brush, titleX, titleY);
                }
            }
        }

        protected override void UpdateHeader()
        {
            _stepperPanel.Invalidate();
        }
    }
}
