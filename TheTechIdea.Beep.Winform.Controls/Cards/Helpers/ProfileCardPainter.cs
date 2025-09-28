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
            int pad = 20;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            // Large image at top (40% of height)
            int imageHeight = (int)(ctx.DrawingRect.Height * 0.4);
            ctx.ImageRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                imageHeight
            );
            
            // Header below image
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ImageRect.Bottom + 16,
                ctx.DrawingRect.Width - pad * 2,
                28
            );
            
            // Status below header
            ctx.StatusRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 4,
                ctx.DrawingRect.Width - pad * 2,
                20
            );
            
            // Badge in top-right corner
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    ctx.DrawingRect.Right - pad - 60,
                    ctx.DrawingRect.Top + pad + 12,
                    50, 20
                );
            }
            
            // Button at bottom
            int buttonHeight = 40;
            ctx.ButtonRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Bottom - pad - buttonHeight,
                ctx.DrawingRect.Width - pad * 2,
                buttonHeight
            );
            
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        public override void DrawBackground(Graphics g, LayoutContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 16, layers: 5, offset: 3);
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