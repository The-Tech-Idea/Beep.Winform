using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DataControls.Helpers
{
    internal sealed class GlassBarNavigatorPainter : NavigatorPainterBase
    {
        public override NavigatorLayout AdjustLayout(Rectangle drawingRect, NavigatorLayout ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -2, -2);
            return ctx;
        }

        public override void DrawBackground(Graphics g, NavigatorLayout ctx)
        {
            using var glass = new SolidBrush(Color.FromArgb(28, Color.White));
            using var path = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(glass, path);
            using var pen = new Pen(Color.FromArgb(64, Color.White), 1);
            g.DrawPath(pen, path);
        }

        public override void DrawForeground(Graphics g, NavigatorLayout ctx)
        {
            var top = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, ctx.DrawingRect.Width, ctx.DrawingRect.Height / 3);
            using var lg = new LinearGradientBrush(top, Color.FromArgb(64, Color.White), Color.FromArgb(0, Color.White), LinearGradientMode.Vertical);
            g.FillRectangle(lg, top);
        }
    }
}
