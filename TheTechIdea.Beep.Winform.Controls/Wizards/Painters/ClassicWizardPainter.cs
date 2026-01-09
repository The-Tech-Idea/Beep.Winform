using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Wizards;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Painters
{
    /// <summary>
    /// Painter for ClassicWizardForm
    /// Traditional wizard with left sidebar
    /// </summary>
    public class ClassicWizardPainter : WizardPainterBase
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
    }
}
