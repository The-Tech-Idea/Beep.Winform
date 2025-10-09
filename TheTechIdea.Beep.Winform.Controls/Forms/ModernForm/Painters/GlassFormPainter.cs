using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Glass-effect form painter with transparency and gradient overlays.
    /// Features semi-transparent background with subtle gradient and glass caption bar.
    /// </summary>
    internal sealed class GlassFormPainter : IFormPainter, IFormPainterMetricsProvider
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Glass, owner.CurrentTheme);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var surface = Color.FromArgb(240, metrics.CaptionColor.R, metrics.CaptionColor.G, metrics.CaptionColor.B);

            using var brush = new SolidBrush(surface);
            g.FillRectangle(brush, owner.ClientRectangle);

            using var gradBrush = new LinearGradientBrush(
                owner.ClientRectangle,
                Color.FromArgb(30, 255, 255, 255),
                Color.FromArgb(5, 255, 255, 255),
                90f);
            g.FillRectangle(gradBrush, owner.ClientRectangle);
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // Top highlight line for glass effect
            using var highlight = new Pen(Color.FromArgb(100, 255, 255, 255), 1f);
            g.DrawLine(highlight, captionRect.Left, captionRect.Top, captionRect.Right, captionRect.Top);

            // Bottom border line with primary color
            using var border = new Pen(metrics.BorderColor, 1f);
            g.DrawLine(border, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // Outer border with primary color
            using var pen = new Pen(metrics.BorderColor, metrics.BorderWidth);
            var r = owner.ClientRectangle;
            r.Width -= 2;
            r.Height -= 2;
            r.Inflate(1, 1);
            g.DrawRectangle(pen, r);

            // Inner glass highlight
            using var innerPen = new Pen(Color.FromArgb(80, 255, 255, 255), 1f);
            var innerRect = owner.ClientRectangle;
            innerRect.Inflate(-2, -2);
            g.DrawRectangle(innerPen, innerRect);
        }
    }
}
