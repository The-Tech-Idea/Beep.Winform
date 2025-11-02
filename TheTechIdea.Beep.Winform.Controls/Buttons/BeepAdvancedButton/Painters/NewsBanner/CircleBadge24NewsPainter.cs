using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Circle badge with "24" or number on left + colored text banner
    /// Style: White text banner with colored circular badge overlapping left side
    /// Example: "24 NEWS" or "24 BREAKING NEWS"
    /// </summary>
    public class CircleBadge24NewsPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            // Configuration
            float circleScale = 1.3f; // Circle is 130% of banner height
            int circleOverlap = 10; // Pixels the circle overlaps the banner
            int circleBorderWidth = 3;

            // Calculate circle size and position
            int circleSize = (int)(bounds.Height * circleScale);
            Rectangle circleBounds = new Rectangle(
                bounds.X - circleOverlap,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize,
                circleSize
            );

            // Badge color (from context or default)
            Color badgeColor = context.IconBackgroundColor != Color.Empty
                ? context.IconBackgroundColor
                : context.SolidBackground;

            // Draw main white banner
            int mainStartX = bounds.X + circleSize - circleOverlap - 10;
            Rectangle mainBounds = new Rectangle(
                mainStartX,
                bounds.Y,
                bounds.Width - (mainStartX - bounds.X),
                bounds.Height
            );

            using (Brush mainBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(mainBrush, mainBounds);
            }

            // Draw circle badge
            using (Brush circleBrush = new SolidBrush(badgeColor))
            {
                g.FillEllipse(circleBrush, circleBounds);
            }

            // Draw circle border
            using (Pen circlePen = new Pen(Color.White, circleBorderWidth))
            {
                g.DrawEllipse(circlePen, circleBounds);
            }

            // Extract and draw badge text (24, etc.)
            string badgeText = ExtractBadgeNumber(context.Text);
            if (!string.IsNullOrEmpty(badgeText))
            {
                using (Font badgeFont = new Font(context.Font.FontFamily, context.Font.Size + 4, FontStyle.Bold))
                using (Brush badgeTextBrush = new SolidBrush(Color.White))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    g.DrawString(badgeText, badgeFont, badgeTextBrush, circleBounds, format);
                }
            }

            // Draw main text (everything after the badge number)
            string mainText = RemoveBadgeNumber(context.Text);
            if (!string.IsNullOrEmpty(mainText))
            {
                Rectangle textBounds = new Rectangle(
                    mainBounds.X + metrics.PaddingHorizontal,
                    mainBounds.Y,
                    mainBounds.Width - metrics.PaddingHorizontal * 2,
                    mainBounds.Height
                );

                Color textColor = badgeColor == Color.White ? Color.Black : badgeColor;
                using (Brush textBrush = new SolidBrush(textColor))
                using (Font boldFont = new Font(context.Font, FontStyle.Bold))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    g.DrawString(mainText, boldFont, textBrush, textBounds, format);
                }
            }
        }

        private string ExtractBadgeNumber(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // Look for numbers at the start
            string[] parts = text.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0 && int.TryParse(parts[0], out _))
                return parts[0];

            return "";
        }

        private string RemoveBadgeNumber(string text)
        {
            string badge = ExtractBadgeNumber(text);
            if (string.IsNullOrEmpty(badge))
                return text;

            return text.Substring(text.IndexOf(badge) + badge.Length).Trim();
        }
    }
}
