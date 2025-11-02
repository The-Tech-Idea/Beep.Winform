using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Lightning bolt + orange "BREAKING NEWS LIVE" banner
    /// Style: Yellow lightning section + orange pill with LIVE badge
    /// Example: "BREAKING NEWS | LIVE"
    /// </summary>
    public class LightningBreakingNewsLivePainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            int iconWidth = bounds.Height;
            int angleWidth = 15;

            Color iconColor = context.IconBackgroundColor != Color.Empty
                ? context.IconBackgroundColor
                : Color.FromArgb(255, 220, 0); // Yellow

            Color mainColor = context.SolidBackground != Color.Empty
                ? context.SolidBackground
                : Color.FromArgb(255, 120, 0); // Orange

            // Draw lightning icon section
            using (GraphicsPath iconPath = new GraphicsPath())
            {
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.X + iconWidth, bounds.Y),
                    new Point(bounds.X + iconWidth + angleWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                iconPath.AddPolygon(points);

                using (Brush iconBrush = new SolidBrush(iconColor))
                {
                    g.FillPath(iconBrush, iconPath);
                }
            }

            // Draw lightning icon
            if (!string.IsNullOrEmpty(context.IconLeft))
            {
                Rectangle iconBounds = new Rectangle(
                    bounds.X + (iconWidth - metrics.IconSize) / 2,
                    bounds.Y + (bounds.Height - metrics.IconSize) / 2,
                    metrics.IconSize,
                    metrics.IconSize
                );
                DrawIcon(g, context, iconBounds, context.IconLeft);
            }

            // Draw main pill section (orange)
            int pillStart = bounds.X + iconWidth + angleWidth;
            Rectangle pillBounds = new Rectangle(
                pillStart,
                bounds.Y,
                bounds.Width - iconWidth - angleWidth,
                bounds.Height
            );

            int radius = bounds.Height / 2;
            using (GraphicsPath pillPath = new GraphicsPath())
            {
                int diameter = radius * 2;
                pillPath.AddLine(pillBounds.X, pillBounds.Y, pillBounds.Right - radius, pillBounds.Y);
                pillPath.AddArc(pillBounds.Right - diameter, pillBounds.Y, diameter, diameter, 270, 180);
                pillPath.AddLine(pillBounds.Right - radius, pillBounds.Bottom, pillBounds.X, pillBounds.Bottom);
                pillPath.CloseFigure();

                using (Brush pillBrush = new SolidBrush(mainColor))
                {
                    g.FillPath(pillBrush, pillPath);
                }
            }

            // Draw main text
            Rectangle textBounds = new Rectangle(
                pillBounds.X + metrics.PaddingHorizontal,
                pillBounds.Y,
                pillBounds.Width - radius - metrics.PaddingHorizontal - 60,
                pillBounds.Height
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

            // Draw LIVE badge
            Rectangle liveBounds = new Rectangle(
                pillBounds.Right - radius - 55,
                pillBounds.Y + (int)(pillBounds.Height * 0.3),
                50,
                (int)(pillBounds.Height * 0.4)
            );

            using (Brush liveBrush = new SolidBrush(iconColor))
            {
                g.FillRectangle(liveBrush, liveBounds);
            }

            using (Brush liveTextBrush = new SolidBrush(Color.Black))
            using (Font liveFont = new Font(context.Font.FontFamily, context.Font.Size * 0.7f, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString("LIVE", liveFont, liveTextBrush, liveBounds, format);
            }
        }
    }
}
