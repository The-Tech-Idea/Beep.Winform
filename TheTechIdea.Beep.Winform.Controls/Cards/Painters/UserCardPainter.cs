using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// UserCardPainter - For user profiles and team member cards
    /// Compact layout with avatar, name, role, and contact actions
    /// </summary>
    internal sealed class UserCardPainter : CardPainterBase
    {
        private Font _badgeFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _badgeFont?.Dispose(); } catch { }
            _badgeFont = new Font(Owner.Font.FontFamily,8f, FontStyle.Regular);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // Avatar/profile image (centered or left-aligned)
            if (ctx.ShowImage)
            {
                int avatarSize =60;
                // Center avatar horizontally
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + (ctx.DrawingRect.Width - avatarSize) /2, ctx.DrawingRect.Top + pad, avatarSize, avatarSize);
            }

            // Name (header)
            int nameTop = ctx.ShowImage ? ctx.ImageRect.Bottom +10 : ctx.DrawingRect.Top + pad;
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, nameTop, ctx.DrawingRect.Width - pad *2, HeaderHeight);

            // Role/Title (subtitle)
            ctx.SubtitleRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom +4, ctx.DrawingRect.Width - pad *2,16);

            // Status badge (online, away, busy, offline)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.DrawingRect.Left + (ctx.DrawingRect.Width -80) /2, ctx.SubtitleRect.Bottom +8,80,20);
            }

            // Bio or additional info
            int bioTop = ctx.SubtitleRect.Bottom + (string.IsNullOrEmpty(ctx.BadgeText1) ?10 :32);
            ctx.ParagraphRect = new Rectangle(ctx.DrawingRect.Left + pad, bioTop, ctx.DrawingRect.Width - pad *2, Math.Max(18, ctx.DrawingRect.Height - (bioTop - ctx.DrawingRect.Top) - pad *2 - (ctx.ShowButton ? ButtonHeight +8 :0)));

            // Action buttons (View Profile, Message, Follow, etc.)
            if (ctx.ShowButton)
            {
                int buttonY = Math.Max(ctx.DrawingRect.Bottom - pad - ButtonHeight, ctx.ParagraphRect.Bottom +8);
                
                if (ctx.ShowSecondaryButton)
                {
                    int buttonWidth = (ctx.DrawingRect.Width - pad *3) /2;
                    ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Left + pad, buttonY, buttonWidth, ButtonHeight);
                    ctx.SecondaryButtonRect = new Rectangle(ctx.ButtonRect.Right + pad, buttonY, buttonWidth, ButtonHeight);
                }
                else
                {
                    ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Left + pad, buttonY, ctx.DrawingRect.Width - pad *2, ButtonHeight);
                }
            }

            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw status badge (online, away, etc.)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }

            // Draw avatar border/ring (optional accent)
            if (ctx.ShowImage)
            {
                var borderPen = PaintersFactory.GetPen(ctx.AccentColor,2);
                // Draw circular border around avatar
                g.DrawEllipse(borderPen, ctx.ImageRect);
            }

            // Draw centered text formatting helper lines (optional subtle design element)
            // This helps with Material Design elevation feel
        }
    }
}
