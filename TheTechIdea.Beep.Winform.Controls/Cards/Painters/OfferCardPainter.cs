using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _descFont = bodyFont;
            _priceFont = titleFont;
            _originalPriceFont = titleFont;
            _discountFont = bodyFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int imageHeight = DpiScalingHelper.ScaleValue(ImageHeight, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int descHeight = DpiScalingHelper.ScaleValue(DescHeight, _owner);
            int priceHeight = DpiScalingHelper.ScaleValue(PriceHeight, _owner);
            int discountBadgeSize = DpiScalingHelper.ScaleValue(DiscountBadgeSize, _owner);
            int buttonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Product image at top
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left,
                    drawingRect.Top,
                    drawingRect.Width,
                    imageHeight);
            }
            
            // Discount badge (top-right corner, overlapping image)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - padding - discountBadgeSize,
                    drawingRect.Top + padding,
                    discountBadgeSize,
                    discountBadgeSize);
            }
            
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + padding : drawingRect.Top + padding;
            
            // Title
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + padding,
                contentTop,
                drawingRect.Width - padding * 2,
                titleHeight);
            
            // Description
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + padding,
                ctx.HeaderRect.Bottom + elementGap / 2,
                drawingRect.Width - padding * 2,
                descHeight);
            
            // Price area (original + sale price)
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + padding,
                ctx.ParagraphRect.Bottom + elementGap,
                drawingRect.Width - padding * 2,
                priceHeight);
            
            // Expiry/countdown
            if (!string.IsNullOrEmpty(ctx.StatusText))
            {
                ctx.StatusRect = new Rectangle(
                    drawingRect.Left + padding,
                    ctx.SubtitleRect.Bottom + elementGap / 2,
                    drawingRect.Width - padding * 2,
                    DpiScalingHelper.ScaleValue(20, _owner));
            }
            
            // CTA button
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + padding,
                    drawingRect.Bottom - padding - buttonHeight,
                    drawingRect.Width - padding * 2,
                    buttonHeight);
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
                var saleRect = new Rectangle(ctx.SubtitleRect.Left, ctx.SubtitleRect.Top, DpiScalingHelper.ScaleValue(100, _owner), ctx.SubtitleRect.Height);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(prices[0].Trim(), _priceFont, saleBrush, saleRect, format);
                
                // Original price (strikethrough)
                if (prices.Length >= 2)
                {
                    using var origBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    var origRect = new Rectangle(
                        saleRect.Right + DpiScalingHelper.ScaleValue(10, _owner),
                        ctx.SubtitleRect.Top + DpiScalingHelper.ScaleValue(8, _owner),
                        DpiScalingHelper.ScaleValue(80, _owner),
                        ctx.SubtitleRect.Height - DpiScalingHelper.ScaleValue(8, _owner));
                    g.DrawString(prices[1].Trim(), _originalPriceFont, origBrush, origRect, format);
                }
            }
        }
        
        private void DrawExpiryInfo(Graphics g, LayoutContext ctx)
        {
            // Clock icon
            int iconSize = DpiScalingHelper.ScaleValue(14, _owner);
            var iconRect = new Rectangle(ctx.StatusRect.Left, ctx.StatusRect.Top + DpiScalingHelper.ScaleValue(3, _owner), iconSize, iconSize);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var iconPen = new Pen(Color.FromArgb(220, 53, 69), DpiScalingHelper.ScaleValue(2, _owner));
            g.DrawEllipse(iconPen, iconRect);
            
            int cx = iconRect.Left + iconRect.Width / 2;
            int cy = iconRect.Top + iconRect.Height / 2;
            g.DrawLine(iconPen, cx, cy - DpiScalingHelper.ScaleValue(3, _owner), cx, cy);
            g.DrawLine(iconPen, cx, cy, cx + DpiScalingHelper.ScaleValue(3, _owner), cy);
            
            // Expiry text
            var textRect = new Rectangle(
                iconRect.Right + 6,
                ctx.StatusRect.Top,
                ctx.StatusRect.Width - iconSize - DpiScalingHelper.ScaleValue(6, _owner),
                ctx.StatusRect.Height);
            
            using var textBrush = new SolidBrush(Color.FromArgb(220, 53, 69));
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.StatusText, _badgeFont, textBrush, textRect, format);
        }
        
        private void DrawRibbon(Graphics g, LayoutContext ctx)
        {
            // Corner ribbon
            int ribbonWidth = DpiScalingHelper.ScaleValue(100, _owner);
            int ribbonHeight = DpiScalingHelper.ScaleValue(24, _owner);
            
            var ribbonRect = new Rectangle(
                ctx.ImageRect.Left,
                ctx.ImageRect.Top + DpiScalingHelper.ScaleValue(20, _owner),
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
_disposed = true;
        }
        
        #endregion
    }
}

