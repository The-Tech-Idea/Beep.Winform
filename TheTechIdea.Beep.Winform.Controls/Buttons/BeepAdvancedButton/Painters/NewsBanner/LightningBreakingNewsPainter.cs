using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Lightning bolt icon + colored rectangle banner (BREAKING NEWS)
    /// Style: Yellow lightning bolt icon section + orange text section
    /// Example: "BREAKING NEWS - TODAY EXCLUSIVE"
    /// </summary>
    public class LightningBreakingNewsPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            int iconSectionWidth = bounds.Height;
            int angleWidth = 15;

            Color iconBgColor = context.IconBackgroundColor != Color.Empty
                ? context.IconBackgroundColor
                : Color.FromArgb(255, 230, 0); // Yellow

            Color mainBgColor = context.SolidBackground != Color.Empty
                ? context.SolidBackground
                : Color.FromArgb(255, 100, 0); // Orange

            // Draw icon section with angle
            using (GraphicsPath iconPath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.X + iconSectionWidth, bounds.Y),
                    new Point(bounds.X + iconSectionWidth + angleWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                iconPath.AddPolygon(points);

                using (Brush iconBrush = new SolidBrush(iconBgColor))
                {
                    g.FillPath(iconBrush, iconPath);
                }
            }

            // Draw main section
            Rectangle mainBounds = new Rectangle(
                bounds.X + iconSectionWidth + angleWidth,
                bounds.Y,
                bounds.Width - iconSectionWidth - angleWidth,
                bounds.Height
            );

            using (Brush mainBrush = new SolidBrush(mainBgColor))
            {
                g.FillRectangle(mainBrush, mainBounds);
            }

            // Draw icon (lightning bolt)
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    bounds.X + (iconSectionWidth - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw main text
            Rectangle textBounds = new Rectangle(
                mainBounds.X + metrics.PaddingHorizontal,
                mainBounds.Y,
                mainBounds.Width - metrics.PaddingHorizontal * 2,
                mainBounds.Height
            );

            using (Brush textBrush = new SolidBrush(Color.White))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                g.DrawString(context.Text, boldFont, textBrush, textBounds, format);
            }
        }
    }
}
