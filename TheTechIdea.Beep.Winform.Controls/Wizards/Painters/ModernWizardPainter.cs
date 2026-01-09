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
    /// Painter for ModernWizardForm
    /// Clean modern design with top progress bar
    /// </summary>
    public class ModernWizardPainter : WizardPainterBase
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

            // Draw title and description (handled by BeepLabel controls)
            // Progress bar is handled by BeepProgressBar control
        }

        public override void PaintContent(Graphics g, Rectangle bounds)
        {
            if (Owner == null) return;

            // Content is handled by step UserControls
            // This method can be used for additional background effects
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

        public override void PaintProgress(Graphics g, Rectangle bounds, int currentStep, int totalSteps)
        {
            if (totalSteps <= 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            var progress = (float)currentStep / totalSteps;
            var accentColor = GetAccentColor();

            // Background
            using (var backBrush = new SolidBrush(Color.FromArgb(40, accentColor)))
            {
                g.FillRectangle(backBrush, bounds);
            }

            // Progress fill
            var fillWidth = (int)(bounds.Width * progress);
            var fillRect = new Rectangle(bounds.X, bounds.Y, fillWidth, bounds.Height);
            using (var fillBrush = new SolidBrush(accentColor))
            {
                g.FillRectangle(fillBrush, fillRect);
            }
        }
    }
}
