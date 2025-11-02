using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Circular icon badge overlapping pill-shaped banner (FOOTBALL NEWS style)
    /// Style: Colored pill with white circular icon badge on left
    /// Example: "FOOTBALL NEWS - EXCLUSIVE" with soccer ball icon
    /// </summary>
    public class IconCirclePillNewsPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            float iconScale = 1.2f;
            int iconOverlap = 10;

            // Calculate icon circle size
            int iconCircleSize = (int)(bounds.Height * iconScale);
            Rectangle iconCircleBounds = new Rectangle(
                bounds.X - iconOverlap,
                bounds.Y + (bounds.Height - iconCircleSize) / 2,
                iconCircleSize,
                iconCircleSize
            );

            Color pillColor = context.SolidBackground;
            Color iconBgColor = context.IconBackgroundColor != Color.Empty
                ? context.IconBackgroundColor
                : Color.White;

            // Draw main pill
            int radius = bounds.Height / 2;
            using (GraphicsPath pillPath = new GraphicsPath())
            {
                int diameter = radius * 2;
                pillPath.AddArc(bounds.X, bounds.Y, diameter, diameter, 90, 180);
                pillPath.AddLine(bounds.X + radius, bounds.Y, bounds.Right - radius, bounds.Y);
                pillPath.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 180);
                pillPath.AddLine(bounds.Right - radius, bounds.Bottom, bounds.X + radius, bounds.Bottom);
                pillPath.CloseFigure();

                using (Brush pillBrush = new SolidBrush(pillColor))
                {
                    g.FillPath(pillBrush, pillPath);
                }
            }

            // Draw icon circle badge
            using (Brush iconCircleBrush = new SolidBrush(iconBgColor))
            {
                g.FillEllipse(iconCircleBrush, iconCircleBounds);
            }

            // Draw icon circle border
            using (Pen iconPen = new Pen(pillColor, 3))
            {
                g.DrawEllipse(iconPen, iconCircleBounds);
            }

            // Draw icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    iconCircleBounds.X + (iconCircleBounds.Width - metrics.IconSize) / 2,
                    iconCircleBounds.Y + (iconCircleBounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            int textStartX = bounds.X + iconCircleSize - iconOverlap + 10;
            Rectangle textBounds = new Rectangle(
                textStartX,
                bounds.Y,
                bounds.Right - radius - textStartX - 10,
                bounds.Height
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
