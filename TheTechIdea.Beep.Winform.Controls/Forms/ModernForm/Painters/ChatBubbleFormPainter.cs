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
            return new FormPainterMetrics
            {
                CaptionHeight = 44,    // slightly larger to resemble a bubble header
                ButtonWidth = 30,
                ButtonSpacing = 6,
                IconLeftPadding = 10,
                IconSize = 22,
                FontHeightMultiplier = 2.6f,
                // Keep extras minimal, but allow MenuStyle as a playful toggle
                ShowThemeButton = true,
                ShowStyleButton = false,
                ShowSearchButton = false,
                ShowProfileButton = false,
                ShowMailButton = false
            };
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            // Soft chat canvas background
            var bg = Color.FromArgb(250, 247, 244); // off-white with a hint of warmth
            using var brush = new SolidBrush(bg);
            g.FillRectangle(brush, owner.ClientRectangle);
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Chat bubble-like rounded bar filling the caption area
            var accent = Color.FromArgb(84, 180, 235);     // friendly blue (similar to right-side bubbles)
            var accentDark = Color.FromArgb(56, 152, 213); // darker bottom edge

            using var path = new GraphicsPath();
            int radius = Math.Min(12, captionRect.Height / 2);
            var r = Rectangle.Inflate(captionRect, -6, -4);
            AddRoundedRect(path, r, radius);

            using var lg = new LinearGradientBrush(r, accent, accentDark, LinearGradientMode.Vertical);
            g.FillPath(lg, path);

            // Title centered inside the bubble
            var textRect = r;
            var textColor = Color.White;
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            // Soft neutral border for the window, matching the chat style
            using var pen = new Pen(Color.FromArgb(180, 172, 165), 1f);
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
