using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class ClassicBottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "Classic";

        public override void Paint(BottomBarPainterContext context)
        {
            CalculateLayout(context);
            PaintBarBackground(context.Graphics, context);
            PaintBarBorder(context.Graphics, context);
            DrawIndicatorPill(context.Graphics, GetAnimatedIndicatorRect(context), ResolveAccent(context), 0.3f);
            PaintItems(context.Graphics, context);
        }

        private RectangleF GetAnimatedIndicatorRect(BottomBarPainterContext context)
        {
            var indicatorRect = _layoutHelper.GetIndicatorRect();
            if (context.AnimatedIndicatorWidth > 0f)
            {
                return new RectangleF(context.AnimatedIndicatorX, indicatorRect.Top, context.AnimatedIndicatorWidth, indicatorRect.Height);
            }
            return indicatorRect;
        }
    }
}
