using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// OfferCard - Special offer/deal card with discount badge.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class OfferCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Offer card fonts
        private Font _titleFont;
        private Font _descFont;
        private Font _priceFont;
        private Font _originalPriceFont;
        private Font _discountFont;
        private Font _badgeFont;
        
        // Offer card spacing
        private const int Padding = 16;
        private const int ImageHeight = 120;
        private const int TitleHeight = 28;
        private const int DescHeight = 40;
        private const int PriceHeight = 32;
        private const int DiscountBadgeSize = 60;
        private const int ButtonHeight = 40;
        private const int ElementGap = 10;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            try { _priceFont?.Dispose(); } catch { }
            try { _originalPriceFont?.Dispose(); } catch { }
            try { _discountFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 13f, FontStyle.Bold);
            _descFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _priceFont = new Font(fontFamily, 20f, FontStyle.Bold);
            _originalPriceFont = new Font(fontFamily, 11f, FontStyle.Strikeout);
            _discountFont = new Font(fontFamily, 12f, FontStyle.Bold);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Product image at top
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left,
                    drawingRect.Top,
                    drawingRect.Width,
                    ImageHeight);
            }
            
            // Discount badge (top-right corner, overlapping image)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - DiscountBadgeSize,
                    drawingRect.Top + Padding,
                    DiscountBadgeSize,
                    DiscountBadgeSize);
            }
            
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + Padding : drawingRect.Top + Padding;
            
            // Title
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                contentTop,
                drawingRect.Width - Padding * 2,
                TitleHeight);
            
            // Description
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                drawingRect.Width - Padding * 2,
                DescHeight);
            
            // Price area (original + sale price)
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.ParagraphRect.Bottom + ElementGap,
                drawingRect.Width - Padding * 2,
                PriceHeight);
            
            // Expiry/countdown
            if (!string.IsNullOrEmpty(ctx.StatusText))
            {
                ctx.StatusRect = new Rectangle(
                    drawingRect.Left + Padding,
                    ctx.SubtitleRect.Bottom + ElementGap / 2,
                    drawingRect.Width - Padding * 2,
                    20);
            }
            
            // CTA button
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Bottom - Padding - ButtonHeight,
                    drawingRect.Width - Padding * 2,
                    ButtonHeight);
            }
            
            ctx.ShowSecondaryButton = false;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw discount badge (circular)
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                DrawDiscountBadge(g, ctx);
            }
            
            // Draw prices
            if (!ctx.SubtitleRect.IsEmpty)
            {
                DrawPrices(g, ctx);
            }
            
            // Draw expiry countdown
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.StatusRect.IsEmpty)
            {
                DrawExpiryInfo(g, ctx);
            }
            
            // Draw "Limited Time" ribbon on image
            if (ctx.ShowImage && ctx.ShowStatus && !ctx.ImageRect.IsEmpty)
            {
                DrawRibbon(g, ctx);
            }
        }
        
        private void DrawDiscountBadge(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Circular badge
            using var bgBrush = new SolidBrush(Color.FromArgb(220, 53, 69)); // Red
            g.FillEllipse(bgBrush, ctx.BadgeRect);
            
            // Discount text
            using var textBrush = new SolidBrush(Color.White);
            var format = new StringFormat 
            { 
                Alignment = StringAlignment.Center, 
                LineAlignment = StringAlignment.Center 
            };
            g.DrawString(ctx.BadgeText1, _discountFont, textBrush, ctx.BadgeRect, format);
        }
        
        private void DrawPrices(Graphics g, LayoutContext ctx)
        {
            // Parse prices from SubtitleText (format: "$99.99|$149.99" or just "$99.99")
            string[] prices = (ctx.SubtitleText ?? "").Split('|');
            
            if (prices.Length >= 1)
            {
                // Sale price (large, accent color)
                using var saleBrush = new SolidBrush(ctx.AccentColor);
                var saleRect = new Rectangle(ctx.SubtitleRect.Left, ctx.SubtitleRect.Top, 100, ctx.SubtitleRect.Height);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(prices[0].Trim(), _priceFont, saleBrush, saleRect, format);
                
                // Original price (strikethrough)
                if (prices.Length >= 2)
                {
                    using var origBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    var origRect = new Rectangle(saleRect.Right + 10, ctx.SubtitleRect.Top + 8, 80, ctx.SubtitleRect.Height - 8);
                    g.DrawString(prices[1].Trim(), _originalPriceFont, origBrush, origRect, format);
                }
            }
        }
        
        private void DrawExpiryInfo(Graphics g, LayoutContext ctx)
        {
            // Clock icon
            int iconSize = 14;
            var iconRect = new Rectangle(ctx.StatusRect.Left, ctx.StatusRect.Top + 3, iconSize, iconSize);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var iconPen = new Pen(Color.FromArgb(220, 53, 69), 1.5f);
            g.DrawEllipse(iconPen, iconRect);
            
            int cx = iconRect.Left + iconRect.Width / 2;
            int cy = iconRect.Top + iconRect.Height / 2;
            g.DrawLine(iconPen, cx, cy - 3, cx, cy);
            g.DrawLine(iconPen, cx, cy, cx + 3, cy);
            
            // Expiry text
            var textRect = new Rectangle(
                iconRect.Right + 6,
                ctx.StatusRect.Top,
                ctx.StatusRect.Width - iconSize - 6,
                ctx.StatusRect.Height);
            
            using var textBrush = new SolidBrush(Color.FromArgb(220, 53, 69));
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.StatusText, _badgeFont, textBrush, textRect, format);
        }
        
        private void DrawRibbon(Graphics g, LayoutContext ctx)
        {
            // Corner ribbon
            int ribbonWidth = 100;
            int ribbonHeight = 24;
            
            var ribbonRect = new Rectangle(
                ctx.ImageRect.Left,
                ctx.ImageRect.Top + 20,
                ribbonWidth,
                ribbonHeight);
            
            using var ribbonBrush = new SolidBrush(ctx.StatusColor != Color.Empty ? ctx.StatusColor : Color.FromArgb(255, 193, 7));
            g.FillRectangle(ribbonBrush, ribbonRect);
            
            // Ribbon text
            using var textBrush = new SolidBrush(Color.Black);
            var format = new StringFormat 
            { 
                Alignment = StringAlignment.Center, 
                LineAlignment = StringAlignment.Center 
            };
            g.DrawString("LIMITED TIME", _badgeFont, textBrush, ribbonRect, format);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Image hit area
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Image", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Image", ctx.ImageRect));
            }
            
            // Discount badge hit area
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Discount", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Discount", ctx.BadgeRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _descFont?.Dispose();
            _priceFont?.Dispose();
            _originalPriceFont?.Dispose();
            _discountFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

