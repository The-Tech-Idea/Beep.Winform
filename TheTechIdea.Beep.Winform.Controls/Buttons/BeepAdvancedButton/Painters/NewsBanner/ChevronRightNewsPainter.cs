using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Right-pointing chevron/arrow banner
    /// Style: Full-width colored banner with arrow point on right side
    /// Example: "BREAKING NEWS" with right arrow
    /// </summary>
    public class ChevronRightNewsPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            int chevronWidth = 30;
            Color bgColor = context.SolidBackground;

            // Draw chevron shape
            using (GraphicsPath chevronPath = new GraphicsPath())
            {
                int midY = bounds.Y + bounds.Height / 2;

                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.Right - chevronWidth, bounds.Y),
                    new Point(bounds.Right, midY),
                    new Point(bounds.Right - chevronWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                chevronPath.AddPolygon(points);

                using (Brush bgBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(bgBrush, chevronPath);
                }
            }

            // Draw text
            Rectangle textBounds = new Rectangle(
                bounds.X + metrics.PaddingHorizontal,
                bounds.Y,
                bounds.Width - chevronWidth - metrics.PaddingHorizontal * 2,
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
