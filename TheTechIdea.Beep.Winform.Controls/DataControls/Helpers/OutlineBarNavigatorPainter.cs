using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DataControls.Helpers
{
    internal sealed class OutlineBarNavigatorPainter : NavigatorPainterBase
    {
        public override NavigatorLayout AdjustLayout(Rectangle drawingRect, NavigatorLayout ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -1, -1);
            return ctx;
        }

        public override void DrawBackground(Graphics g, NavigatorLayout ctx)
        {
            using var bg = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var path = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(bg, path);
            using var pen = new Pen(Theme?.StatsCardBorderColor ?? Color.Silver, 1);
            g.DrawPath(pen, path);
        }

        public override void DrawForeground(Graphics g, NavigatorLayout ctx)
        {
            // no extra accents
        }
    }
}
