using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DataControls.Helpers
{
    internal sealed class MaterialBarNavigatorPainter : NavigatorPainterBase
    {
        public override NavigatorLayout AdjustLayout(Rectangle drawingRect, NavigatorLayout ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -2, -2);
            return ctx;
        }

        public override void DrawBackground(Graphics g, NavigatorLayout ctx)
        {
            // simple bar with subtle elevation
            SoftShadow(g, ctx.DrawingRect, ctx.Radius, layers: 3, offset: 1);
            using var bg = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var p = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(bg, p);
        }

        public override void DrawForeground(Graphics g, NavigatorLayout ctx)
        {
            // Optional accent line at bottom (like material indicator)
            var line = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Bottom - 2, ctx.DrawingRect.Width, 2);
            using var accent = new SolidBrush(ctx.AccentColor);
            g.FillRectangle(accent, line);
        }
    }
}
