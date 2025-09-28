using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// ListCard - Horizontal layout with avatar/icon (like Director listings)
    /// </summary>
    internal sealed class ListCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Avatar/icon on left
            int avatarSize = 40;
            ctx.ImageRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + (ctx.DrawingRect.Height - avatarSize) / 2,
                avatarSize, avatarSize
            );
            
            // Content in middle
            int contentLeft = ctx.ImageRect.Right + 12;
            int contentWidth = ctx.DrawingRect.Width - (contentLeft - ctx.DrawingRect.Left) - 120 - pad;
            
            ctx.HeaderRect = new Rectangle(contentLeft, ctx.DrawingRect.Top + pad, contentWidth, 20);
            ctx.SubtitleRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + 2, contentWidth, 16);
            ctx.ParagraphRect = new Rectangle(contentLeft, ctx.SubtitleRect.Bottom + 4, contentWidth, 20);
            
            // Badge/rating on right
            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(
                    ctx.DrawingRect.Right - pad - 100,
                    ctx.DrawingRect.Top + pad,
                    95, 16
                );
            }
            else if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    ctx.DrawingRect.Right - pad - 80,
                    ctx.DrawingRect.Top + pad,
                    75, 18
                );
            }
            
            ctx.ShowButton = false;
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
            // Draw rating stars
            if (ctx.ShowRating && ctx.Rating > 0)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, ctx.AccentColor);
            }
            // Draw badge
            else if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                using var badgeFont = new Font(Owner.Font.FontFamily, 7.5f, FontStyle.Bold);
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, badgeFont);
            }
        }
    }
}