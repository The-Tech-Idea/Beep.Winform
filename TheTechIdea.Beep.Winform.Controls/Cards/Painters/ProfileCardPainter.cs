using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// ProfileCard - Vertical profile with large image (like Corey Tawney card)
    /// </summary>
    internal sealed class ProfileCardPainter : CardPainterBase
    {
        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad + 4;
            ctx.DrawingRect = drawingRect;

            // Hero image area ~40% of card height, clamped
            int imgH = Math.Max(80, Math.Min((int)(ctx.DrawingRect.Height * 0.4), ctx.DrawingRect.Height - (pad * 4)));
            ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, imgH);

            // Header under image
            ctx.HeaderRect = new Rectangle(ctx.ImageRect.Left, ctx.ImageRect.Bottom + 12, ctx.ImageRect.Width, HeaderHeight);

            // Optional status line under header
            ctx.StatusRect = new Rectangle(ctx.HeaderRect.Left, ctx.HeaderRect.Bottom + 4, ctx.HeaderRect.Width, 18);

            // Badge top-right over image
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.ImageRect.Right - 60, ctx.ImageRect.Top + 12, 50, 20);
            }

            // Primary button at bottom with full width minus padding
            int btnH = Math.Min(40, Math.Max(ButtonHeight, 32));
            ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Bottom - pad - btnH, ctx.DrawingRect.Width - pad * 2, btnH);

            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                using var badgeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, badgeFont);
            }
            
            // Draw status
            if (ctx.ShowStatus && !string.IsNullOrEmpty(ctx.StatusText))
            {
                using var statusFont = new Font(Owner.Font.FontFamily, 8.5f);
                CardRenderingHelpers.DrawStatus(g, ctx.StatusRect, ctx.StatusText, ctx.StatusColor, statusFont);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
                owner.AddHitArea("Badge", ctx.BadgeRect, null, () => notifyAreaHit?.Invoke("Badge", ctx.BadgeRect));
            
            if (!ctx.StatusRect.IsEmpty)
                owner.AddHitArea("Status", ctx.StatusRect, null, () => notifyAreaHit?.Invoke("Status", ctx.StatusRect));
        }
    }
}
