using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Pink/magenta "FAKE NEWS" banner with yellow chevron edge
    /// Style: Two-color chevron design with contrasting colors
    /// Example: "LIVE NEWS | FAKE NEWS"
    /// </summary>
    public class FakeNewsChevronPainter : BaseButtonPainter
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
            string section1Text = parts[0]; // LIVE NEWS
            string section2Text = parts[1]; // FAKE NEWS

            // Measure section 1 width
            int section1Width = string.IsNullOrEmpty(section1Text) ? 0 : MeasureTextWidth(g, section1Text, context.Font) + 30;
            int chevronWidth = 40;

            // Colors
            Color section1Color = Color.FromArgb(240, 50, 120); // Magenta/pink
            Color section2Color = context.SolidBackground != Color.Empty
                ? context.SolidBackground
                : Color.FromArgb(255, 220, 0); // Yellow

            // Draw section 1 (LIVE NEWS) if exists
            if (section1Width > 0)
            {
                Rectangle section1Bounds = new Rectangle(bounds.X, bounds.Y, section1Width, bounds.Height);
                using (Brush section1Brush = new SolidBrush(section1Color))
                {
                    g.FillRectangle(section1Brush, section1Bounds);
                }

                // Draw section 1 text
                using (Brush text1Brush = new SolidBrush(Color.White))
                using (Font boldFont = new Font(context.Font, FontStyle.Bold))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    g.DrawString(section1Text, boldFont, text1Brush, section1Bounds, format);
                }
            }

            // Draw section 2 (FAKE NEWS) with chevron
            int section2Start = bounds.X + section1Width;
            using (GraphicsPath section2Path = new GraphicsPath())
            {
                int midY = bounds.Y + bounds.Height / 2;

                Point[] points = new Point[]
                {
                    new Point(section2Start, bounds.Y),
                    new Point(bounds.Right - chevronWidth, bounds.Y),
                    new Point(bounds.Right, midY),
                    new Point(bounds.Right - chevronWidth, bounds.Bottom),
                    new Point(section2Start, bounds.Bottom)
                };
                section2Path.AddPolygon(points);

                using (Brush section2Brush = new SolidBrush(section2Color))
                {
                    g.FillPath(section2Brush, section2Path);
                }
            }

            // Draw section 2 text
            Rectangle textBounds = new Rectangle(
                section2Start + metrics.PaddingHorizontal,
                bounds.Y,
                bounds.Right - section2Start - chevronWidth - metrics.PaddingHorizontal * 2,
                bounds.Height
            );

            Color textColor = section2Color.GetBrightness() > 0.5f ? Color.Black : Color.White;
            using (Brush textBrush = new SolidBrush(textColor))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                g.DrawString(section2Text, boldFont, textBrush, textBounds, format);
            }

            // Draw small label below if present
            if (parts.Length > 2 && !string.IsNullOrEmpty(parts[2]))
            {
                Rectangle labelBounds = new Rectangle(
                    section2Start + metrics.PaddingHorizontal,
                    bounds.Y + (int)(bounds.Height * 0.6),
                    textBounds.Width,
                    (int)(bounds.Height * 0.3)
                );

                using (Font smallFont = new Font(context.Font.FontFamily, context.Font.Size * 0.65f))
                using (Brush labelBrush = new SolidBrush(textColor))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    g.DrawString(parts[2], smallFont, labelBrush, labelBounds, format);
                }
            }
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "", "FAKE NEWS", "" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                if (parts.Length >= 2)
                    return new[] { parts[0].Trim(), parts[1].Trim(), parts.Length > 2 ? parts[2].Trim() : "" };
            }

            return new[] { "", text, "" };
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
