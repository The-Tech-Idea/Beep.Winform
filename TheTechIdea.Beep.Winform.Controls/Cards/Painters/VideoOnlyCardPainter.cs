using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// VideoOnlyCard - Card that displays only a video thumbnail with play button overlay.
    /// </summary>
    internal sealed class VideoOnlyCardPainter : ICardPainter
    {
        #region Fields
        
        private bool _disposed;
        
        private const int PlayButtonSize = 56;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            // No custom resources needed
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Image fills the entire card (video thumbnail)
            ctx.ImageRect = new Rectangle(
                drawingRect.Left,
                drawingRect.Top,
                drawingRect.Width,
                drawingRect.Height);
            
            // Play button centered on thumbnail
            ctx.ButtonRect = new Rectangle(
                drawingRect.Left + (drawingRect.Width - PlayButtonSize) / 2,
                drawingRect.Top + (drawingRect.Height - PlayButtonSize) / 2,
                PlayButtonSize,
                PlayButtonSize);
            
            // Hide everything else
            ctx.HeaderRect = Rectangle.Empty;
            ctx.ParagraphRect = Rectangle.Empty;
            ctx.SecondaryButtonRect = Rectangle.Empty;
            ctx.BadgeRect = Rectangle.Empty;
            ctx.RatingRect = Rectangle.Empty;
            ctx.StatusRect = Rectangle.Empty;
            
            ctx.ShowImage = true;
            ctx.ShowButton = true;
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
            // Draw play button overlay
            if (!ctx.ButtonRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Semi-transparent circle background
                using var circleBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
                g.FillEllipse(circleBrush, ctx.ButtonRect);
                
                // Play triangle
                int triangleSize = ctx.ButtonRect.Width / 3;
                int cx = ctx.ButtonRect.Left + ctx.ButtonRect.Width / 2;
                int cy = ctx.ButtonRect.Top + ctx.ButtonRect.Height / 2;
                int offsetX = 3;
                
                Point[] triangle = new Point[]
                {
                    new Point(cx - triangleSize / 2 + offsetX, cy - triangleSize / 2),
                    new Point(cx - triangleSize / 2 + offsetX, cy + triangleSize / 2),
                    new Point(cx + triangleSize / 2 + offsetX, cy)
                };
                
                using var triangleBrush = new SolidBrush(Color.White);
                g.FillPolygon(triangleBrush, triangle);
            }
            
            // Draw gradient overlay at bottom of thumbnail
            if (!ctx.ImageRect.IsEmpty && ctx.ImageRect.Height > 50)
            {
                var gradientRect = new Rectangle(
                    ctx.ImageRect.Left,
                    ctx.ImageRect.Bottom - 50,
                    ctx.ImageRect.Width,
                    50);
                
                using var gradientBrush = new LinearGradientBrush(
                    new Point(gradientRect.Left, gradientRect.Top),
                    new Point(gradientRect.Left, gradientRect.Bottom),
                    Color.FromArgb(0, 0, 0, 0),
                    Color.FromArgb(120, 0, 0, 0));
                
                g.FillRectangle(gradientBrush, gradientRect);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.ButtonRect.IsEmpty)
            {
                owner.AddHitArea("Play", ctx.ButtonRect, null,
                    () => notifyAreaHit?.Invoke("Play", ctx.ButtonRect));
            }
            
            if (!ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Thumbnail", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Thumbnail", ctx.ImageRect));
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
