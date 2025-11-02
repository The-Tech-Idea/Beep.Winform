using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Circle "BN LIVE" badge + two-tone angled banner
    /// Style: Black circle with white text + pink/white angled sections
    /// Example: "BN LIVE | BREAKING NEWS"
    /// </summary>
    public class BNLiveCircleBannerPainter : BaseButtonPainter
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
            string badgeText = parts[0]; // BN LIVE
            string mainText = parts[1]; // BREAKING NEWS
            string subText = parts[2]; // Lorem ipsum

            // Circle badge
            int circleSize = (int)(bounds.Height * 1.2);
            Rectangle circleBounds = new Rectangle(
                bounds.X - 5,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize,
                circleSize
            );

            Color circleBgColor = Color.FromArgb(30, 30, 30); // Dark gray/black

            using (Brush circleBrush = new SolidBrush(circleBgColor))
            {
                g.FillEllipse(circleBrush, circleBounds);
            }

            // Draw badge text
            using (Brush badgeTextBrush = new SolidBrush(Color.White))
            using (Font badgeFont = new Font(context.Font.FontFamily, context.Font.Size * 0.7f, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(badgeText, badgeFont, badgeTextBrush, circleBounds, format);
            }

            // Draw angled banner section (blue)
            int bannerStart = bounds.X + circleSize - 10;
            int angleWidth = 30;

            Color section1Color = context.SolidBackground != Color.Empty
                ? context.SolidBackground
                : Color.FromArgb(0, 150, 200); // Cyan/blue

            using (GraphicsPath section1Path = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bannerStart, bounds.Y),
                    new Point(bounds.Right - angleWidth, bounds.Y),
                    new Point(bounds.Right, bounds.Bottom),
                    new Point(bannerStart + angleWidth, bounds.Bottom)
                };
                section1Path.AddPolygon(points);

                using (Brush section1Brush = new SolidBrush(section1Color))
                {
                    g.FillPath(section1Brush, section1Path);
                }
            }

            // Draw main text
            Rectangle mainTextBounds = new Rectangle(
                bannerStart + angleWidth / 2 + 10,
                bounds.Y,
                bounds.Right - bannerStart - angleWidth - 20,
                (int)(bounds.Height * 0.55)
            );

            using (Brush textBrush = new SolidBrush(Color.White))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                g.DrawString(mainText, boldFont, textBrush, mainTextBounds, format);
            }

            // Draw sub text if present
            if (!string.IsNullOrEmpty(subText))
            {
                Rectangle subTextBounds = new Rectangle(
                    mainTextBounds.X,
                    bounds.Y + (int)(bounds.Height * 0.5),
                    mainTextBounds.Width,
                    (int)(bounds.Height * 0.45)
                );

                using (Font smallFont = new Font(context.Font.FontFamily, context.Font.Size * 0.65f))
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
                return new[] { "BN\nLIVE", "BREAKING NEWS", "" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                return new[] 
                {
                    parts.Length > 0 ? parts[0].Trim() : "BN LIVE",
                    parts.Length > 1 ? parts[1].Trim() : "",
                    parts.Length > 2 ? parts[2].Trim() : ""
                };
            }

            return new[] { "BN\nLIVE", text, "" };
        }
    }
}
