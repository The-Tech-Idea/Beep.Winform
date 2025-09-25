using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    internal sealed class SoftShadowCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            SoftShadow(g, ctx.DrawingRect, ctx.Radius + 4, layers: 6, offset: 4);
            using var bg = new SolidBrush(Theme?.DashboardCardBackColor ?? Color.White);
            using var p = Round(ctx.DrawingRect, ctx.Radius + 2);
            g.FillPath(bg, p);
        }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
        }

        public override void UpdateHitAreas(BaseControl owner, LayoutContext ctx, System.Action<string, Rectangle> notifyAreaHit)
        {
        }
    }
}
