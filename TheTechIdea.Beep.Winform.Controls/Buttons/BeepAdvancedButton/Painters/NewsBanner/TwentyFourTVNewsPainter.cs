using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Circle badge "24 NEWS" on left + "TV NEWS" label on right
    /// Style: Orange circle with 24 + orange TV NEWS pill
    /// Example: "24 | TV NEWS"
    /// </summary>
    public class TwentyFourTVNewsPainter : BaseButtonPainter
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
            string mainText = parts[1]; // TV NEWS
            string subText = parts[2]; // LOREM IPSUM DOLOR SIT AMET

            Color badgeColor = context.SolidBackground;

            // Draw circle badge
            int circleSize = (int)(bounds.Height * 1.1);
            Rectangle circleBounds = new Rectangle(
                bounds.X,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize,
                circleSize
            );

            using (Brush circleBrush = new SolidBrush(badgeColor))
            {
                g.FillEllipse(circleBrush, circleBounds);
            }

            // Draw badge text
            using (Brush badgeTextBrush = new SolidBrush(Color.White))
            using (Font badgeFont = new Font(context.Font.FontFamily, context.Font.Size + 2, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(badgeText, badgeFont, badgeTextBrush, circleBounds, format);
            }

            // Draw main pill section
            int pillStart = bounds.X + circleSize + 10;
            int pillWidth = bounds.Width - circleSize - 10;
            Rectangle pillBounds = new Rectangle(pillStart, bounds.Y, pillWidth, bounds.Height);

            int radius = bounds.Height / 2;
            using (GraphicsPath pillPath = new GraphicsPath())
            {
                int diameter = radius * 2;
                pillPath.AddArc(pillBounds.X, pillBounds.Y, diameter, diameter, 90, 180);
                pillPath.AddLine(pillBounds.X + radius, pillBounds.Y, pillBounds.Right - radius, pillBounds.Y);
                pillPath.AddArc(pillBounds.Right - diameter, pillBounds.Y, diameter, diameter, 270, 180);
                pillPath.AddLine(pillBounds.Right - radius, pillBounds.Bottom, pillBounds.X + radius, pillBounds.Bottom);
                pillPath.CloseFigure();

                using (Brush pillBrush = new SolidBrush(badgeColor))
                {
                    g.FillPath(pillBrush, pillPath);
                }
            }

            // Draw main text
            Rectangle mainTextBounds = new Rectangle(
                pillBounds.X + radius / 2 + 10,
                pillBounds.Y,
                pillBounds.Width - radius - 20,
                (int)(pillBounds.Height * 0.55)
            );

            using (Brush textBrush = new SolidBrush(Color.White))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(mainText, boldFont, textBrush, mainTextBounds, format);
            }

            // Draw sub text if present
            if (!string.IsNullOrEmpty(subText))
            {
                Rectangle subTextBounds = new Rectangle(
                    mainTextBounds.X,
                    pillBounds.Y + (int)(pillBounds.Height * 0.5),
                    mainTextBounds.Width,
                    (int)(pillBounds.Height * 0.45)
                );

                using (Font smallFont = new Font(context.Font.FontFamily, context.Font.Size * 0.7f))
                using (Brush subTextBrush = new SolidBrush(Color.White))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    g.DrawString(subText, smallFont, subTextBrush, subTextBounds, format);
                }
            }
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "24", "TV NEWS", "" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                return new[] 
                { 
                    parts.Length > 0 ? parts[0].Trim() : "24",
                    parts.Length > 1 ? parts[1].Trim() : "TV NEWS",
                    parts.Length > 2 ? parts[2].Trim() : ""
                };
            }

            // Try to extract "24" or number
            string[] words = text.Split(' ');
            if (words.Length > 0 && int.TryParse(words[0], out _))
                return new[] { words[0], string.Join(" ", words, 1, words.Length - 1), "" };

            return new[] { "24", text, "" };
        }
    }
}
