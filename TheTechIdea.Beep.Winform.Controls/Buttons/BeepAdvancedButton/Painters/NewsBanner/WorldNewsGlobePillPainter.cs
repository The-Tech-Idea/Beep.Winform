using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Globe icon circle + white "WORLD NEWS | SPECIAL REPORT" banner
    /// Style: Purple/blue circle with globe icon + white pill banner
    /// Example: "WORLD NEWS | SPECIAL REPORT"
    /// </summary>
    public class WorldNewsGlobePillPainter : BaseButtonPainter
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
            string section1Text = parts[0]; // WORLD NEWS
            string section2Text = parts[1]; // SPECIAL REPORT

            Color iconCircleColor = context.IconBackgroundColor != Color.Empty
                ? context.IconBackgroundColor
                : Color.FromArgb(100, 80, 180); // Purple/blue

            // Draw icon circle
            int circleSize = (int)(bounds.Height * 1.1);
            Rectangle circleBounds = new Rectangle(
                bounds.X - 5,
                bounds.Y + (bounds.Height - circleSize) / 2,
                circleSize,
                circleSize
            );

            using (Brush circleBrush = new SolidBrush(iconCircleColor))
            {
                g.FillEllipse(circleBrush, circleBounds);
            }

            // Draw globe icon
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

            // Draw main pill section (white)
            int pillStart = bounds.X + circleSize;
            int pillWidth = bounds.Width - circleSize;
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

            // Draw section 1 label (small text at top)
            if (!string.IsNullOrEmpty(section2Text))
            {
                Rectangle labelBounds = new Rectangle(
                    pillBounds.X + metrics.PaddingHorizontal,
                    pillBounds.Y + 2,
                    pillBounds.Width - radius - metrics.PaddingHorizontal,
                    (int)(pillBounds.Height * 0.35)
                );

                using (Font smallFont = new Font(context.Font.FontFamily, context.Font.Size * 0.65f))
                using (Brush labelBrush = new SolidBrush(Color.FromArgb(150, 150, 150)))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    g.DrawString(section2Text.ToUpper(), smallFont, labelBrush, labelBounds, format);
                }
            }

            // Draw main text
            Rectangle textBounds = new Rectangle(
                pillBounds.X + metrics.PaddingHorizontal,
                pillBounds.Y + (int)(pillBounds.Height * 0.35),
                pillBounds.Width - radius - metrics.PaddingHorizontal * 2,
                (int)(pillBounds.Height * 0.6)
            );

            using (Brush textBrush = new SolidBrush(iconCircleColor))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(section1Text, boldFont, textBrush, textBounds, format);
            }
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "WORLD NEWS", "SPECIAL REPORT" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                return new[] { parts[0].Trim(), parts.Length > 1 ? parts[1].Trim() : "" };
            }

            return new[] { text, "" };
        }
    }
}
