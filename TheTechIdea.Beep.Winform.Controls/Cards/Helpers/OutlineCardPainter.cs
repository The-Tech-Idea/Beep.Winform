using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    internal sealed class OutlineCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -1, -1);
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            using var bg = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var p = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(bg, p);
            using var pen = new Pen(Theme?.StatsCardBorderColor ?? Color.Silver, 1);
            g.DrawPath(pen, p);
        }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
        }

        public override void UpdateHitAreas(BaseControl owner, LayoutContext ctx, System.Action<string, Rectangle> notifyAreaHit)
        {
        }
    }
}
