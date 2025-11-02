using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Hexagon/chevron shape on both sides (WORLD NEWS style)
    /// Style: Colored hexagon banner with chevron points on left and right
    /// Example: "WORLD NEWS - BREAKING NEWS"
    /// </summary>
    public class HexagonWorldNewsPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            int chevronWidth = 25;
            Color bgColor = context.SolidBackground;

            // Draw hexagon shape (chevrons on both sides)
            using (GraphicsPath hexagonPath = new GraphicsPath())
            {
                int midY = bounds.Y + bounds.Height / 2;

                Point[] points = new Point[]
                {
                    new Point(bounds.X + chevronWidth, bounds.Y),
                    new Point(bounds.Right - chevronWidth, bounds.Y),
                    new Point(bounds.Right, midY),
                    new Point(bounds.Right - chevronWidth, bounds.Bottom),
                    new Point(bounds.X + chevronWidth, bounds.Bottom),
                    new Point(bounds.X, midY)
                };
                hexagonPath.AddPolygon(points);

                using (Brush bgBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(bgBrush, hexagonPath);
                }
            }

            // Draw icon if provided (globe icon)
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    bounds.X + chevronWidth + 10,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw text
            int textStartX = bounds.X + chevronWidth + (context.IconLeft != null ? metrics.IconSize + 20 : 15);
            Rectangle textBounds = new Rectangle(
                textStartX,
                bounds.Y,
                bounds.Right - chevronWidth - textStartX - 15,
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
