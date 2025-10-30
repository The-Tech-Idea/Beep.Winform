using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// ReviewCardPainter - For user reviews, testimonials, and comments
    /// Features avatar, user info, rating, and feedback text
    /// </summary>
    internal sealed class ReviewCardPainter : CardPainterBase
    {
        private Font _badgeFont;
        private Font _helpfulFont;
        private Font _quoteFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _badgeFont?.Dispose(); } catch { }
            try { _helpfulFont?.Dispose(); } catch { }
            try { _quoteFont?.Dispose(); } catch { }
            _badgeFont = new Font(Owner.Font.FontFamily, 7.5f, FontStyle.Regular);
            _helpfulFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            _quoteFont = new Font("Georgia", 32f, FontStyle.Bold);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // User avatar (small, top-left)
            if (ctx.ShowImage)
            {
                int avatarSize = 44;
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, avatarSize, avatarSize);
            }

            // User info area (name and date)
            int infoLeft = ctx.DrawingRect.Left + pad + (ctx.ShowImage ? 54 : 0);
            int infoWidth = ctx.DrawingRect.Width - pad * 2 - (ctx.ShowImage ? 54 : 0);
            
            // User name (header)
            ctx.HeaderRect = new Rectangle(infoLeft, ctx.DrawingRect.Top + pad, infoWidth, 18);

            // Review date (subtitle)
            ctx.SubtitleRect = new Rectangle(infoLeft, ctx.HeaderRect.Bottom + 2, infoWidth / 2, 14);

            // Star rating (top right)
            if (ctx.ShowRating)
            {
                ctx.RatingRect = new Rectangle(ctx.DrawingRect.Right - pad - 90, ctx.DrawingRect.Top + pad, 85, 18);
            }

            // Verification badge (verified purchase, verified reviewer)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(infoLeft, ctx.SubtitleRect.Bottom + 4, 100, 18);
            }

            // Review text
            int reviewTop = Math.Max(ctx.ImageRect.Bottom, ctx.SubtitleRect.Bottom) + (string.IsNullOrEmpty(ctx.BadgeText1) ? 10 : 26);
            int reviewHeight = Math.Max(40, ctx.DrawingRect.Height - (reviewTop - ctx.DrawingRect.Top) - pad * 2 - (ctx.ShowButton ? ButtonHeight + 8 : 0));
            ctx.ParagraphRect = new Rectangle(ctx.DrawingRect.Left + pad, reviewTop, 
                ctx.DrawingRect.Width - pad * 2, reviewHeight);

            // Helpful/reaction buttons
            if (ctx.ShowButton)
            {
                int buttonWidth = 100;
                ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Bottom - pad - ButtonHeight, 
                    buttonWidth, ButtonHeight);
                
                // Helpful count display
                if (!string.IsNullOrEmpty(ctx.StatusText))
                {
                    ctx.StatusRect = new Rectangle(ctx.ButtonRect.Right + 10, ctx.ButtonRect.Top, 
                        ctx.DrawingRect.Right - pad - (ctx.ButtonRect.Right + 10), ButtonHeight);
                }
            }

            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw star rating
            if (ctx.ShowRating && ctx.Rating > 0)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, ctx.AccentColor);
            }

            // Draw verification badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }

            // Draw helpful count
            if (!string.IsNullOrEmpty(ctx.StatusText) && ctx.StatusRect != Rectangle.Empty)
            {
                var helpfulBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(120, ctx.AccentColor));
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _helpfulFont, helpfulBrush, ctx.StatusRect, format);
            }

            // Draw avatar border
            if (ctx.ShowImage)
            {
                var borderPen = PaintersFactory.GetPen(Color.FromArgb(50, ctx.AccentColor), 2);
                g.DrawEllipse(borderPen, ctx.ImageRect);
            }

            // Draw quote marks for testimonial Style (subtle decoration)
            if (ctx.ParagraphRect.Height > 40)
            {
                var quoteBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(15, ctx.AccentColor));
                g.DrawString("\"", _quoteFont, quoteBrush, ctx.ParagraphRect.Left - 4, ctx.ParagraphRect.Top - 12);
            }
        }
    }
}
