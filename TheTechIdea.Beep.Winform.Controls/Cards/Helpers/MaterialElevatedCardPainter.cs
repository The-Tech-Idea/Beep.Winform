using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    internal sealed class MaterialElevatedCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            // Slight inner padding to create breathing room
            var pad = 4;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -pad, -pad);
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // MD-like subtle shadow and white surface
            SoftShadow(g, ctx.DrawingRect, ctx.Radius, layers: 4, offset: 2);
            using var bg = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var p = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(bg, p);
        }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Optional small divider between content areas can be added by consumers if needed
        }

        public override void UpdateHitAreas(BaseControl owner, LayoutContext ctx, System.Action<string, Rectangle> notifyAreaHit)
        {
            // No extra areas for this style
        }
    }
}
