using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Yellow/white "BREAKING NEWS" with globe icon background
    /// Style: Yellow "BREAKING" + blue "NEWS" sections with globe icon
    /// Example: "BREAKING | NEWS"
    /// </summary>
    public class BreakingNewsGlobePainter : BaseButtonPainter
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
            string section1Text = parts[0]; // BREAKING
            string section2Text = parts[1]; // NEWS

            // Draw globe icon section (left)
            int globeWidth = bounds.Height;
            Rectangle globeBounds = new Rectangle(bounds.X, bounds.Y, globeWidth, bounds.Height);

            Color globeColor = context.IconBackgroundColor != Color.Empty
                ? context.IconBackgroundColor
                : Color.FromArgb(255, 220, 0); // Yellow

            using (Brush globeBrush = new SolidBrush(globeColor))
            {
                g.FillRectangle(globeBrush, globeBounds);
            }

            // Draw globe icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    globeBounds.X + (globeBounds.Width - metrics.IconSize) / 2,
                    globeBounds.Y + (globeBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Measure section widths
            int section1Width = MeasureTextWidth(g, section1Text, context.Font) + 20;
            
            // Draw section 1 (BREAKING) - white
            Rectangle section1Bounds = new Rectangle(
                bounds.X + globeWidth,
                bounds.Y,
                section1Width,
                bounds.Height
            );

            using (Brush section1Brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(section1Brush, section1Bounds);
            }

            // Draw section 2 (NEWS) - blue
            Rectangle section2Bounds = new Rectangle(
                bounds.X + globeWidth + section1Width,
                bounds.Y,
                bounds.Width - globeWidth - section1Width,
                bounds.Height
            );

            Color section2Color = context.SolidBackground != Color.Empty
                ? context.SolidBackground
                : Color.FromArgb(0, 120, 200); // Blue

            using (Brush section2Brush = new SolidBrush(section2Color))
            {
                g.FillRectangle(section2Brush, section2Bounds);
            }

            // Draw section 1 text (BREAKING)
            using (Brush text1Brush = new SolidBrush(Color.Black))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(section1Text, boldFont, text1Brush, section1Bounds, format);
            }

            // Draw section 2 text (NEWS)
            Rectangle textBounds = new Rectangle(
                section2Bounds.X + metrics.PaddingHorizontal,
                section2Bounds.Y,
                section2Bounds.Width - metrics.PaddingHorizontal * 2,
                section2Bounds.Height
            );

            using (Brush text2Brush = new SolidBrush(Color.White))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(section2Text, boldFont, text2Brush, textBounds, format);
            }
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "BREAKING", "NEWS" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                return new[] { parts[0].Trim(), parts.Length > 1 ? parts[1].Trim() : "" };
            }

            // Split "BREAKING NEWS"
            if (text.ToUpper().Contains("BREAKING"))
            {
                return new[] { "BREAKING", text.Replace("BREAKING", "").Trim() };
            }

            return new[] { text, "NEWS" };
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
