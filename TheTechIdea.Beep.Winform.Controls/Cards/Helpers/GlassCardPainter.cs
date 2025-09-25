using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    internal sealed class GlassCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -2, -2);
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            using var glass = new SolidBrush(Color.FromArgb(28, Color.White));
            using var p = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(glass, p);
            using var pen = new Pen(Color.FromArgb(64, Color.White), 1);
            g.DrawPath(pen, p);
        }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            var top = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, ctx.DrawingRect.Width, ctx.DrawingRect.Height / 3);
            using var lg = new LinearGradientBrush(top, Color.FromArgb(64, Color.White), Color.FromArgb(0, Color.White), LinearGradientMode.Vertical);
            g.FillRectangle(lg, top);
        }

        public override void UpdateHitAreas(BaseControl owner, LayoutContext ctx, System.Action<string, Rectangle> notifyAreaHit)
        {
        }
    }
}
