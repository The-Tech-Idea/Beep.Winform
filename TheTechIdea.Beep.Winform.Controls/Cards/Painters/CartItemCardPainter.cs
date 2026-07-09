using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _variantFont = bodyFont;
            _priceFont = titleFont;
            _quantityFont = bodyFont;
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
            
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                var subtitleColor = Color.FromArgb(180, _theme?.CardTextForeColor ?? _owner?.ForeColor ?? Color.Black);
                TextRenderer.DrawText(g, ctx.SubtitleText, _variantFont, ctx.SubtitleRect, subtitleColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
            }

            // Draw quantity controls
            if (!ctx.ButtonRect.IsEmpty)
            {
                DrawQuantityControls(g, ctx);
            }
            
            // Draw total price
            if (!ctx.RatingRect.IsEmpty && !string.IsNullOrEmpty(ctx.StatusText))
            {
                TextRenderer.DrawText(g, ctx.StatusText, _priceFont, ctx.RatingRect, ctx.AccentColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
            }
            
            // Draw remove button
            if (!ctx.SecondaryButtonRect.IsEmpty)
            {
                DrawRemoveButton(g, ctx);
            }
            
            // Draw thumbnail border
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                var borderPen = CardPaintCache.Pen(Color.FromArgb(30, _theme?.CardTextForeColor ?? Color.Black), DpiScalingHelper.ScaleValue(1, _owner));
                using var borderPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, DpiScalingHelper.ScaleValue(4, _owner));
                g.DrawPath(borderPen, borderPath);
            }
        }
        
        private void DrawQuantityControls(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            var rect = ctx.ButtonRect;
            
            // Background
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(rect, DpiScalingHelper.ScaleValue(6, _owner));
            var bgBrush = CardPaintCache.Brush(Color.FromArgb(245, 245, 245));
            g.FillPath(bgBrush, bgPath);

            // Border
            var borderPen = CardPaintCache.Pen(Color.FromArgb(200, 200, 200), DpiScalingHelper.ScaleValue(1, _owner));
            g.DrawPath(borderPen, bgPath);

            const TextFormatFlags centerFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
            var buttonColor = Color.FromArgb(100, _theme?.CardTextForeColor ?? Color.Black);

            // Minus button
            var minusRect = new Rectangle(rect.Left + DpiScalingHelper.ScaleValue(2, _owner), rect.Top + DpiScalingHelper.ScaleValue(2, _owner), QuantityButtonSize, rect.Height - DpiScalingHelper.ScaleValue(4, _owner));
            TextRenderer.DrawText(g, "−", _quantityFont, minusRect, buttonColor, centerFlags);

            // Quantity value (use BadgeText1 for quantity)
            string qty = ctx.BadgeText1 ?? "1";
            var qtyRect = new Rectangle(minusRect.Right, rect.Top, rect.Width - QuantityButtonSize * 2 - DpiScalingHelper.ScaleValue(4, _owner), rect.Height);
            TextRenderer.DrawText(g, qty, _quantityFont, qtyRect, _theme?.CardTextForeColor ?? Color.Black, centerFlags);

            // Plus button
            var plusRect = new Rectangle(rect.Right - QuantityButtonSize - DpiScalingHelper.ScaleValue(2, _owner), rect.Top + DpiScalingHelper.ScaleValue(2, _owner), QuantityButtonSize, rect.Height - DpiScalingHelper.ScaleValue(4, _owner));
            TextRenderer.DrawText(g, "+", _quantityFont, plusRect, buttonColor, centerFlags);
        }
        
        private void DrawRemoveButton(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            var rect = ctx.SecondaryButtonRect;
            
            // Hover background
            var bgBrush = CardPaintCache.Brush(Color.FromArgb(10, _theme?.CardTextForeColor ?? Color.Black));
            g.FillEllipse(bgBrush, rect);

            // X icon
            int margin = DpiScalingHelper.ScaleValue(6, _owner);
            using var pen = new Pen(Color.FromArgb(100, _theme?.CardTextForeColor ?? Color.Black), DpiScalingHelper.ScaleValue(2, _owner));
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
_disposed = true;
        }
        
        #endregion
    }
}

