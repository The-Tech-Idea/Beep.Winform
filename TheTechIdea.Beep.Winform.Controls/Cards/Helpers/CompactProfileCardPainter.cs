using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// CompactProfile - Smaller profile variant for lists
    /// </summary>
    internal sealed class CompactProfileCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            
            // Smaller circular avatar on left
            int avatarSize = 48;
            ctx.ImageRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                avatarSize, avatarSize
            );
            
            // Content area to the right
            int contentLeft = ctx.ImageRect.Right + 12;
            int contentWidth = ctx.DrawingRect.Width - (contentLeft - ctx.DrawingRect.Left) - pad;
            
            ctx.HeaderRect = new Rectangle(contentLeft, ctx.DrawingRect.Top + pad, contentWidth, 20);
            ctx.StatusRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + 2, contentWidth, 16);
            
            // Badge in top-right
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.DrawingRect.Right - pad - 40, ctx.DrawingRect.Top + pad, 35, 16);
            }
            
            ctx.ShowSecondaryButton = false;
            ctx.ShowButton = false;
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
            // Draw badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                using var badgeFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Bold);
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, badgeFont);
            }
            
            // Draw status
            if (ctx.ShowStatus && !string.IsNullOrEmpty(ctx.StatusText))
            {
                using var statusFont = new Font(Owner.Font.FontFamily, 7.5f);
                CardRenderingHelpers.DrawStatus(g, ctx.StatusRect, ctx.StatusText, ctx.StatusColor, statusFont);
            }
        }
    }
}