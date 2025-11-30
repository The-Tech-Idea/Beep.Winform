using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// CartItemCard - Shopping cart item with quantity controls.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class CartItemCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Cart item card fonts
        private Font _titleFont;
        private Font _variantFont;
        private Font _priceFont;
        private Font _quantityFont;
        
        // Cart item card spacing
        private const int Padding = 12;
        private const int ImageSize = 80;
        private const int TitleHeight = 22;
        private const int VariantHeight = 18;
        private const int PriceHeight = 24;
        private const int QuantityControlWidth = 100;
        private const int QuantityControlHeight = 32;
        private const int QuantityButtonSize = 28;
        private const int RemoveButtonSize = 24;
        private const int ElementGap = 6;
        private const int ContentGap = 14;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _variantFont?.Dispose(); } catch { }
            try { _priceFont?.Dispose(); } catch { }
            try { _quantityFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 11f, FontStyle.Bold);
            _variantFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _priceFont = new Font(fontFamily, 12f, FontStyle.Bold);
            _quantityFont = new Font(fontFamily, 11f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Product thumbnail (left)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Top + (drawingRect.Height - ImageSize) / 2,
                    ImageSize,
                    ImageSize);
            }
            
            int contentLeft = drawingRect.Left + Padding + (ctx.ShowImage ? ImageSize + ContentGap : 0);
            int rightAreaWidth = Math.Max(QuantityControlWidth, 80) + Padding;
            int contentWidth = drawingRect.Width - (contentLeft - drawingRect.Left) - rightAreaWidth;
            
            // Product title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + Padding,
                contentWidth,
                TitleHeight);
            
            // Variant info (size, color)
            ctx.SubtitleRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                contentWidth,
                VariantHeight);
            
            // Unit price
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.SubtitleRect.Bottom + ElementGap,
                contentWidth / 2,
                PriceHeight);
            
            // Quantity controls (right side)
            ctx.ButtonRect = new Rectangle(
                drawingRect.Right - Padding - QuantityControlWidth,
                drawingRect.Top + (drawingRect.Height - QuantityControlHeight) / 2,
                QuantityControlWidth,
                QuantityControlHeight);
            
            // Total price (below quantity)
            ctx.RatingRect = new Rectangle(
                ctx.ButtonRect.Left,
                ctx.ButtonRect.Bottom + ElementGap,
                QuantityControlWidth,
                PriceHeight);
            
            // Remove button (top-right)
            ctx.SecondaryButtonRect = new Rectangle(
                drawingRect.Right - Padding - RemoveButtonSize,
                drawingRect.Top + Padding,
                RemoveButtonSize,
                RemoveButtonSize);
            
            ctx.ShowButton = true;
            ctx.ShowSecondaryButton = true;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw quantity controls
            if (!ctx.ButtonRect.IsEmpty)
            {
                DrawQuantityControls(g, ctx);
            }
            
            // Draw total price
            if (!ctx.RatingRect.IsEmpty && !string.IsNullOrEmpty(ctx.StatusText))
            {
                using var brush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Far, 
                    LineAlignment = StringAlignment.Center 
                };
                g.DrawString(ctx.StatusText, _priceFont, brush, ctx.RatingRect, format);
            }
            
            // Draw remove button
            if (!ctx.SecondaryButtonRect.IsEmpty)
            {
                DrawRemoveButton(g, ctx);
            }
            
            // Draw thumbnail border
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                using var borderPen = new Pen(Color.FromArgb(30, Color.Black), 1);
                using var borderPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, 4);
                g.DrawPath(borderPen, borderPath);
            }
        }
        
        private void DrawQuantityControls(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            var rect = ctx.ButtonRect;
            
            // Background
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(rect, 6);
            using var bgBrush = new SolidBrush(Color.FromArgb(245, 245, 245));
            g.FillPath(bgBrush, bgPath);
            
            // Border
            using var borderPen = new Pen(Color.FromArgb(200, 200, 200), 1);
            g.DrawPath(borderPen, bgPath);
            
            // Minus button
            var minusRect = new Rectangle(rect.Left + 2, rect.Top + 2, QuantityButtonSize, rect.Height - 4);
            using var buttonBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("−", _quantityFont, buttonBrush, minusRect, format);
            
            // Quantity value (use BadgeText1 for quantity)
            string qty = ctx.BadgeText1 ?? "1";
            var qtyRect = new Rectangle(minusRect.Right, rect.Top, rect.Width - QuantityButtonSize * 2 - 4, rect.Height);
            using var qtyBrush = new SolidBrush(Color.Black);
            g.DrawString(qty, _quantityFont, qtyBrush, qtyRect, format);
            
            // Plus button
            var plusRect = new Rectangle(rect.Right - QuantityButtonSize - 2, rect.Top + 2, QuantityButtonSize, rect.Height - 4);
            g.DrawString("+", _quantityFont, buttonBrush, plusRect, format);
        }
        
        private void DrawRemoveButton(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            var rect = ctx.SecondaryButtonRect;
            
            // Hover background
            using var bgBrush = new SolidBrush(Color.FromArgb(10, Color.Black));
            g.FillEllipse(bgBrush, rect);
            
            // X icon
            int margin = 6;
            using var pen = new Pen(Color.FromArgb(100, Color.Black), 2);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            
            g.DrawLine(pen,
                rect.Left + margin, rect.Top + margin,
                rect.Right - margin, rect.Bottom - margin);
            g.DrawLine(pen,
                rect.Right - margin, rect.Top + margin,
                rect.Left + margin, rect.Bottom - margin);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Quantity minus button
            if (!ctx.ButtonRect.IsEmpty)
            {
                var minusRect = new Rectangle(ctx.ButtonRect.Left, ctx.ButtonRect.Top, QuantityButtonSize, ctx.ButtonRect.Height);
                owner.AddHitArea("Minus", minusRect, null,
                    () => notifyAreaHit?.Invoke("Minus", minusRect));
                
                var plusRect = new Rectangle(ctx.ButtonRect.Right - QuantityButtonSize, ctx.ButtonRect.Top, QuantityButtonSize, ctx.ButtonRect.Height);
                owner.AddHitArea("Plus", plusRect, null,
                    () => notifyAreaHit?.Invoke("Plus", plusRect));
            }
            
            // Remove button
            if (!ctx.SecondaryButtonRect.IsEmpty)
            {
                owner.AddHitArea("Remove", ctx.SecondaryButtonRect, null,
                    () => notifyAreaHit?.Invoke("Remove", ctx.SecondaryButtonRect));
            }
            
            // Image hit area
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Image", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Image", ctx.ImageRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _variantFont?.Dispose();
            _priceFont?.Dispose();
            _quantityFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

