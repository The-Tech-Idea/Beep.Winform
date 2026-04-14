using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// ProductCard - E-commerce product display with image, price, rating, and add to cart.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class ProductCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Product card fonts
        private Font _nameFont;
        private Font _descFont;
        private Font _priceFont;
        private Font _badgeFont;
        
        // Product card spacing
        private const int Padding = 12;
        private const int ImageHeightPercent = 50;
        private const int NameHeight = 24;
        private const int DescHeight = 18;
        private const int RatingHeight = 18;
        private const int PriceWidth = 80;
        private const int ButtonHeight = 36;
        private const int BadgeWidth = 50;
        private const int BadgeHeight = 22;
        private const int ElementGap = 6;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_nameFont = titleFont;
            _descFont = bodyFont;
            _priceFont = titleFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int nameHeight = DpiScalingHelper.ScaleValue(NameHeight, _owner);
            int descHeight = DpiScalingHelper.ScaleValue(DescHeight, _owner);
            int ratingHeight = DpiScalingHelper.ScaleValue(RatingHeight, _owner);
            int priceWidth = DpiScalingHelper.ScaleValue(PriceWidth, _owner);
            int buttonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Product image - 50% of card height
            int imageHeight = Math.Min(
                drawingRect.Width - padding * 2,
                Math.Max(80, (int)(drawingRect.Height * ImageHeightPercent / 100f)));
            
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + padding,
                drawingRect.Top + padding,
                drawingRect.Width - padding * 2,
                imageHeight);
            
            // Sale/discount badge on image
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    ctx.ImageRect.Left + elementGap,
                    ctx.ImageRect.Top + elementGap,
                    badgeWidth,
                    badgeHeight);
            }
            
            // Content area below image
            int contentTop = ctx.ImageRect.Bottom + padding;
            int contentWidth = drawingRect.Width - padding * 2;
            
            // Product name
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + padding,
                contentTop,
                contentWidth,
                nameHeight);
            
            // Rating stars and price on same row
            ctx.RatingRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + elementGap,
                contentWidth - priceWidth - elementGap,
                ratingHeight);
            
            // Price at right
            ctx.SubtitleRect = new Rectangle(
                ctx.HeaderRect.Right - priceWidth,
                ctx.RatingRect.Top,
                priceWidth,
                ratingHeight + DpiScalingHelper.ScaleValue(4, _owner));
            
            // Description below rating
            ctx.ParagraphRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.RatingRect.Bottom + elementGap,
                contentWidth,
                descHeight);
            
            // Add to Cart button at bottom
            ctx.ButtonRect = new Rectangle(
                drawingRect.Left + padding,
                drawingRect.Bottom - padding - buttonHeight,
                contentWidth,
                buttonHeight);
            
            ctx.ShowSecondaryButton = false;
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw sale/discount badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw rating stars
            if (ctx.ShowRating && ctx.Rating > 0 && !ctx.RatingRect.IsEmpty)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, ctx.AccentColor);
            }
            
            // Draw price
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                using var brush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Far, 
                    LineAlignment = StringAlignment.Center 
                };
                g.DrawString(ctx.SubtitleText, _priceFont, brush, ctx.SubtitleRect, format);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Badge", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Badge", ctx.BadgeRect));
            }
            
            if (ctx.ShowRating && !ctx.RatingRect.IsEmpty)
            {
                owner.AddHitArea("Rating", ctx.RatingRect, null,
                    () => notifyAreaHit?.Invoke("Rating", ctx.RatingRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
_disposed = true;
        }
        
        #endregion
    }
}
