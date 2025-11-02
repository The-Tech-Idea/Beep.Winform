using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Circle "NEWS LIVE" badge + pink banner with angled edge
    /// Style: Dark circle badge + magenta/pink angled section
    /// Example: "NEWS LIVE | BREAKING NEWS"
    /// </summary>
    public class NewsLiveCirclePinkPainter : BaseButtonPainter
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
            string badgeText = parts[0]; // NEWS LIVE
            string mainText = parts[1]; // BREAKING NEWS

            // Circle badge
            int circleSize = (int)(bounds.Height * 1.2);
            Rectangle circleBounds = new Rectangle(
                bounds.X - 5,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize,
                circleSize
            );

            Color circleBgColor = Color.FromArgb(40, 40, 50); // Dark

            using (Brush circleBrush = new SolidBrush(circleBgColor))
            {
                g.FillEllipse(circleBrush, circleBounds);
            }

            // Draw badge text (split into two lines)
            string[] badgeLines = badgeText.Split(' ');
            if (badgeLines.Length >= 2)
            {
                using (Brush badgeTextBrush = new SolidBrush(Color.White))
                using (Font badgeFont = new Font(context.Font.FontFamily, context.Font.Size * 0.65f, FontStyle.Bold))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    
                    Rectangle line1Bounds = new Rectangle(
                        circleBounds.X,
                        circleBounds.Y + (int)(circleBounds.Height * 0.35),
                        circleBounds.Width,
                        (int)(circleBounds.Height * 0.3)
                    );
                    g.DrawString(badgeLines[0], badgeFont, badgeTextBrush, line1Bounds, format);

                    Rectangle line2Bounds = new Rectangle(
                        circleBounds.X,
                        circleBounds.Y + (int)(circleBounds.Height * 0.55),
                        circleBounds.Width,
                        (int)(circleBounds.Height * 0.3)
                    );
                    g.DrawString(badgeLines[1], badgeFont, badgeTextBrush, line2Bounds, format);
                }
            }

            // Draw angled banner section (pink/magenta)
            int bannerStart = bounds.X + circleSize - 10;
            int angleWidth = 30;

            Color bannerColor = context.SolidBackground != Color.Empty
                ? context.SolidBackground
                : Color.FromArgb(240, 50, 140); // Pink/magenta

            using (GraphicsPath bannerPath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bannerStart, bounds.Y),
                    new Point(bounds.Right, bounds.Y),
                    new Point(bounds.Right - angleWidth, bounds.Bottom),
                    new Point(bannerStart + angleWidth / 2, bounds.Bottom)
                };
                bannerPath.AddPolygon(points);

                using (Brush bannerBrush = new SolidBrush(bannerColor))
                {
                    g.FillPath(bannerBrush, bannerPath);
                }
            }

            // Draw "LIVE" indicator badge
            Rectangle liveBounds = new Rectangle(
                bannerStart + angleWidth,
                bounds.Y + (int)(bounds.Height * 0.6),
                50,
                (int)(bounds.Height * 0.32)
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

            // Draw main text
            Rectangle textBounds = new Rectangle(
                bannerStart + angleWidth + 10,
                bounds.Y,
                bounds.Right - bannerStart - angleWidth * 2 - 20,
                (int)(bounds.Height * 0.5)
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
                return new[] { "NEWS LIVE", "BREAKING NEWS" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                return new[] { parts[0].Trim(), parts.Length > 1 ? parts[1].Trim() : "" };
            }

            return new[] { "NEWS LIVE", text };
        }
    }
}
