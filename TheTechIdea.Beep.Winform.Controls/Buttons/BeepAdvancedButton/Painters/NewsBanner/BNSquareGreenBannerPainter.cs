using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// "BN" square badge + green banner with LIVE indicator
    /// Style: Black square with BN + green colored section + LIVE badge
    /// Example: "BN | BREAKING NEWS"
    /// </summary>
    public class BNSquareGreenBannerPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            // Split text
            string[] parts = SplitText(context.Text);
            string badgeText = parts[0]; // BN
            string mainText = parts[1]; // BREAKING NEWS

            // Square badge
            int squareSize = (int)(bounds.Height * 0.85);
            Rectangle squareBounds = new Rectangle(
                bounds.X + 5,
                bounds.Y + (bounds.Height - squareSize) / 2,
                squareSize,
                squareSize
            );

            Color squareBgColor = Color.FromArgb(30, 30, 30); // Dark

            using (Brush squareBrush = new SolidBrush(squareBgColor))
            {
                g.FillRectangle(squareBrush, squareBounds);
            }

            // Draw badge text
            using (Brush badgeTextBrush = new SolidBrush(Color.White))
            using (Font badgeFont = new Font(context.Font.FontFamily, context.Font.Size, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(badgeText, badgeFont, badgeTextBrush, squareBounds, format);
            }

            // Draw main banner section (green)
            int bannerStart = bounds.X + squareSize + 15;
            Rectangle bannerBounds = new Rectangle(
                bannerStart,
                bounds.Y,
                bounds.Width - squareSize - 15,
                bounds.Height
            );

            Color bannerColor = context.SolidBackground != Color.Empty
                ? context.SolidBackground
                : Color.FromArgb(100, 200, 100); // Green

            using (Brush bannerBrush = new SolidBrush(bannerColor))
            {
                g.FillRectangle(bannerBrush, bannerBounds);
            }

            // Draw main text
            Rectangle textBounds = new Rectangle(
                bannerBounds.X + metrics.PaddingHorizontal,
                bannerBounds.Y,
                bannerBounds.Width - metrics.PaddingHorizontal * 2,
                bannerBounds.Height
            );

            using (Brush textBrush = new SolidBrush(Color.White))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                g.DrawString(mainText, boldFont, textBrush, textBounds, format);
            }

            // Draw LIVE badge
            Rectangle liveBounds = new Rectangle(
                bannerBounds.X + metrics.PaddingHorizontal,
                bannerBounds.Y + (int)(bannerBounds.Height * 0.58),
                55,
                (int)(bannerBounds.Height * 0.35)
            );

            using (Brush liveBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(liveBrush, liveBounds);
            }

            using (Brush liveTextBrush = new SolidBrush(bannerColor))
            using (Font liveFont = new Font(context.Font.FontFamily, context.Font.Size * 0.6f, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString("● LIVE", liveFont, liveTextBrush, liveBounds, format);
            }
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "BN", "BREAKING NEWS" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                return new[] { parts[0].Trim(), parts.Length > 1 ? parts[1].Trim() : "" };
            }

            return new[] { "BN", text };
        }
    }
}
