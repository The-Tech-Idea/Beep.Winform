using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// BlogCardPainter - For blog posts, news articles, and editorial content
    /// Article-Style layout with featured image, title, excerpt, metadata
    /// </summary>
    internal sealed class BlogCardPainter : CardPainterBase
    {
        private Font _badgeFont;
        private Font _statsFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _badgeFont?.Dispose(); } catch { }
            try { _statsFont?.Dispose(); } catch { }
            _badgeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            _statsFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // Featured image at top (large, prominent)
            if (ctx.ShowImage)
            {
                int imageHeight = Math.Min(ctx.DrawingRect.Width, Math.Max(100, (int)(ctx.DrawingRect.Height * 0.4)));
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, ctx.DrawingRect.Width, imageHeight);
            }

            // Category badge overlaying top-left of image
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.DrawingRect.Left + pad + 4, ctx.DrawingRect.Top + pad + 4, 80, 20);
            }

            // Content area below image
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + pad : ctx.DrawingRect.Top + pad;
            int contentHeight = ctx.DrawingRect.Bottom - contentTop - pad;

            // Article title
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, contentTop, ctx.DrawingRect.Width - pad * 2, HeaderHeight + 8); // Slightly taller for blog titles

            // Author and date metadata
            ctx.SubtitleRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 4, 
                ctx.DrawingRect.Width - pad * 2, 16);

            // Article excerpt/summary
            int excerptHeight = Math.Max(40, contentHeight - (ctx.HeaderRect.Height + ctx.SubtitleRect.Height) - (ctx.ShowButton ? ButtonHeight + 12 : 8));
            ctx.ParagraphRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.SubtitleRect.Bottom + 6, 
                ctx.DrawingRect.Width - pad * 2, excerptHeight);

            // Read more button or engagement stats
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Bottom - pad - ButtonHeight, 
                    (ctx.DrawingRect.Width - pad * 2) / 2, ButtonHeight);
            }

            // Rating or engagement indicators (likes, comments, shares)
            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(ctx.DrawingRect.Right - pad - 100, ctx.DrawingRect.Bottom - pad - ButtonHeight, 
                    95, ButtonHeight);
            }

            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }

            // Draw engagement stats (likes, comments, views)
            if (ctx.ShowRating && !string.IsNullOrEmpty(ctx.StatusText))
            {
                var statsBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(128, ctx.AccentColor));
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _statsFont, statsBrush, ctx.RatingRect, format);
            }

            // Draw subtle divider line between image and content
            if (ctx.ShowImage)
            {
                var dividerPen = PaintersFactory.GetPen(Color.FromArgb(30, ctx.AccentColor), 1);
                g.DrawLine(dividerPen, ctx.DrawingRect.Left, ctx.ImageRect.Bottom, 
                    ctx.DrawingRect.Right, ctx.ImageRect.Bottom);
            }
        }
    }
}
