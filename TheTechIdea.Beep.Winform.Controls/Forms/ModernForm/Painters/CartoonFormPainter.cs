using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// A playful, cartoon-inspired painter with a warm caption bar and bold outline.
    /// </summary>
    internal sealed class CartoonFormPainter : IFormPainter, IFormPainterMetricsProvider
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Cartoon, owner.CurrentTheme);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            using var brush = new SolidBrush(metrics.CaptionColor);
            g.FillRectangle(brush, owner.ClientRectangle);
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            var capDark = metrics.BorderColor;

            using var capBrush = new SolidBrush(metrics.CaptionColor);
            g.FillRectangle(capBrush, captionRect);

            // Decorative bottom line
            using var bottomPen = new Pen(capDark, 2f);
            g.DrawLine(bottomPen, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);

            // Title, centered based on the provided captionRect
            var textRect = Rectangle.Inflate(captionRect, -8, 0);
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            using var pen = new Pen(metrics.BorderColor, metrics.BorderWidth);
            pen.Alignment = PenAlignment.Inset;
            var r = owner.ClientRectangle;
            r.Width -= 1;
            r.Height -= 1;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawRectangle(pen, r);
        }
    }
}
