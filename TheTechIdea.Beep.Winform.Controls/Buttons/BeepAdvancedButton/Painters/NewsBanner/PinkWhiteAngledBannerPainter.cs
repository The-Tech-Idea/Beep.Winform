using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Two-section banner with angled pink/white sections
    /// Style: Pink "BREAKING NEWS" section + white main section with angled divider
    /// Example: "BREAKING NEWS | Lorem ipsum dolor sit amet"
    /// </summary>
    public class PinkWhiteAngledBannerPainter : BaseButtonPainter
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
            string section1Text = parts[0]; // BREAKING NEWS
            string section2Text = parts[1]; // Lorem ipsum

            // Measure section 1
            int section1Width = MeasureTextWidth(g, section1Text, context.Font) + 30;
            int angleWidth = 30;

            Color section1Color = context.SolidBackground != Color.Empty
                ? context.SolidBackground
                : Color.FromArgb(240, 50, 120); // Pink/magenta

            // Draw section 1 (pink) with angled edge
            using (GraphicsPath section1Path = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.X + section1Width, bounds.Y),
                    new Point(bounds.X + section1Width - angleWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                section1Path.AddPolygon(points);

                using (Brush section1Brush = new SolidBrush(section1Color))
                {
                    g.FillPath(section1Brush, section1Path);
                }
            }

            // Draw section 2 (white)
            Rectangle section2Bounds = new Rectangle(
                bounds.X + section1Width - angleWidth - 5,
                bounds.Y,
                bounds.Width - section1Width + angleWidth + 5,
                bounds.Height
            );

            using (Brush section2Brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(section2Brush, section2Bounds);
            }

            // Draw section 1 text
            Rectangle section1TextBounds = new Rectangle(
                bounds.X + 10,
                bounds.Y,
                section1Width - angleWidth - 10,
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

            // Draw "LIVE" badge if present
            float liveHeight = bounds.Height * 0.35f;
            Rectangle liveBounds = new Rectangle(
                bounds.X + 10,
                bounds.Y + (int)(bounds.Height * 0.62),
                60,
                (int)liveHeight
            );

            using (Brush liveBgBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(liveBgBrush, liveBounds);
            }

            using (Brush liveTextBrush = new SolidBrush(section1Color))
            using (Font liveFont = new Font(context.Font.FontFamily, context.Font.Size * 0.65f, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString("● LIVE", liveFont, liveTextBrush, liveBounds, format);
            }

            // Draw section 2 text
            if (!string.IsNullOrEmpty(section2Text))
            {
                Rectangle textBounds = new Rectangle(
                    section2Bounds.X + metrics.PaddingHorizontal,
                    section2Bounds.Y,
                    section2Bounds.Width - metrics.PaddingHorizontal * 2,
                    section2Bounds.Height
                );

                using (Brush textBrush = new SolidBrush(Color.FromArgb(100, 100, 100)))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    g.DrawString(section2Text, context.Font, textBrush, textBounds, format);
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
