using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DataControls.Helpers
{
    internal sealed class PillNavigatorPainter : NavigatorPainterBase
    {
        public override NavigatorLayout AdjustLayout(Rectangle drawingRect, NavigatorLayout ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            return ctx;
        }

        public override void DrawBackground(Graphics g, NavigatorLayout ctx)
        {
            var pillRect = ctx.DrawingRect;
            using var bg = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var path = Round(pillRect, pillRect.Height / 2);
            g.FillPath(bg, path);
        }

        public override void DrawForeground(Graphics g, NavigatorLayout ctx)
        {
        }
    }
}
