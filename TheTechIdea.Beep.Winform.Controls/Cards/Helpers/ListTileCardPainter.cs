using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    internal sealed class ListTileCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            // Compact look similar to list tiles
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -2, -2);
            // Shift header slightly right to make room for a small avatar ring accent
            ctx.HeaderRect = new Rectangle(ctx.HeaderRect.Left + 32, ctx.HeaderRect.Top + 4, ctx.HeaderRect.Width - 32, ctx.HeaderRect.Height);
            ctx.ParagraphRect = new Rectangle(ctx.ParagraphRect.Left + 32, ctx.ParagraphRect.Top + 4, ctx.ParagraphRect.Width - 32, ctx.ParagraphRect.Height - 4);
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            using var bg = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var p = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(bg, p);
        }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Small colored dot/avatar ring
            int d = ctx.HeaderRect.Height;
            var dot = new Rectangle(ctx.HeaderRect.Left - d, ctx.HeaderRect.Top, d - 4, d - 4);
            using var ring = new Pen(ctx.AccentColor, 3);
            g.DrawEllipse(ring, dot);
        }

        public override void UpdateHitAreas(BaseControl owner, LayoutContext ctx, System.Action<string, Rectangle> notifyAreaHit)
        {
            int d = ctx.HeaderRect.Height;
            var dot = new Rectangle(ctx.HeaderRect.Left - d, ctx.HeaderRect.Top, d - 4, d - 4);
            owner.AddHitArea("AvatarRing", dot, null, () => notifyAreaHit?.Invoke("AvatarRing", dot));
        }
    }
}
