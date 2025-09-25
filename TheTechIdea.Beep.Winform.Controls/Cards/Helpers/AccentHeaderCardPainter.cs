using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    internal sealed class AccentHeaderCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int headerStripe = 8;
            var inner = Rectangle.Inflate(drawingRect, -1, -1);
            ctx.DrawingRect = inner;
            ctx.HeaderRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + headerStripe + 4, ctx.HeaderRect.Width, ctx.HeaderRect.Height);
            ctx.ParagraphRect = new Rectangle(ctx.ParagraphRect.X, ctx.ParagraphRect.Y + headerStripe + 4, ctx.ParagraphRect.Width, ctx.ParagraphRect.Height - (headerStripe + 4));
            if (!ctx.ImageRect.IsEmpty)
            {
                ctx.ImageRect = new Rectangle(ctx.ImageRect.X, ctx.ImageRect.Y + headerStripe + 4, ctx.ImageRect.Width, ctx.ImageRect.Height);
            }
            if (!ctx.ButtonRect.IsEmpty)
            {
                ctx.ButtonRect = new Rectangle(ctx.ButtonRect.X, ctx.ButtonRect.Y + headerStripe + 4, ctx.ButtonRect.Width, ctx.ButtonRect.Height);
            }
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            using var bg = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var p = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(bg, p);

            var stripe = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, ctx.DrawingRect.Width, 8);
            using var stripeBrush = new SolidBrush(ctx.AccentColor);
            g.FillRectangle(stripeBrush, stripe);
        }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            int d = ctx.HeaderRect.Height;
            var badge = new Rectangle(ctx.HeaderRect.Left - d - 6, ctx.HeaderRect.Top, d, d);
            using var b = new SolidBrush(Color.FromArgb(24, ctx.AccentColor));
            using var pen = new Pen(ctx.AccentColor, 2);
            g.FillEllipse(b, badge);
            g.DrawEllipse(pen, badge);
        }

        public override void UpdateHitAreas(BaseControl owner, LayoutContext ctx, System.Action<string, Rectangle> notifyAreaHit)
        {
            var stripe = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, ctx.DrawingRect.Width, 8);
            owner.AddHitArea("AccentHeaderStripe", stripe, null, () => notifyAreaHit?.Invoke("AccentHeaderStripe", stripe));
        }
    }
}
