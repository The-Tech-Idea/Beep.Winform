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
            int pad = 20;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            // Quote content at top
            ctx.ParagraphRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad + 20,
                ctx.DrawingRect.Width - pad * 2,
                (int)(ctx.DrawingRect.Height * 0.5)
            );
            
            // Avatar at bottom left
            int avatarSize = 48;
            ctx.ImageRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Bottom - pad - avatarSize,
                avatarSize, avatarSize
            );
            
            // Author name next to avatar
            ctx.HeaderRect = new Rectangle(
                ctx.ImageRect.Right + 12,
                ctx.ImageRect.Top,
                ctx.DrawingRect.Width - (ctx.ImageRect.Right + 12 - ctx.DrawingRect.Left) - pad,
                20
            );
            
            // Rating stars below name
            ctx.RatingRect = new Rectangle(
                ctx.ImageRect.Right + 12,
                ctx.HeaderRect.Bottom + 4,
                100, 16
            );
            
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 16, layers: 5, offset: 3);
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 16);
            g.FillPath(bgBrush, bgPath);
            
            // Draw quote marks
            using var quoteBrush = new SolidBrush(Color.FromArgb(30, ctx.AccentColor));
            using var quoteFont = new Font("Serif", 36, FontStyle.Bold);
            g.DrawString("''", quoteFont, quoteBrush, ctx.DrawingRect.Left + 20, ctx.DrawingRect.Top + 5);
        }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw rating stars
            if (ctx.ShowRating && ctx.Rating > 0)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, ctx.AccentColor);
            }
        }
    }
}