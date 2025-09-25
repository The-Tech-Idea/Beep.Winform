using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DataControls.Helpers
{
    internal sealed class SoftShadowBarNavigatorPainter : NavigatorPainterBase
    {
        public override NavigatorLayout AdjustLayout(Rectangle drawingRect, NavigatorLayout ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            return ctx;
        }

        public override void DrawBackground(Graphics g, NavigatorLayout ctx)
        {
            SoftShadow(g, ctx.DrawingRect, ctx.Radius + 2, layers: 5, offset: 3);
            using var bg = new SolidBrush(Theme?.DashboardCardBackColor ?? Color.White);
            using var path = Round(ctx.DrawingRect, ctx.Radius + 2);
            g.FillPath(bg, path);
        }

        public override void DrawForeground(Graphics g, NavigatorLayout ctx)
        {
        }
    }
}
