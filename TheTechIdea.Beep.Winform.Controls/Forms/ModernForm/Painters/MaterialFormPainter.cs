using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Material Design 3 form painter with vertical accent bar.
    /// Features a 4px vertical primary color bar on the left side of the caption.
    /// </summary>
    internal sealed class MaterialFormPainter : IFormPainter, IFormPainterMetricsProvider
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.Material, owner.CurrentTheme);
        }

        /// <summary>
        /// Paints a solid background using Material3 style background color.
        /// </summary>
        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);
            using var brush = new SolidBrush(metrics.CaptionColor);
            g.FillRectangle(brush, owner.ClientRectangle);
        }

        /// <summary>
        /// Paints the caption bar with Material Design 3 style vertical accent bar.
        /// Features a 4px primary color bar on the left edge of the caption.
        /// </summary>
        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            var primary = metrics.BorderColor;

            // Draw 4px vertical accent bar on the left
            using var brush = new SolidBrush(primary);
            var bar = new Rectangle(captionRect.Left, captionRect.Top, 4, captionRect.Height);
            g.FillRectangle(brush, bar);

            // Draw title text with 12px padding
            var textRect = captionRect;
            textRect.Inflate(-12, 0);
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        /// <summary>
        /// Paints a 1px border around the form using Material3 border color.
        /// </summary>
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
