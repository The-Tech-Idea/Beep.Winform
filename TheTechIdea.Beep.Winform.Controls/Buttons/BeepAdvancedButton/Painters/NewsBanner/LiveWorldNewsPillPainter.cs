using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    /// <summary>
    /// Yellow "LIVE NEWS" pill with "WORLD NEWS" label above
    /// Style: Stacked design with label text above main pill banner
    /// Example: "WORLD NEWS | LIVE NEWS"
    /// </summary>
    public class LiveWorldNewsPillPainter : BaseButtonPainter
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
            string labelText = parts[0]; // LOREM IPSUM DOLOR SIT AMET
            string mainText = parts[1]; // WORLD NEWS

            // Calculate label height (small text at top)
            int labelHeight = (int)(bounds.Height * 0.35);
            int mainHeight = bounds.Height - labelHeight;

            // Draw label section (small text at top)
            Rectangle labelBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, labelHeight);
            
            Color labelColor = context.SecondaryColor != null && context.SecondaryColor != Color.Empty
                ? context.SecondaryColor
                : Color.FromArgb(100, 150, 200);

            using (Brush labelBrush = new SolidBrush(labelColor))
            {
                g.FillRectangle(labelBrush, labelBounds);
            }

            // Draw main pill section
            Rectangle mainBounds = new Rectangle(bounds.X, bounds.Y + labelHeight, bounds.Width, mainHeight);
            int radius = mainHeight / 2;

            Color pillColor = context.SolidBackground;

            using (GraphicsPath pillPath = new GraphicsPath())
            {
                int diameter = radius * 2;
                pillPath.AddArc(mainBounds.X, mainBounds.Y, diameter, diameter, 90, 180);
                pillPath.AddLine(mainBounds.X + radius, mainBounds.Y, mainBounds.Right - radius, mainBounds.Y);
                pillPath.AddArc(mainBounds.Right - diameter, mainBounds.Y, diameter, diameter, 270, 180);
                pillPath.AddLine(mainBounds.Right - radius, mainBounds.Bottom, mainBounds.X + radius, mainBounds.Bottom);
                pillPath.CloseFigure();

                using (Brush pillBrush = new SolidBrush(pillColor))
                {
                    g.FillPath(pillBrush, pillPath);
                }
            }

            // Draw label text (small)
            if (!string.IsNullOrEmpty(labelText))
            {
                using (Font smallFont = new Font(context.Font.FontFamily, context.Font.Size * 0.7f))
                using (Brush labelTextBrush = new SolidBrush(Color.White))
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    g.DrawString(labelText.ToUpper(), smallFont, labelTextBrush, labelBounds, format);
                }
            }

            // Draw main text
            Rectangle textBounds = new Rectangle(
                mainBounds.X + metrics.PaddingHorizontal + radius / 2,
                mainBounds.Y,
                mainBounds.Width - metrics.PaddingHorizontal * 2 - radius,
                mainBounds.Height
            );

            using (Brush textBrush = new SolidBrush(Color.White))
            using (Font boldFont = new Font(context.Font, FontStyle.Bold))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                g.DrawString(mainText, boldFont, textBrush, textBounds, format);
            }
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "", "WORLD NEWS" };

            if (text.Contains("|"))
            {
                string[] parts = text.Split('|');
                return new[] { parts[0].Trim(), parts.Length > 1 ? parts[1].Trim() : "" };
            }

            return new[] { "", text };
        }
    }
}
