using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// TestimonialCard - Quote style with avatar and rating stars
    /// </summary>
    internal sealed class TestimonialCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad + 4;
            ctx.DrawingRect = Inset(drawingRect, 8);

            // Quote area
            ctx.ParagraphRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad + 12, ctx.DrawingRect.Width - pad * 2, Math.Max(40, (int)(ctx.DrawingRect.Height * 0.45)));

            int avatar = 48;
            ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Bottom - pad - avatar, avatar, avatar);

            int nameLeft = ctx.ImageRect.Right + 12;
            int nameWidth = Math.Max(0, ctx.DrawingRect.Width - (nameLeft - ctx.DrawingRect.Left) - pad);
            ctx.HeaderRect = new Rectangle(nameLeft, ctx.ImageRect.Top, nameWidth, 20);
            ctx.RatingRect = new Rectangle(nameLeft, ctx.HeaderRect.Bottom + 4, 100, 16);

            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw quote marks as a subtle background accent inside the content area
            using (var quoteBrush = new SolidBrush(Color.FromArgb(30, ctx.AccentColor)))
            using (var quoteFont = new Font("Serif", 36, FontStyle.Bold))
            {
                g.DrawString("''", quoteFont, quoteBrush, ctx.DrawingRect.Left + 20, ctx.DrawingRect.Top + 5);
            }

            // Draw rating stars
            if (ctx.ShowRating && ctx.Rating > 0)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, ctx.AccentColor);
            }
        }
    }
}