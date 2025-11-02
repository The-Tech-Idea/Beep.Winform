using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Diagonal "24" badge + cyan "WORLD NEWS" hexagon banner
    /// Style: White diagonal with 24 + cyan hexagon shape
    /// Example: "24 | WORLD NEWS"
    /// </summary>
    public class TwentyFourWorldNewsHexagonPainter : BaseButtonPainter
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
            string badgeText = parts[0]; // 24
            string mainText = parts[1]; // WORLD NEWS

            int badgeWidth = bounds.Height;
            int angleWidth = 20;
            int chevronWidth = 25;

            // Draw diagonal badge section (white)
            using (GraphicsPath badgePath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.X + badgeWidth, bounds.Y),
                    new Point(bounds.X + badgeWidth + angleWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                badgePath.AddPolygon(points);

                using (Brush badgeBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(badgeBrush, badgePath);
                }
            }

            // Draw badge text (24)
            Rectangle badgeTextBounds = new Rectangle(
                bounds.X,
                bounds.Y,
                badgeWidth,
                bounds.Height
            );

            using (Brush badgeTextBrush = new SolidBrush(Color.Black))
            using (Font badgeFont = new Font(context.Font.FontFamily, context.Font.Size + 2, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(badgeText, badgeFont, badgeTextBrush, badgeTextBounds, format);
            }

            // Draw main hexagon section (cyan)
            int mainStart = bounds.X + badgeWidth + angleWidth;
            
            Color mainColor = context.SolidBackground != Color.Empty
                ? context.SolidBackground
                : Color.FromArgb(0, 180, 220); // Cyan

            using (GraphicsPath hexagonPath = new GraphicsPath())
            {
                int midY = bounds.Y + bounds.Height / 2;

                Point[] points = new Point[]
                {
                    new Point(mainStart, bounds.Y),
                    new Point(bounds.Right - chevronWidth, bounds.Y),
                    new Point(bounds.Right, midY),
                    new Point(bounds.Right - chevronWidth, bounds.Bottom),
                    new Point(mainStart, bounds.Bottom),
                    new Point(mainStart - angleWidth / 2, midY)
                };
                hexagonPath.AddPolygon(points);

                using (Brush hexagonBrush = new SolidBrush(mainColor))
                {
                    g.FillPath(hexagonBrush, hexagonPath);
                }
            }

            // Draw main text
            Rectangle textBounds = new Rectangle(
                mainStart + 10,
                bounds.Y,
                bounds.Right - mainStart - chevronWidth - 20,
                bounds.Height
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
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "24", "WORLD NEWS" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                return new[] { parts[0].Trim(), parts.Length > 1 ? parts[1].Trim() : "" };
            }

            // Try to extract number
            string[] words = text.Split(' ');
            if (words.Length > 0 && int.TryParse(words[0], out _))
                return new[] { words[0], string.Join(" ", words, 1, words.Length - 1) };

            return new[] { "24", text };
        }
    }
}
