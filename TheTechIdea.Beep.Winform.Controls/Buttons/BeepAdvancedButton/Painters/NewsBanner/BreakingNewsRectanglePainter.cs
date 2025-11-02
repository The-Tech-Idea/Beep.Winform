using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Classic breaking news banner: colored rectangle badge + white text section
    /// Style: BREAKING NEWS in colored box + main text in white box
    /// Example: "BREAKING NEWS | TODAY EXCLUSIVE"
    /// </summary>
    public class BreakingNewsRectanglePainter : BaseButtonPainter
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
            string badgeText = parts[0]; // BREAKING NEWS
            string mainText = parts[1]; // Description

            // Measure badge width
            int badgeWidth = MeasureTextWidth(g, badgeText, context.Font) + 30;

            // Colors
            Color badgeColor = context.SolidBackground;
            Color mainColor = context.SecondaryColor != null && context.SecondaryColor != Color.Empty
                ? context.SecondaryColor
                : Color.White;

            // Draw badge section
            Rectangle badgeBounds = new Rectangle(bounds.X, bounds.Y, badgeWidth, bounds.Height);
            using (Brush badgeBrush = new SolidBrush(badgeColor))
            {
                g.FillRectangle(badgeBrush, badgeBounds);
            }

            // Draw main section
            Rectangle mainBounds = new Rectangle(
                bounds.X + badgeWidth,
                bounds.Y,
                bounds.Width - badgeWidth,
                bounds.Height
            );
            using (Brush mainBrush = new SolidBrush(mainColor))
            {
                g.FillRectangle(mainBrush, mainBounds);
            }

            // Draw badge text
            using (Brush badgeTextBrush = new SolidBrush(Color.White))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(badgeText, boldFont, badgeTextBrush, badgeBounds, format);
            }

            // Draw main text
            if (!string.IsNullOrEmpty(mainText))
            {
                Rectangle textBounds = new Rectangle(
                    mainBounds.X + metrics.PaddingHorizontal,
                    mainBounds.Y,
                    mainBounds.Width - metrics.PaddingHorizontal * 2,
                    mainBounds.Height
                );

                Color textColor = mainColor == Color.White ? badgeColor : Color.White;
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

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "BREAKING NEWS", "" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                return new[] { parts[0].Trim(), parts.Length > 1 ? parts[1].Trim() : "" };
            }

            if (text.Contains(" - "))
            {
                string[] parts = text.Split(new[] { " - " }, System.StringSplitOptions.None);
                return new[] { parts[0].Trim(), parts.Length > 1 ? parts[1].Trim() : "" };
            }

            // Look for "BREAKING NEWS" in text
            int index = text.IndexOf("NEWS", System.StringComparison.OrdinalIgnoreCase);
            if (index > 0)
            {
                int endIndex = index + 4;
                return new[] { text.Substring(0, endIndex).Trim(), text.Substring(endIndex).Trim() };
            }

            return new[] { text, "" };
        }

        private int MeasureTextWidth(Graphics g, string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            SizeF size = g.MeasureString(text, new Font(font, FontStyle.Bold));
            return (int)size.Width;
        }
    }
}
