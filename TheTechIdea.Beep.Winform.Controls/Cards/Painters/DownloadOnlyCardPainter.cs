using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// DownloadOnlyCard - Card that displays a download icon with a download button, centered.
    /// </summary>
    internal sealed class DownloadOnlyCardPainter : ICardPainter
    {
        #region Fields

        private BaseControl _owner;
        private bool _disposed;
        
        private const int IconSize = 48;
        private const int ButtonWidth = 120;
        private const int ButtonHeight = 36;
        private const int ElementGap = 16;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            // No custom resources needed
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            int totalHeight = IconSize + ElementGap + ButtonHeight;
            int topY = drawingRect.Top + (drawingRect.Height - totalHeight) / 2;
            
            // Download icon area (centered)
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + (drawingRect.Width - IconSize) / 2,
                topY,
                IconSize,
                IconSize);
            
            // Download button below icon (centered)
            ctx.ButtonRect = new Rectangle(
                drawingRect.Left + (drawingRect.Width - ButtonWidth) / 2,
                ctx.ImageRect.Bottom + ElementGap,
                ButtonWidth,
                ButtonHeight);
            
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
            // Draw download arrow icon if no image is provided
            if (!ctx.ImageRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Draw circular background
                var bgRect = new Rectangle(
                    ctx.ImageRect.X - DpiScalingHelper.ScaleValue(8, _owner),
                    ctx.ImageRect.Y - DpiScalingHelper.ScaleValue(8, _owner),
                    ctx.ImageRect.Width + DpiScalingHelper.ScaleValue(16, _owner),
                    ctx.ImageRect.Height + DpiScalingHelper.ScaleValue(16, _owner));

                g.FillEllipse(CardPaintCache.Brush(Color.FromArgb(25, ctx.AccentColor)), bgRect);

                g.DrawEllipse(CardPaintCache.Pen(Color.FromArgb(40, ctx.AccentColor), DpiScalingHelper.ScaleValue(2, _owner)), bgRect);
                
                // Draw download arrow
                int cx = ctx.ImageRect.Left + ctx.ImageRect.Width / 2;
                int cy = ctx.ImageRect.Top + ctx.ImageRect.Height / 2;
                int arrowSize = IconSize / 3;
                
                using var arrowPen = new Pen(ctx.AccentColor, 3f);
                arrowPen.StartCap = LineCap.Round;
                arrowPen.EndCap = LineCap.Round;
                arrowPen.LineJoin = LineJoin.Round;
                
                // Vertical line
                g.DrawLine(arrowPen, cx, cy - arrowSize, cx, cy + arrowSize / 2);
                
                // Arrow head
                g.DrawLines(arrowPen, new Point[]
                {
                    new Point(cx - arrowSize / 2, cy),
                    new Point(cx, cy + arrowSize / 2),
                    new Point(cx + arrowSize / 2, cy)
                });
                
                // Tray/platform line
                g.DrawLine(arrowPen, cx - arrowSize, cy + arrowSize, cx + arrowSize, cy + arrowSize);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.ButtonRect.IsEmpty)
            {
                owner.AddHitArea("Download", ctx.ButtonRect, null,
                    () => notifyAreaHit?.Invoke("Download", ctx.ButtonRect));
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
