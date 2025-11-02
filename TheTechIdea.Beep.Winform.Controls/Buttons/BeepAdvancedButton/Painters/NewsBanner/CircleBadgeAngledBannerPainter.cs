using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Circle badge on left + slanted/angled white banner section
    /// Style: Colored circle + angled white text section
    /// Example: "24 | BREAKING NEWS"
    /// </summary>
    public class CircleBadgeAngledBannerPainter : BaseButtonPainter
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
            string badgeText = parts[0]; // 24 or icon text
            string mainText = parts[1]; // BREAKING NEWS

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

            // Draw badge border
            using (Pen circlePen = new Pen(Color.White, 2))
            {
                g.DrawEllipse(circlePen, circleBounds);
            }

            // Draw badge text or icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    circleBounds.X + (circleBounds.Width - metrics.IconSize) / 2,
                    circleBounds.Y + (circleBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }
            else if (!string.IsNullOrEmpty(badgeText))
            {
                using (Brush badgeTextBrush = new SolidBrush(Color.White))
                using (Font badgeFont = new Font(context.Font.FontFamily, context.Font.Size + 2, FontStyle.Bold))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    g.DrawString(badgeText, badgeFont, badgeTextBrush, circleBounds, format);
                }
            }

            // Draw angled white banner
            int bannerStart = bounds.X + circleSize - 15;
            int angleSlant = 15;

            using (GraphicsPath bannerPath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bannerStart + angleSlant, bounds.Y),
                    new Point(bounds.Right, bounds.Y),
                    new Point(bounds.Right, bounds.Bottom),
                    new Point(bannerStart, bounds.Bottom)
                };
                bannerPath.AddPolygon(points);

                using (Brush bannerBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(bannerBrush, bannerPath);
                }
            }

            // Draw main text
            Rectangle textBounds = new Rectangle(
                bannerStart + angleSlant + metrics.PaddingHorizontal,
                bounds.Y,
                bounds.Right - bannerStart - angleSlant - metrics.PaddingHorizontal * 2,
                bounds.Height
            );

            using (Brush textBrush = new SolidBrush(badgeColor))
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
                return new[] { "24", "BREAKING NEWS" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                return new[] { parts[0].Trim(), parts.Length > 1 ? parts[1].Trim() : "" };
            }

            // Try to extract number or short word
            string[] words = text.Split(' ');
            if (words.Length > 0 && (int.TryParse(words[0], out _) || words[0].Length <= 3))
                return new[] { words[0], string.Join(" ", words, 1, words.Length - 1) };

            return new[] { "", text };
        }
    }
}
