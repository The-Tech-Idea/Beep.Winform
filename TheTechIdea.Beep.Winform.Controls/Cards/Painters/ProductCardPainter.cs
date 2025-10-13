using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// ProductCard - E-commerce product display with image, price, rating
    /// </summary>
    internal sealed class ProductCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            int imageHeight = Math.Min(ctx.DrawingRect.Width - pad * 2, Math.Max(80, (int)(ctx.DrawingRect.Height * 0.5)));
            ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, imageHeight);

            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.ImageRect.Left + 8, ctx.ImageRect.Top + 8, 50, 20);
            }

            var content = new Rectangle(ctx.DrawingRect.Left + pad, ctx.ImageRect.Bottom + 10, ctx.DrawingRect.Width - pad * 2, Math.Max(0, ctx.DrawingRect.Bottom - (ctx.ImageRect.Bottom + 10)));

            ctx.HeaderRect = new Rectangle(content.Left, content.Top, content.Width, HeaderHeight);
            ctx.SubtitleRect = new Rectangle(content.Left, ctx.HeaderRect.Bottom + 4, content.Width, 16);
            ctx.RatingRect = new Rectangle(content.Left, ctx.SubtitleRect.Bottom + 6, 100, 16);
            ctx.ParagraphRect = new Rectangle(content.Right - 80, ctx.RatingRect.Top, 75, 20); // price at right

            ctx.ButtonRect = new Rectangle(content.Left, Math.Max(content.Bottom - ButtonHeight, ctx.RatingRect.Bottom + 10), content.Width, ButtonHeight);
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw sale/new badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                using var badgeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, badgeFont);
            }
            
            // Draw rating stars
            if (ctx.ShowRating && ctx.Rating > 0)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, ctx.AccentColor);
            }
            
            // Draw price with currency formatting
            if (!string.IsNullOrEmpty(ctx.SubtitleText)) // Use SubtitleText for price
            {
                using var priceFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var priceBrush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, priceFont, priceBrush, ctx.ParagraphRect, format);
            }
        }
    }
}
