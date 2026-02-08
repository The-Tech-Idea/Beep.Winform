using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// SecondaryButtonOnlyCard - Card that displays only a secondary action button, centered.
    /// </summary>
    internal sealed class SecondaryButtonOnlyCardPainter : ICardPainter
    {
        #region Fields
        
        private bool _disposed;
        
        private const int ButtonWidth = 140;
        private const int ButtonHeight = 40;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            // No custom resources needed
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Center secondary button in the card
            ctx.SecondaryButtonRect = new Rectangle(
                drawingRect.Left + (drawingRect.Width - ButtonWidth) / 2,
                drawingRect.Top + (drawingRect.Height - ButtonHeight) / 2,
                ButtonWidth,
                ButtonHeight);
            
            // Hide everything else
            ctx.HeaderRect = Rectangle.Empty;
            ctx.ParagraphRect = Rectangle.Empty;
            ctx.ImageRect = Rectangle.Empty;
            ctx.ButtonRect = Rectangle.Empty;
            ctx.BadgeRect = Rectangle.Empty;
            ctx.RatingRect = Rectangle.Empty;
            ctx.StatusRect = Rectangle.Empty;
            
            ctx.ShowImage = false;
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = true;
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
            if (!ctx.SecondaryButtonRect.IsEmpty)
            {
                owner.AddHitArea("SecondaryButton", ctx.SecondaryButtonRect, null,
                    () => notifyAreaHit?.Invoke("SecondaryButton", ctx.SecondaryButtonRect));
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
