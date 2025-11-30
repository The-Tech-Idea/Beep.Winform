using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _nameFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            try { _priceFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _nameFont = new Font(fontFamily, 12f, FontStyle.Bold);
            _descFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _priceFont = new Font(fontFamily, 14f, FontStyle.Bold);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Product image - 50% of card height
            int imageHeight = Math.Min(
                drawingRect.Width - Padding * 2,
                Math.Max(80, (int)(drawingRect.Height * ImageHeightPercent / 100f)));
            
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Top + Padding,
                drawingRect.Width - Padding * 2,
                imageHeight);
            
            // Sale/discount badge on image
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    ctx.ImageRect.Left + ElementGap,
                    ctx.ImageRect.Top + ElementGap,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Content area below image
            int contentTop = ctx.ImageRect.Bottom + Padding;
            int contentWidth = drawingRect.Width - Padding * 2;
            
            // Product name
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                contentTop,
                contentWidth,
                NameHeight);
            
            // Rating stars and price on same row
            ctx.RatingRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + ElementGap,
                contentWidth - PriceWidth - ElementGap,
                RatingHeight);
            
            // Price at right
            ctx.ParagraphRect = new Rectangle(
                ctx.HeaderRect.Right - PriceWidth,
                ctx.RatingRect.Top,
                PriceWidth,
                RatingHeight + 4);
            
            // Description below rating
            ctx.SubtitleRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.RatingRect.Bottom + ElementGap,
                contentWidth,
                DescHeight);
            
            // Add to Cart button at bottom
            ctx.ButtonRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Bottom - Padding - ButtonHeight,
                contentWidth,
                ButtonHeight);
            
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
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.ParagraphRect.IsEmpty)
            {
                using var brush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Far, 
                    LineAlignment = StringAlignment.Center 
                };
                g.DrawString(ctx.SubtitleText, _priceFont, brush, ctx.ParagraphRect, format);
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
            
            _nameFont?.Dispose();
            _descFont?.Dispose();
            _priceFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
