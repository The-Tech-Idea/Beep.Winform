using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// IconCard - Centered icon with text below (landing page style).
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class IconCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Icon card fonts
        private Font _titleFont;
        private Font _descFont;
        
        // Icon card spacing
        private const int Padding = 24;
        private const int IconSize = 72;
        private const int IconBackgroundPadding = 16;
        private const int TitleHeight = 28;
        private const int DescMinHeight = 48;
        private const int ElementGap = 12;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 14f, FontStyle.Bold);
            _descFont = new Font(fontFamily, 10f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Large centered icon at top
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - IconSize) / 2,
                    drawingRect.Top + Padding,
                    IconSize,
                    IconSize);
            }
            
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + ElementGap * 2 : drawingRect.Top + Padding;
            
            // Title (centered)
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                contentTop,
                drawingRect.Width - Padding * 2,
                TitleHeight);
            
            // Description (centered)
            int descHeight = Math.Max(DescMinHeight,
                drawingRect.Height - (ctx.HeaderRect.Bottom - drawingRect.Top) - Padding - ElementGap);
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.HeaderRect.Bottom + ElementGap,
                drawingRect.Width - Padding * 2,
                descHeight);
            
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw icon background circle
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                DrawIconBackground(g, ctx);
            }
        }
        
        private void DrawIconBackground(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Large circular background
            var bgRect = new Rectangle(
                ctx.ImageRect.X - IconBackgroundPadding,
                ctx.ImageRect.Y - IconBackgroundPadding,
                ctx.ImageRect.Width + IconBackgroundPadding * 2,
                ctx.ImageRect.Height + IconBackgroundPadding * 2);
            
            // Gradient fill
            using var gradientBrush = new LinearGradientBrush(
                new Point(bgRect.Left, bgRect.Top),
                new Point(bgRect.Right, bgRect.Bottom),
                Color.FromArgb(25, ctx.AccentColor),
                Color.FromArgb(10, ctx.AccentColor));
            
            g.FillEllipse(gradientBrush, bgRect);
            
            // Subtle border
            using var borderPen = new Pen(Color.FromArgb(40, ctx.AccentColor), 2);
            g.DrawEllipse(borderPen, bgRect);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Icon hit area
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                var bgRect = new Rectangle(
                    ctx.ImageRect.X - IconBackgroundPadding,
                    ctx.ImageRect.Y - IconBackgroundPadding,
                    ctx.ImageRect.Width + IconBackgroundPadding * 2,
                    ctx.ImageRect.Height + IconBackgroundPadding * 2);
                
                owner.AddHitArea("Icon", bgRect, null,
                    () => notifyAreaHit?.Invoke("Icon", bgRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _descFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

