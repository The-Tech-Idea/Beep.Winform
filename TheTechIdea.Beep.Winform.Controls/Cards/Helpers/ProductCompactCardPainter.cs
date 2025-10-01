using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// ProductCompactCard - Horizontal compact product display for lists
    /// </summary>
    internal sealed class ProductCompactCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = Inset(drawingRect, 6);

            int imageSize = Math.Max(40, ctx.DrawingRect.Height - pad * 2);
            ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, imageSize, imageSize);

            int contentLeft = ctx.ImageRect.Right + 12;
            int contentWidth = Math.Max(0, ctx.DrawingRect.Width - (contentLeft - ctx.DrawingRect.Left) - 80 - pad);

            ctx.HeaderRect = new Rectangle(contentLeft, ctx.DrawingRect.Top + pad, contentWidth, 18);
            ctx.SubtitleRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + 2, contentWidth, 14);
            ctx.RatingRect = new Rectangle(contentLeft, ctx.SubtitleRect.Bottom + 4, 80, 14);

            ctx.ParagraphRect = new Rectangle(Math.Max(ctx.DrawingRect.Right - pad - 75, contentLeft), ctx.DrawingRect.Top + pad, 70, 20);

            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(Math.Max(ctx.DrawingRect.Right - pad - 45, contentLeft), Math.Max(ctx.DrawingRect.Bottom - pad - 16, ctx.RatingRect.Bottom + 4), 40, 14);
            }

            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw price
            if (!string.IsNullOrEmpty(ctx.SubtitleText)) // Price in SubtitleText
            {
                using var priceFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
                using var priceBrush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, priceFont, priceBrush, ctx.ParagraphRect, format);
            }
            
            // Draw rating stars (compact)
            if (ctx.ShowRating && ctx.Rating > 0)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, Color.FromArgb(255, 193, 7));
            }
            
            // Draw badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                using var badgeFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Bold);
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, badgeFont);
            }
        }
    }
}