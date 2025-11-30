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
    /// FeatureCard - Centered icon with accent circle, title, and description.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class FeatureCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Feature card fonts
        private Font _titleFont;
        private Font _descFont;
        
        // Feature card spacing
        private const int Padding = 20;
        private const int IconSize = 64;
        private const int IconCirclePadding = 16;
        private const int TitleHeight = 28;
        private const int DescHeight = 60;
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
            
            // Center the icon horizontally at top
            int iconAreaWidth = drawingRect.Width - Padding * 2;
            int iconX = drawingRect.Left + (drawingRect.Width - IconSize) / 2;
            
            ctx.ImageRect = new Rectangle(
                iconX,
                drawingRect.Top + Padding + IconCirclePadding,
                IconSize,
                IconSize);
            
            // Title centered below icon
            int contentTop = ctx.ImageRect.Bottom + IconCirclePadding + ElementGap;
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                contentTop,
                drawingRect.Width - Padding * 2,
                TitleHeight);
            
            // Description below title
            int descHeight = Math.Max(40, drawingRect.Bottom - ctx.HeaderRect.Bottom - Padding - ElementGap);
            ctx.ParagraphRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + ElementGap,
                ctx.HeaderRect.Width,
                descHeight);
            
            // No buttons for feature cards
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
            // Draw accent circle behind icon
            if (!ctx.ImageRect.IsEmpty)
            {
                int circleSize = IconSize + IconCirclePadding * 2;
                var circleRect = new Rectangle(
                    ctx.ImageRect.X - IconCirclePadding,
                    ctx.ImageRect.Y - IconCirclePadding,
                    circleSize,
                    circleSize);
                
                using var brush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(brush, circleRect);
            }
            
            // Draw badge if present
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                using var font = new Font(_owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif, 8f, FontStyle.Bold);
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, font);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Feature cards typically don't have additional clickable areas
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
