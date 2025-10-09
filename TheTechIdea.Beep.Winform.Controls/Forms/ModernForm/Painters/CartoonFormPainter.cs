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
            // Slightly taller caption with medium button size and a little extra spacing
            return new FormPainterMetrics
            {
                CaptionHeight = 40,
                ButtonWidth = 30,
                ButtonSpacing = 6,
                IconLeftPadding = 10,
                IconSize = 22,
                FontHeightMultiplier = 2.6f,
                // Let extras show to match the playful theme (owner toggles still apply)
                ShowThemeButton = true,
                ShowStyleButton = true,
                ShowSearchButton = false,
                ShowProfileButton = false,
                ShowMailButton = false
            };
        }

        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            // Creamy background reminiscent of paper
            var bg = Color.FromArgb(255, 248, 235);
            using var brush = new SolidBrush(bg);
            g.FillRectangle(brush, owner.ClientRectangle);
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            // Warm yellow/orange caption bar with a subtle darker bottom line
            var cap = Color.FromArgb(255, 224, 102);   // soft warm yellow
            var capDark = Color.FromArgb(233, 185, 75); // slightly darker shadow line

            using var capBrush = new SolidBrush(cap);
            g.FillRectangle(capBrush, captionRect);

            // Decorative bottom line
            using var bottomPen = new Pen(capDark, 2f);
            g.DrawLine(bottomPen, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);

            // Title, centered based on the provided captionRect
            var textColor = Color.FromArgb(50, 50, 50);
            var textRect = Rectangle.Inflate(captionRect, -8, 0);
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            // Bold black outline for a cartoon/comic vibe
            using var pen = new Pen(Color.Black, 2f);
            pen.Alignment = PenAlignment.Inset;
            var r = owner.ClientRectangle;
            r.Width -= 1;
            r.Height -= 1;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawRectangle(pen, r);
        }
    }
}
