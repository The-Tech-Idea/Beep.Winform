using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// macOS-style form painter with traffic light buttons and subtle borders.
    /// </summary>
    internal sealed class MacOSFormPainter : IFormPainter, IFormPainterMetricsProvider
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.MacOS, owner.CurrentTheme);
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
            var trafficLightRed = Color.FromArgb(255, 88, 85);
            var trafficLightYellow = Color.FromArgb(255, 189, 46);
            var trafficLightGreen = Color.FromArgb(39, 201, 63);

            // Draw traffic light buttons
            var buttonSize = 12;
            var spacing = 6;
            var topOffset = (captionRect.Height - buttonSize) / 2;

            using var redBrush = new SolidBrush(trafficLightRed);
            using var yellowBrush = new SolidBrush(trafficLightYellow);
            using var greenBrush = new SolidBrush(trafficLightGreen);

            g.FillEllipse(redBrush, captionRect.Left + spacing, captionRect.Top + topOffset, buttonSize, buttonSize);
            g.FillEllipse(yellowBrush, captionRect.Left + spacing * 2 + buttonSize, captionRect.Top + topOffset, buttonSize, buttonSize);
            g.FillEllipse(greenBrush, captionRect.Left + spacing * 3 + buttonSize * 2, captionRect.Top + topOffset, buttonSize, buttonSize);

            // Draw title text
            var textRect = captionRect;
            textRect.X += spacing * 4 + buttonSize * 3;
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