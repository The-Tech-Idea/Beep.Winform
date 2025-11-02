using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Green "MORNING LIVE" label + yellow "BREAKING NEWS" banner with time display
    /// Style: Stacked green section on top + yellow angled main section + time badge
    /// Example: "MORNING LIVE | BREAKING NEWS | 15:30 PM"
    /// </summary>
    public class MorningLiveYellowBannerPainter : BaseButtonPainter
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
            string topText = parts[0]; // MORNING LIVE
            string mainText = parts[1]; // BREAKING NEWS
            string timeText = parts[2]; // 15:30 PM

            int topHeight = (int)(bounds.Height * 0.35);
            int angleWidth = 25;

            Color topColor = Color.FromArgb(80, 180, 100); // Green
            Color mainColor = context.SolidBackground != Color.Empty
                ? context.SolidBackground
                : Color.FromArgb(255, 220, 0); // Yellow

            // Draw top section (green)
            Rectangle topBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, topHeight);
            using (Brush topBrush = new SolidBrush(topColor))
            {
                g.FillRectangle(topBrush, topBounds);
            }

            // Draw top text
            using (Brush topTextBrush = new SolidBrush(Color.White))
            using (Font topFont = new Font(context.Font.FontFamily, context.Font.Size * 0.75f, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(topText, topFont, topTextBrush, topBounds, format);
            }

            // Draw main section (yellow) with angled bottom-right
            int mainHeight = bounds.Height - topHeight;
            Rectangle mainBounds = new Rectangle(bounds.X, bounds.Y + topHeight, bounds.Width, mainHeight);

            using (GraphicsPath mainPath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(mainBounds.X, mainBounds.Y),
                    new Point(mainBounds.Right, mainBounds.Y),
                    new Point(mainBounds.Right - angleWidth, mainBounds.Bottom),
                    new Point(mainBounds.X, mainBounds.Bottom)
                };
                mainPath.AddPolygon(points);

                using (Brush mainBrush = new SolidBrush(mainColor))
                {
                    g.FillPath(mainBrush, mainPath);
                }
            }

            // Draw main text
            Rectangle textBounds = new Rectangle(
                mainBounds.X + metrics.PaddingHorizontal,
                mainBounds.Y,
                mainBounds.Width - angleWidth - metrics.PaddingHorizontal * 2 - 70,
                mainBounds.Height
            );

            Color textColor = mainColor.GetBrightness() > 0.5f ? Color.Black : Color.White;
            using (Brush textBrush = new SolidBrush(textColor))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                g.DrawString(mainText, boldFont, textBrush, textBounds, format);
            }

            // Draw time badge on right side
            Rectangle timeBounds = new Rectangle(
                mainBounds.Right - angleWidth - 70,
                mainBounds.Y + (int)(mainBounds.Height * 0.25),
                60,
                (int)(mainBounds.Height * 0.5)
            );

            using (Brush timeBgBrush = new SolidBrush(topColor))
            {
                g.FillRectangle(timeBgBrush, timeBounds);
            }

            using (Brush timeTextBrush = new SolidBrush(Color.White))
            using (Font timeFont = new Font(context.Font.FontFamily, context.Font.Size * 0.65f, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(timeText, timeFont, timeTextBrush, timeBounds, format);
            }
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "MORNING LIVE", "BREAKING NEWS", "15:30 PM" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                return new[] 
                {
                    parts.Length > 0 ? parts[0].Trim() : "MORNING LIVE",
                    parts.Length > 1 ? parts[1].Trim() : "BREAKING NEWS",
                    parts.Length > 2 ? parts[2].Trim() : "15:30 PM"
                };
            }

            return new[] { "MORNING LIVE", text, "15:30 PM" };
        }
    }
}
