using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// ContentCard - Banner image top, content below (like course cards)
    /// </summary>
    internal sealed class ContentCardPainter : CardPainterBase
    {
        private Font _badgeFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _badgeFont?.Dispose(); } catch { }
            _badgeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // Banner image top area ~45%
            int bannerHeight = Math.Max(60, (int)(ctx.DrawingRect.Height * 0.45));
            ctx.ImageRect = new Rectangle(
                ctx.DrawingRect.Left,
                ctx.DrawingRect.Top,
                ctx.DrawingRect.Width,
                Math.Min(bannerHeight, ctx.DrawingRect.Height - (pad * 3))
            );

            // Content below
            var content = new Rectangle(ctx.DrawingRect.Left + pad, ctx.ImageRect.Bottom + pad, ctx.DrawingRect.Width - pad * 2, Math.Max(0, ctx.DrawingRect.Bottom - (ctx.ImageRect.Bottom + pad)));

            ctx.HeaderRect = new Rectangle(content.Left, content.Top, content.Width, HeaderHeight);
            ctx.ParagraphRect = new Rectangle(content.Left, ctx.HeaderRect.Bottom + 6, content.Width, Math.Max(0, content.Height - HeaderHeight - ButtonHeight - 18));

            // Tags row under paragraph
            ctx.TagsRect = new Rectangle(content.Left, Math.Min(content.Bottom - ButtonHeight - 8, ctx.ParagraphRect.Bottom + 6), Math.Max(0, content.Width - 120), 22);

            // Badge in top-right of banner
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(Math.Max(ctx.DrawingRect.Right - pad - 50, ctx.ImageRect.Left + 8), ctx.ImageRect.Top + pad, 45, 20);
            }

            // Primary button bottom-right
            ctx.ButtonRect = new Rectangle(Math.Max(content.Right - 100, content.Left), Math.Max(content.Bottom - ButtonHeight, ctx.TagsRect.Bottom + 6), 95, ButtonHeight);

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
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw chips/tags
            CardRenderingHelpers.DrawChips(g, Owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags);
        }
    }
}
