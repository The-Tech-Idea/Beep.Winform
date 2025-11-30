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
    /// BasicCard - Minimal card for general content with accent line.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class BasicCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Basic card spacing
        private const int Padding = 16;
        private const int HeaderHeight = 24;
        private const int ButtonHeight = 36;
        private const int ButtonWidth = 100;
        private const int AccentLineWidth = 40;
        private const int AccentLineThickness = 3;
        private const int ElementGap = 8;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Header at top with space for accent line
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Top + Padding + AccentLineThickness + ElementGap,
                drawingRect.Width - Padding * 2,
                HeaderHeight);
            
            // Calculate paragraph height based on available space
            int availableHeight = drawingRect.Height - HeaderHeight - ButtonHeight - (Padding * 3) - AccentLineThickness;
            int paragraphHeight = Math.Max(40, availableHeight);
            
            ctx.ParagraphRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + ElementGap,
                ctx.HeaderRect.Width,
                paragraphHeight);
            
            // Button at bottom-right
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Right - Padding - ButtonWidth,
                    drawingRect.Bottom - Padding - ButtonHeight,
                    ButtonWidth,
                    ButtonHeight);
            }
            
            ctx.ShowSecondaryButton = false;
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw accent line at top
            using var pen = new Pen(ctx.AccentColor, AccentLineThickness);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            
            int lineY = ctx.DrawingRect.Top + Padding;
            g.DrawLine(pen,
                ctx.DrawingRect.Left + Padding,
                lineY,
                ctx.DrawingRect.Left + Padding + AccentLineWidth,
                lineY);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // No additional hit areas for basic card
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
