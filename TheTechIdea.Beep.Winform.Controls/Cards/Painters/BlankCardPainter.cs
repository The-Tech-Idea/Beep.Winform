using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// BlankCard - Empty card with no content. Only draws the card background/border via BaseControl.
    /// Default card style.
    /// </summary>
    internal sealed class BlankCardPainter : ICardPainter
    {
        #region Fields
        
        private bool _disposed;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            // No resources to initialize for blank card
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Hide everything - blank card shows nothing
            ctx.HeaderRect = Rectangle.Empty;
            ctx.ParagraphRect = Rectangle.Empty;
            ctx.ImageRect = Rectangle.Empty;
            ctx.ButtonRect = Rectangle.Empty;
            ctx.SecondaryButtonRect = Rectangle.Empty;
            ctx.BadgeRect = Rectangle.Empty;
            ctx.RatingRect = Rectangle.Empty;
            ctx.StatusRect = Rectangle.Empty;
            ctx.SubtitleRect = Rectangle.Empty;
            ctx.AvatarRect = Rectangle.Empty;
            ctx.TagsRect = Rectangle.Empty;
            
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
            // No foreground accents for blank card
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // No hit areas for blank card
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
