using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// StatusOnlyCard - Card that displays only a status indicator with dot and text, centered.
    /// </summary>
    internal sealed class StatusOnlyCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        private Font _statusFont;
        
        private const int StatusWidth = 180;
        private const int StatusHeight = 24;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _statusFont?.Dispose(); } catch { }
            _statusFont = new Font(fontFamily, 10f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Center status in the card
            ctx.StatusRect = new Rectangle(
                drawingRect.Left + (drawingRect.Width - StatusWidth) / 2,
                drawingRect.Top + (drawingRect.Height - StatusHeight) / 2,
                StatusWidth,
                StatusHeight);
            
            // Hide everything else
            ctx.HeaderRect = Rectangle.Empty;
            ctx.ParagraphRect = Rectangle.Empty;
            ctx.ImageRect = Rectangle.Empty;
            ctx.ButtonRect = Rectangle.Empty;
            ctx.SecondaryButtonRect = Rectangle.Empty;
            ctx.BadgeRect = Rectangle.Empty;
            ctx.RatingRect = Rectangle.Empty;
            
            ctx.ShowImage = false;
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            ctx.ShowStatus = true;
            ctx.ShowRating = false;
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw status indicator with dot and text
            if (ctx.ShowStatus && !string.IsNullOrEmpty(ctx.StatusText) && !ctx.StatusRect.IsEmpty)
            {
                CardRenderingHelpers.DrawStatus(g, ctx.StatusRect, ctx.StatusText,
                    ctx.StatusColor, _statusFont);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.StatusRect.IsEmpty)
            {
                owner.AddHitArea("Status", ctx.StatusRect, null,
                    () => notifyAreaHit?.Invoke("Status", ctx.StatusRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            _statusFont?.Dispose();
            _disposed = true;
        }
        
        #endregion
    }
}
