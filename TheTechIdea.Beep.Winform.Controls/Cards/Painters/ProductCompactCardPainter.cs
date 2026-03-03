using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using PaintersFactory = TheTechIdea.Beep.Winform.Controls.Styling.PaintersFactory;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// ProductCompactCard - Horizontal compact product display for lists
    /// </summary>
    internal sealed class ProductCompactCardPainter : ICardPainter, IDisposable
    {
        private BaseControl Owner;
        private IBeepTheme Theme;
        private Font _priceFont;
        private Font _badgeFont;

        // Constants specific to this painter
        private const int DefaultPad = 12;
        private const int HeaderHeight = 26;
        private const int ButtonHeight = 32;

        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            Owner = owner;
            Theme = theme;
_priceFont = titleFont;
            _badgeFont = captionFont;
        }

        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DpiScalingHelper.ScaleValue(DefaultPad, Owner);
            int headerHeight = DpiScalingHelper.ScaleValue(HeaderHeight, Owner);
            ctx.DrawingRect = drawingRect;

            int imageSize = Math.Max(40, ctx.DrawingRect.Height - pad * 2);
            ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, imageSize, imageSize);

            int contentLeft = ctx.ImageRect.Right + DpiScalingHelper.ScaleValue(12, Owner);
            int contentWidth = Math.Max(0, ctx.DrawingRect.Width - (contentLeft - ctx.DrawingRect.Left) - DpiScalingHelper.ScaleValue(80, Owner) - pad);

            ctx.HeaderRect = new Rectangle(contentLeft, ctx.DrawingRect.Top + pad, contentWidth, headerHeight);
            ctx.SubtitleRect = new Rectangle(contentLeft, ctx.HeaderRect.Bottom + DpiScalingHelper.ScaleValue(2, Owner), contentWidth, DpiScalingHelper.ScaleValue(14, Owner));
            ctx.RatingRect = new Rectangle(contentLeft, ctx.SubtitleRect.Bottom + DpiScalingHelper.ScaleValue(4, Owner), DpiScalingHelper.ScaleValue(80, Owner), DpiScalingHelper.ScaleValue(14, Owner));
            ctx.ParagraphRect = new Rectangle(Math.Max(ctx.DrawingRect.Right - pad - DpiScalingHelper.ScaleValue(75, Owner), contentLeft), ctx.DrawingRect.Top + pad, DpiScalingHelper.ScaleValue(70, Owner), DpiScalingHelper.ScaleValue(20, Owner));

            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    Math.Max(ctx.DrawingRect.Right - pad - DpiScalingHelper.ScaleValue(45, Owner), contentLeft),
                    Math.Max(ctx.DrawingRect.Bottom - pad - DpiScalingHelper.ScaleValue(16, Owner), ctx.RatingRect.Bottom + DpiScalingHelper.ScaleValue(4, Owner)),
                    DpiScalingHelper.ScaleValue(40, Owner),
                    DpiScalingHelper.ScaleValue(14, Owner));
            }

            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        public void DrawBackground(Graphics g, LayoutContext ctx) { }

        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw price
            if (!string.IsNullOrEmpty(ctx.SubtitleText)) // Price in SubtitleText
            {
                var priceBrush = PaintersFactory.GetSolidBrush(ctx.AccentColor);
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _priceFont, priceBrush, ctx.ParagraphRect, format);
            }
            
            // Draw rating stars (compact)
            if (ctx.ShowRating && ctx.Rating > 0)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, Color.FromArgb(255, 193, 7));
            }
            
            // Draw badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
        }

        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit) { }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
_disposed = true;
            }
        }
    }
}
