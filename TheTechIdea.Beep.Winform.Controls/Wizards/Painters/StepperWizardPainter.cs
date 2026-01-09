using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Controls.Wizards.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Painters
{
    /// <summary>
    /// Painter for StepperWizardForm
    /// Horizontal stepper with numbered steps
    /// </summary>
    public class StepperWizardPainter : WizardPainterBase
    {
        public override void PaintHeader(Graphics g, Rectangle bounds)
        {
            if (Instance == null || Owner == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw background
            using (var bgBrush = new SolidBrush(GetBackColor()))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Step indicators are painted in the stepper panel
        }

        public override void PaintContent(Graphics g, Rectangle bounds)
        {
            if (Owner == null) return;

            // Content is handled by step UserControls
            using (var bgBrush = new SolidBrush(GetBackColor()))
            {
                g.FillRectangle(bgBrush, bounds);
            }
        }

        public override void PaintFooter(Graphics g, Rectangle bounds)
        {
            if (Owner == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw background
            using (var bgBrush = new SolidBrush(Theme?.PanelBackColor ?? Color.FromArgb(250, 250, 250)))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Draw top border
            using (var borderPen = new Pen(GetBorderColor(), 1))
            {
                g.DrawLine(borderPen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
            }
        }

        public override void PaintStepIndicators(Graphics g, Rectangle bounds)
        {
            if (Instance == null) return;

            var totalSteps = Instance.Config.Steps.Count;
            if (totalSteps == 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            var stepWidth = bounds.Width / totalSteps;
            var centerY = bounds.Height / 2;
            var radius = 20;

            for (int i = 0; i < totalSteps; i++)
            {
                var step = Instance.Config.Steps[i];
                var centerX = bounds.Left + (stepWidth * i) + (stepWidth / 2);

                // Draw connector line (except for first step)
                if (i > 0)
                {
                    var prevCenterX = bounds.Left + (stepWidth * (i - 1)) + (stepWidth / 2);
                    var lineStart = new Point(prevCenterX + radius, centerY);
                    var lineEnd = new Point(centerX - radius, centerY);

                    WizardHelpers.DrawConnectorLine(
                        g,
                        lineStart,
                        lineEnd,
                        i <= Instance.CurrentStepIndex,
                        Theme);
                }

                // Draw step indicator
                WizardHelpers.DrawStepIndicator(
                    g,
                    new Point(centerX, centerY),
                    radius,
                    i + 1,
                    i == Instance.CurrentStepIndex,
                    i < Instance.CurrentStepIndex,
                    Theme);
            }
        }
    }
}
