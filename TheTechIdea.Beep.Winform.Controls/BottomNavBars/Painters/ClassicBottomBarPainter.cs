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
            // Draw flat background with slight top border
            using (var b = new SolidBrush(Color.FromArgb(250, 250, 250)))
            {
                context.Graphics.FillRectangle(b, context.Bounds);
            }
            using (var p = new Pen(Color.FromArgb(220, 220, 220)))
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
