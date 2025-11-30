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
    /// DialogCard - Modal-style confirmation dialog with centered icon, title, message, and action buttons.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class DialogCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Dialog card fonts
        private Font _titleFont;
        private Font _messageFont;
        
        // Dialog card spacing
        private const int Padding = 24;
        private const int IconSize = 56;
        private const int TitleHeight = 28;
        private const int MessageHeight = 60;
        private const int ButtonHeight = 40;
        private const int ButtonWidth = 110;
        private const int ButtonGap = 16;
        private const int ElementGap = 16;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            var fontSize = owner?.Font?.Size ?? 10f;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _messageFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, fontSize + 2f, FontStyle.Bold);
            _messageFont = new Font(fontFamily, fontSize, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            int contentTop = drawingRect.Top + Padding;
            
            // Centered icon at top (optional)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - IconSize) / 2,
                    contentTop,
                    IconSize,
                    IconSize);
                contentTop = ctx.ImageRect.Bottom + ElementGap;
            }
            
            // Title (centered)
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                contentTop,
                drawingRect.Width - Padding * 2,
                TitleHeight);
            
            // Message/description (centered)
            ctx.ParagraphRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + ElementGap,
                ctx.HeaderRect.Width,
                MessageHeight);
            
            // Two action buttons at bottom (Cancel on left, Confirm on right)
            int buttonsTop = drawingRect.Bottom - Padding - ButtonHeight;
            int buttonsWidth = ButtonWidth * 2 + ButtonGap;
            int buttonsLeft = drawingRect.Left + (drawingRect.Width - buttonsWidth) / 2;
            
            // Secondary button (Cancel) - left
            ctx.SecondaryButtonRect = new Rectangle(
                buttonsLeft,
                buttonsTop,
                ButtonWidth,
                ButtonHeight);
            
            // Primary button (Confirm) - right
            ctx.ButtonRect = new Rectangle(
                ctx.SecondaryButtonRect.Right + ButtonGap,
                buttonsTop,
                ButtonWidth,
                ButtonHeight);
            
            ctx.ShowSecondaryButton = true;
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Dialog cards are intentionally minimal
            // Could add icon background circle if needed
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Subtle circle behind icon
                var circleRect = Rectangle.Inflate(ctx.ImageRect, 8, 8);
                using var brush = new SolidBrush(Color.FromArgb(15, ctx.AccentColor));
                g.FillEllipse(brush, circleRect);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Dialog cards don't typically have additional hit areas beyond buttons
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _messageFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
