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
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            int avatar = 48;
            ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, avatar, avatar);

            int contentLeft = ctx.ImageRect.Right + 12;
            int contentWidth = Math.Max(0, ctx.DrawingRect.Width - (contentLeft - ctx.DrawingRect.Left) - pad);

            ctx.HeaderRect = new Rectangle(contentLeft, ctx.DrawingRect.Top + pad, contentWidth, 20);
            ctx.StatusRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + 2, contentWidth, 16);

            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(Math.Max(ctx.DrawingRect.Right - pad - 40, contentLeft), ctx.DrawingRect.Top + pad, 35, 16);
            }

            ctx.ShowSecondaryButton = false;
            ctx.ShowButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

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
