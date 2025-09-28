using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// ContentCard - Banner image top, content below (like course cards)
    /// </summary>
    internal sealed class ContentCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            // Banner image at top
            int bannerHeight = (int)(ctx.DrawingRect.Height * 0.45);
            ctx.ImageRect = new Rectangle(
                ctx.DrawingRect.Left,
                ctx.DrawingRect.Top,
                ctx.DrawingRect.Width,
                bannerHeight
            );
            
            // Content area below banner
            int contentTop = ctx.ImageRect.Bottom + pad;
            int contentLeft = ctx.DrawingRect.Left + pad;
            int contentWidth = ctx.DrawingRect.Width - pad * 2;
            
            ctx.HeaderRect = new Rectangle(contentLeft, contentTop, contentWidth, 24);
            ctx.ParagraphRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + 8, contentWidth, 40);
            
            // Tags/chips row
            ctx.TagsRect = new Rectangle(contentLeft, ctx.ParagraphRect.Bottom + 8, contentWidth - 120, 24);
            
            // Badge in top-right corner of banner
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.DrawingRect.Right - pad - 50, ctx.DrawingRect.Top + pad, 45, 20);
            }
            
            // Action button at bottom-right
            ctx.ButtonRect = new Rectangle(
                ctx.DrawingRect.Right - pad - 100,
                ctx.DrawingRect.Bottom - pad - 32,
                95, 28
            );
            
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 16, layers: 6, offset: 4);
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, 16);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                using var badgeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, badgeFont);
            }
            
            // Draw chips/tags
            CardRenderingHelpers.DrawChips(g, Owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags);
        }
    }
}