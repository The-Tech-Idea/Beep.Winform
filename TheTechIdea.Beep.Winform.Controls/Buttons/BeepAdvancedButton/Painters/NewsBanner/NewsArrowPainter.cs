using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters.NewsBanner
{
    public class NewsArrowPainter : BaseButtonPainter
    {
        public override void Paint(AdvancedButtonPaintContext context)
        {
            Graphics g = context.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var metrics = GetMetrics(context);
            Rectangle bounds = context.Bounds;

            Color bgColor = context.SolidBackground;
            int arrowWidth = 25;

            using (GraphicsPath arrowPath = new GraphicsPath())
            {
                int midY = bounds.Y + bounds.Height / 2;
                Point[] points = new Point[]
                {
                    new Point(bounds.X, bounds.Y),
                    new Point(bounds.Right - arrowWidth, bounds.Y),
                    new Point(bounds.Right, midY),
                    new Point(bounds.Right - arrowWidth, bounds.Bottom),
                    new Point(bounds.X, bounds.Bottom)
                };
                arrowPath.AddPolygon(points);

                using (SolidBrush bgBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(bgBrush, arrowPath);
                }
            }

            Rectangle textBounds = new Rectangle(
                bounds.X + metrics.PaddingHorizontal,
                bounds.Y,
                bounds.Width - arrowWidth - metrics.PaddingHorizontal * 2,
                bounds.Height
            );

            using (Font boldFont = GetDerivedTextFont(context, styleOverride: System.Drawing.FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter })
            {
                g.DrawString(context.Text, boldFont, textBrush, textBounds, sf);
            }

            DrawRippleEffect(g, context);
        }
    }
}
