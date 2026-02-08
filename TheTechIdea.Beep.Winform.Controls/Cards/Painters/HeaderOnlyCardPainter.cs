using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// HeaderOnlyCard - Card that displays only header text, vertically centered.
    /// </summary>
    internal sealed class HeaderOnlyCardPainter : ICardPainter
    {
        #region Fields
        
        private bool _disposed;
        
        private const int Padding = 16;
        private const int HeaderHeight = 32;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            // No custom resources needed - uses BaseControl fonts
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Center header vertically
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Top + (drawingRect.Height - HeaderHeight) / 2,
                drawingRect.Width - Padding * 2,
                HeaderHeight);
            
            // Hide everything else
            ctx.ParagraphRect = Rectangle.Empty;
            ctx.ImageRect = Rectangle.Empty;
            ctx.ButtonRect = Rectangle.Empty;
            ctx.SecondaryButtonRect = Rectangle.Empty;
            ctx.BadgeRect = Rectangle.Empty;
            ctx.RatingRect = Rectangle.Empty;
            ctx.StatusRect = Rectangle.Empty;
            
            ctx.ShowImage = false;
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            ctx.ShowStatus = false;
            ctx.ShowRating = false;
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // No foreground accents
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Header hit area
            if (!ctx.HeaderRect.IsEmpty)
            {
                owner.AddHitArea("Header", ctx.HeaderRect, null,
                    () => notifyAreaHit?.Invoke("Header", ctx.HeaderRect));
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
