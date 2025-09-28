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
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            // Product image at top (square aspect ratio)
            int imageHeight = Math.Min(ctx.DrawingRect.Width - pad * 2, (int)(ctx.DrawingRect.Height * 0.5));
            ctx.ImageRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                imageHeight
            );
            
            // Badge (Sale, New, etc.) in top-left corner
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    ctx.DrawingRect.Left + pad + 8,
                    ctx.DrawingRect.Top + pad + 8,
                    50, 20
                );
            }
            
            // Product title below image
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ImageRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // Brand/category subtitle
            ctx.SubtitleRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 4,
                ctx.DrawingRect.Width - pad * 2,
                16
            );
            
            // Rating stars
            ctx.RatingRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.SubtitleRect.Bottom + 8,
                100, 16
            );
            
            // Price area (right side of rating row)
            ctx.ParagraphRect = new Rectangle(
                ctx.DrawingRect.Right - pad - 80,
                ctx.RatingRect.Top,
                75, 20
            );
            
            // Add to cart button at bottom
            ctx.ButtonRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Bottom - pad - 32,
                ctx.DrawingRect.Width - pad * 2,
                28
            );
            
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 12);
            g.FillPath(bgBrush, bgPath);
        }

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