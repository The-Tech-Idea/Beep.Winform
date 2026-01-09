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
    /// Painter for VerticalStepperWizardForm
    /// Vertical timeline stepper on the left
    /// </summary>
    public class VerticalStepperWizardPainter : WizardPainterBase
    {
        public override void PaintHeader(Graphics g, Rectangle bounds)
        {
            if (Owner == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw background
            using (var bgBrush = new SolidBrush(GetBackColor()))
            {
                g.FillRectangle(bgBrush, bounds);
            }
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

            var stepHeight = 80;
            var radius = 18;
            var iconX = bounds.Left + 30 + radius;

            for (int i = 0; i < totalSteps; i++)
            {
                var step = Instance.Config.Steps[i];
                var iconY = bounds.Top + 40 + (stepHeight * i) + radius;

                // Draw connector line (except for last step)
                if (i < totalSteps - 1)
                {
                    var lineStart = new Point(iconX, iconY + radius);
                    var lineEnd = new Point(iconX, iconY + stepHeight - radius);

                    using (var pen = new Pen(i < Instance.CurrentStepIndex ?
                        (Theme?.SuccessColor ?? Color.Green) :
                        Color.FromArgb(200, 200, 200), 2))
                    {
                        g.DrawLine(pen, lineStart, lineEnd);
                    }
                }

                // Draw step indicator
                WizardHelpers.DrawStepIndicator(
                    g,
                    new Point(iconX, iconY),
                    radius,
                    i + 1,
                    i == Instance.CurrentStepIndex,
                    i < Instance.CurrentStepIndex,
                    Theme);
            }
        }
    }
}
