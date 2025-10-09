using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// A painter inspired by chat UI bubbles: soft background, rounded accent caption, subtle border.
    /// </summary>
    internal sealed class ChatBubbleFormPainter : IFormPainter, IFormPainterMetricsProvider
    {
        public FormPainterMetrics GetMetrics(BeepiFormPro owner)
        {
            return FormPainterMetrics.DefaultFor(FormStyle.ChatBubble, owner.CurrentTheme);
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // Soft chat canvas background
            using var brush = new SolidBrush(metrics.CaptionColor);
            g.FillRectangle(brush, owner.ClientRectangle);
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var metrics = GetMetrics(owner);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Chat bubble-like rounded bar filling the caption area
            using var path = new GraphicsPath();
            int radius = Math.Min(metrics.BorderRadius, captionRect.Height / 2);
            var r = Rectangle.Inflate(captionRect, -6, -4);
            AddRoundedRect(path, r, radius);

            using var lg = new LinearGradientBrush(r, metrics.CaptionColor, metrics.BorderColor, LinearGradientMode.Vertical);
            g.FillPath(lg, path);

            // Title centered inside the bubble
            var textRect = r;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, metrics.CaptionTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var metrics = GetMetrics(owner);

            // Soft neutral border for the window, matching the chat style
            using var pen = new Pen(metrics.BorderColor, metrics.BorderWidth);
            pen.Alignment = PenAlignment.Inset;
            var r = owner.ClientRectangle;
            r.Width -= 1;
            r.Height -= 1;
            g.DrawRectangle(pen, r);
        }

        private static void AddRoundedRect(GraphicsPath path, Rectangle rect, int radius)
        {
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return;
            }
            int d = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(d, d));

            // top-left
            path.AddArc(arc, 180, 90);

            // top-right
            arc.X = rect.Right - d;
            path.AddArc(arc, 270, 90);

            // bottom-right
            arc.Y = rect.Bottom - d;
            path.AddArc(arc, 0, 90);

            // bottom-left
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
        }
    }
}
