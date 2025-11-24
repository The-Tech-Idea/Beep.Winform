using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class ClassicBottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "Classic";
        public override void Paint(BottomBarPainterContext context)
        {
            base.CalculateLayout(context);
            // Draw flat background with slight top border (theme-driven)
            using (var b = new SolidBrush(context.BarBackColor == Color.Empty ? Color.FromArgb(250, 250, 250) : context.BarBackColor))
            {
                context.Graphics.FillRectangle(b, context.Bounds);
            }
            var borderColor = context.NavigationBorderColor == Color.Empty ? (context.BarHoverBackColor == Color.Empty ? Color.FromArgb(220, 220, 220) : context.BarHoverBackColor) : context.NavigationBorderColor;
            using (var p = new Pen(borderColor))
            {
                context.Graphics.DrawLine(p, context.Bounds.Left, context.Bounds.Top, context.Bounds.Right, context.Bounds.Top);
            }

            // indicator
            var indicator = _layoutHelper.GetIndicatorRect();
            using (var ib = new SolidBrush(Color.FromArgb(30, context.AccentColor)))
            {
                context.Graphics.FillRectangle(ib, indicator);
            }

            // Draw items
            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var item = context.Items[i];
                PaintMenuItem(context.Graphics, item, rects[i], context);
            }
        }
    }
}
