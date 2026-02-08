using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// ContactOnlyCard - Card that displays contact information with avatar, header (name), 
    /// and paragraph (contact details), vertically centered.
    /// </summary>
    internal sealed class ContactOnlyCardPainter : ICardPainter
    {
        #region Fields
        
        private bool _disposed;
        
        private const int Padding = 16;
        private const int AvatarSize = 56;
        private const int HeaderHeight = 24;
        private const int DetailHeight = 60;
        private const int ElementGap = 10;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            // No custom resources needed
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            int totalHeight = AvatarSize + ElementGap + HeaderHeight + ElementGap / 2 + DetailHeight;
            int topY = drawingRect.Top + (drawingRect.Height - totalHeight) / 2;
            
            // Avatar/image centered at top
            ctx.AvatarRect = new Rectangle(
                drawingRect.Left + (drawingRect.Width - AvatarSize) / 2,
                topY,
                AvatarSize,
                AvatarSize);
            
            // Name/header centered below avatar
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.AvatarRect.Bottom + ElementGap,
                drawingRect.Width - Padding * 2,
                HeaderHeight);
            
            // Contact details (paragraph) below header
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                drawingRect.Width - Padding * 2,
                DetailHeight);
            
            // Hide everything else
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
            // Draw avatar circle placeholder
            if (!ctx.AvatarRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                using var bgBrush = new SolidBrush(Color.FromArgb(30, ctx.AccentColor));
                g.FillEllipse(bgBrush, ctx.AvatarRect);
                
                using var borderPen = new Pen(Color.FromArgb(60, ctx.AccentColor), 2);
                g.DrawEllipse(borderPen, ctx.AvatarRect);
                
                // Draw person silhouette icon
                int cx = ctx.AvatarRect.Left + ctx.AvatarRect.Width / 2;
                int cy = ctx.AvatarRect.Top + ctx.AvatarRect.Height / 2;
                int headSize = AvatarSize / 4;
                
                using var iconBrush = new SolidBrush(Color.FromArgb(80, ctx.AccentColor));
                
                // Head
                g.FillEllipse(iconBrush, cx - headSize / 2, cy - headSize, headSize, headSize);
                
                // Body (arc)
                var bodyRect = new Rectangle(cx - headSize, cy + 2, headSize * 2, headSize);
                g.FillEllipse(iconBrush, bodyRect);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.AvatarRect.IsEmpty)
            {
                owner.AddHitArea("Avatar", ctx.AvatarRect, null,
                    () => notifyAreaHit?.Invoke("Avatar", ctx.AvatarRect));
            }
            
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
