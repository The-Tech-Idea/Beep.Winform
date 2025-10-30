using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// ListCard - Horizontal layout with avatar/icon (like Director listings)
    /// </summary>
    internal sealed class ListCardPainter : CardPainterBase
    {
        private Font _badgeFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _badgeFont?.Dispose(); } catch { }
            _badgeFont = new Font(Owner.Font.FontFamily, 7.5f, FontStyle.Bold);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            int avatarSize = 40;
            ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + (ctx.DrawingRect.Height - avatarSize) / 2, avatarSize, avatarSize);

            int contentLeft = ctx.ImageRect.Right + 12;
            int contentWidth = Math.Max(0, ctx.DrawingRect.Width - (contentLeft - ctx.DrawingRect.Left) - 120 - pad);

            ctx.HeaderRect = new Rectangle(contentLeft, ctx.DrawingRect.Top + pad, contentWidth, 20);
            ctx.SubtitleRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + 2, contentWidth, 16);
            ctx.ParagraphRect = new Rectangle(contentLeft, ctx.SubtitleRect.Bottom + 4, contentWidth, Math.Max(0, ctx.DrawingRect.Bottom - (ctx.SubtitleRect.Bottom + 4) - pad));

            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(ctx.DrawingRect.Right - pad - 100, ctx.DrawingRect.Top + pad, 95, 16);
            }
            else if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.DrawingRect.Right - pad - 80, ctx.DrawingRect.Top + pad, 75, 18);
            }

            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

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
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
        }
    }
}
