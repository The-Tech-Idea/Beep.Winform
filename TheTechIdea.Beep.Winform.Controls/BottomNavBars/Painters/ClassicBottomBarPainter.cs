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
            using (var b = new SolidBrush(ResolveBarBack(context)))
            {
                context.Graphics.FillRectangle(b, context.Bounds);
            }
            var borderColor = context.NavigationBorderColor == Color.Empty ? Color.FromArgb(30, ResolveBarFore(context)) : context.NavigationBorderColor;
            using (var p = new Pen(borderColor))
            {
                context.Graphics.DrawLine(p, context.Bounds.Left, context.Bounds.Top, context.Bounds.Right, context.Bounds.Top);
            }

            var indicator = _layoutHelper.GetIndicatorRect();
            using (var ib = new SolidBrush(Color.FromArgb(30, ResolveAccent(context))))
            {
                context.Graphics.FillRectangle(ib, indicator);
            }

            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var item = context.Items[i];
                PaintMenuItem(context.Graphics, item, rects[i], context);
            }
        }
    }
}
