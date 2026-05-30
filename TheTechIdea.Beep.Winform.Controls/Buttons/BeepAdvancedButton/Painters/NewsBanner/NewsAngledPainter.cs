using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    public class NewsAngledPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            string[] parts = SplitText(context.Text);
            string leftText = parts[0];
            string rightText = parts.Length > 1 ? parts[1] : string.Empty;

            Color leftColor = context.SolidBackground;
            Color rightColor = context.SecondaryColor != Color.Empty ? context.SecondaryColor : Color.White;
            int slant = 15;

            // Left section
            Rectangle leftBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width / 2 + slant, bounds.Height);
            using (GraphicsPath leftPath = new GraphicsPath())
            {
                leftPath.AddPolygon(new Point[]
                {
                    new Point(leftBounds.X, leftBounds.Y),
                    new Point(leftBounds.Right, leftBounds.Y),
                    new Point(leftBounds.Right - slant, leftBounds.Bottom),
                    new Point(leftBounds.X, leftBounds.Bottom)
                });
                using (SolidBrush leftBrush = new SolidBrush(leftColor))
                {
                    g.FillPath(leftBrush, leftPath);
                }
            }

            // Right section
            Rectangle rightBounds = new Rectangle(bounds.X + bounds.Width / 2, bounds.Y, bounds.Width / 2, bounds.Height);
            using (GraphicsPath rightPath = new GraphicsPath())
            {
                rightPath.AddPolygon(new Point[]
                {
                    new Point(rightBounds.X + slant, rightBounds.Y),
                    new Point(rightBounds.Right, rightBounds.Y),
                    new Point(rightBounds.Right, rightBounds.Bottom),
                    new Point(rightBounds.X, rightBounds.Bottom)
                });
                using (SolidBrush rightBrush = new SolidBrush(rightColor))
                {
                    g.FillPath(rightBrush, rightPath);
                }
            }

            // Left text
            using (Font boldFont = GetDerivedTextFont(context, styleOverride: System.Drawing.FontStyle.Bold))
            using (SolidBrush leftTextBrush = new SolidBrush(Color.White))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(leftText, boldFont, leftTextBrush, leftBounds, sf);
            }

            // Right text
            if (!string.IsNullOrEmpty(rightText))
            {
                Color rightTextColor = rightColor == Color.White ? leftColor : Color.White;
                using (Font boldFont = GetDerivedTextFont(context, styleOverride: System.Drawing.FontStyle.Bold))
                using (SolidBrush rightTextBrush = new SolidBrush(rightTextColor))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(rightText, boldFont, rightTextBrush, rightBounds, sf);
                }
            }

            DrawRippleEffect(g, context);
        }

        private string[] SplitText(string text)
        {
            if (string.IsNullOrEmpty(text)) return new[] { "LIVE", "NEWS" };
            if (text.Contains("|")) return text.Split('|');
            if (text.Contains(" - ")) return text.Split(new[] { " - " }, System.StringSplitOptions.None);
            return new[] { text, string.Empty };
        }
    }
}
