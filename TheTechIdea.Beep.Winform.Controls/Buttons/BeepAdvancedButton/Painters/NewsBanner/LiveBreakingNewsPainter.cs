using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Two-section banner: LIVE badge + BREAKING NEWS main section with angled divider
    /// Style: Pink/magenta LIVE section + white BREAKING NEWS section with diagonal separation
    /// Example: "LIVE | BREAKING NEWS - TODAY EXCLUSIVE"
    /// </summary>
    public class LiveBreakingNewsPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            // Split text into sections
            string[] parts = SplitText(context.Text);
            string section1Text = parts[0]; // LIVE
            string section2Text = parts[1]; // BREAKING NEWS

            // Measure section 1 width
            int section1Width = MeasureTextWidth(g, section1Text, context.Font) + 30;
            int angleWidth = 25; // Width of diagonal transition

            // Colors
            Color section1Color = context.IconBackgroundColor != Color.Empty
                ? context.IconBackgroundColor
                : Color.FromArgb(255, 0, 120); // Magenta/pink

            Color section2Color = context.SolidBackground;

            // Draw section 1 (LIVE) with angled right edge
            using (GraphicsPath section1Path = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.X + section1Width, bounds.Y),
                    new Point(bounds.X + section1Width + angleWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                section1Path.AddPolygon(points);

                using (Brush section1Brush = new SolidBrush(section1Color))
                {
                    g.FillPath(section1Brush, section1Path);
                }
            }

            // Draw section 2 (BREAKING NEWS)
            Rectangle section2Bounds = new Rectangle(
                bounds.X + section1Width + angleWidth - 5,
                bounds.Y,
                bounds.Width - section1Width - angleWidth + 5,
                bounds.Height
            );

            using (Brush section2Brush = new SolidBrush(section2Color))
            {
                g.FillRectangle(section2Brush, section2Bounds);
            }

            // Draw section 1 text
            Rectangle section1TextBounds = new Rectangle(
                bounds.X + 10,
                bounds.Y,
                section1Width - 10,
                bounds.Height
            );

            using (Brush text1Brush = new SolidBrush(Color.White))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(section1Text, boldFont, text1Brush, section1TextBounds, format);
            }

            // Draw section 2 text
            Rectangle section2TextBounds = new Rectangle(
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
                format.Trimming = StringTrimming.EllipsisCharacter;
                g.DrawString(section2Text, boldFont, text2Brush, section2TextBounds, format);
            }
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "", "" };

            // Split on | or -
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

            // Try to split on LIVE, BREAKING, etc.
            string[] keywords = { "LIVE", "BREAKING", "NEWS" };
            foreach (string keyword in keywords)
            {
                int index = text.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase);
                if (index > 0)
                    return new[] { text.Substring(0, index).Trim(), text.Substring(index).Trim() };
            }

            // Default: first word vs rest
            int spaceIndex = text.IndexOf(' ');
            if (spaceIndex > 0)
                return new[] { text.Substring(0, spaceIndex), text.Substring(spaceIndex + 1).Trim() };

            return new[] { text, "" };
        }

        private int MeasureTextWidth(Graphics g, string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return 0;
            SizeF size = g.MeasureString(text, font);
            return (int)size.Width;
        }
    }
}
