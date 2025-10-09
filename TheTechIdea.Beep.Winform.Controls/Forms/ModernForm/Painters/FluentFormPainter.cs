using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Fluent Design form painter with acrylic background and shadow effects.
    /// </summary>
    internal sealed class FluentFormPainter : IFormPainter, IFormPainterMetricsProvider
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Fluent, owner.CurrentTheme);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            var acrylicOverlay = Color.FromArgb(128, 255, 255, 255);

            using var bgBrush = new SolidBrush(metrics.CaptionColor);
            g.FillRectangle(bgBrush, owner.ClientRectangle);

            using var overlayBrush = new SolidBrush(acrylicOverlay);
            g.FillRectangle(overlayBrush, owner.ClientRectangle);
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);

            // Draw accent line at the bottom of the caption
            using var accentPen = new Pen(metrics.BorderColor, 2f);
            g.DrawLine(accentPen, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);

            // Draw title text
            var textRect = captionRect;
            textRect.Inflate(-8, 0);
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            using var pen = new Pen(metrics.BorderColor, metrics.BorderWidth);
            var r = owner.ClientRectangle;
            r.Width -= 1;
            r.Height -= 1;
            g.DrawRectangle(pen, r);
        }
    }
}