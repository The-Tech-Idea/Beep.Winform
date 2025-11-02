using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Circle "24 NEWS" badge + green "SPORT NEWS" banner with "LIVE" label
    /// Style: Green circle + white pill with LIVE indicator
    /// Example: "24 | SPORT NEWS"
    /// </summary>
    public class SportNewsCirclePillPainter : BaseButtonPainter
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
            string mainText = parts[1]; // SPORT NEWS

            Color badgeColor = context.SolidBackground != Color.Empty
                ? context.SolidBackground
                : Color.FromArgb(100, 200, 100); // Green

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

            // Draw main pill section (white)
            int pillStart = bounds.X + circleSize + 5;
            int pillWidth = bounds.Width - circleSize - 5;
            Rectangle pillBounds = new Rectangle(pillStart, bounds.Y, pillWidth, bounds.Height);

            int radius = bounds.Height / 2;
            using (GraphicsPath pillPath = new GraphicsPath())
            {
                int diameter = radius * 2;
                pillPath.AddLine(pillBounds.X, pillBounds.Y, pillBounds.Right - radius, pillBounds.Y);
                pillPath.AddArc(pillBounds.Right - diameter, pillBounds.Y, diameter, diameter, 270, 180);
                pillPath.AddLine(pillBounds.Right - radius, pillBounds.Bottom, pillBounds.X, pillBounds.Bottom);
                pillPath.CloseFigure();

                using (Brush pillBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(pillBrush, pillPath);
                }
            }

            // Draw main text
            Rectangle textBounds = new Rectangle(
                pillBounds.X + metrics.PaddingHorizontal + 10,
                pillBounds.Y,
                pillBounds.Width - radius - metrics.PaddingHorizontal - 70,
                pillBounds.Height
            );

            using (Brush textBrush = new SolidBrush(Color.Black))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(mainText, boldFont, textBrush, textBounds, format);
            }

            // Draw LIVE badge
            Rectangle liveBounds = new Rectangle(
                pillBounds.Right - radius - 60,
                pillBounds.Y + (int)(pillBounds.Height * 0.3),
                50,
                (int)(pillBounds.Height * 0.4)
            );

            using (Brush liveBrush = new SolidBrush(badgeColor))
            {
                g.FillRectangle(liveBrush, liveBounds);
            }

            using (Brush liveTextBrush = new SolidBrush(Color.White))
            using (Font liveFont = new Font(context.Font.FontFamily, context.Font.Size * 0.7f, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString("LIVE", liveFont, liveTextBrush, liveBounds, format);
            }
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "24", "SPORT NEWS" };

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
