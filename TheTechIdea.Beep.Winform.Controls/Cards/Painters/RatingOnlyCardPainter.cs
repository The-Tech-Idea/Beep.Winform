using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// RatingOnlyCard - Card that displays only a star rating, centered.
    /// </summary>
    internal sealed class RatingOnlyCardPainter : ICardPainter
    {
        #region Fields
        
        private bool _disposed;
        
        private const int RatingWidth = 120;
        private const int RatingHeight = 24;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            // No custom resources needed
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Center rating in the card
            ctx.RatingRect = new Rectangle(
                drawingRect.Left + (drawingRect.Width - RatingWidth) / 2,
                drawingRect.Top + (drawingRect.Height - RatingHeight) / 2,
                RatingWidth,
                RatingHeight);
            
            // Hide everything else
            ctx.HeaderRect = Rectangle.Empty;
            ctx.ParagraphRect = Rectangle.Empty;
            ctx.ImageRect = Rectangle.Empty;
            ctx.ButtonRect = Rectangle.Empty;
            ctx.SecondaryButtonRect = Rectangle.Empty;
            ctx.BadgeRect = Rectangle.Empty;
            ctx.StatusRect = Rectangle.Empty;
            
            ctx.ShowImage = false;
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            ctx.ShowStatus = false;
            ctx.ShowRating = true;
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw star rating centered
            if (ctx.ShowRating && ctx.Rating > 0 && !ctx.RatingRect.IsEmpty)
            {
                CardRenderingHelpers.DrawStars(g, ctx.RatingRect, ctx.Rating, ctx.AccentColor);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.RatingRect.IsEmpty)
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
