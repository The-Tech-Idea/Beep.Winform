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
    /// ServiceCard - Service/feature display with centered icon, title, and CTA button.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class ServiceCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Service card fonts
        private Font _titleFont;
        private Font _descFont;
        private Font _badgeFont;
        
        // Service card spacing
        private const int Padding = 20;
        private const int IconSize = 60;
        private const int IconCirclePadding = 12;
        private const int TitleHeight = 28;
        private const int BadgeWidth = 100;
        private const int BadgeHeight = 22;
        private const int DescMinHeight = 50;
        private const int ButtonHeight = 38;
        private const int ButtonMaxWidth = 160;
        private const int AccentLineWidth = 50;
        private const int AccentLineThickness = 3;
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
            try { _badgeFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 14f, FontStyle.Bold);
            _descFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Regular);
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
            
            // Service title (centered below icon)
            int titleTop = ctx.ShowImage ? ctx.ImageRect.Bottom + ElementGap * 2 : drawingRect.Top + Padding;
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                titleTop,
                drawingRect.Width - Padding * 2,
                TitleHeight);
            
            // Category badge (centered below title)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - BadgeWidth) / 2,
                    ctx.HeaderRect.Bottom + ElementGap,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Service description
            int descTop = ctx.HeaderRect.Bottom + 
                (string.IsNullOrEmpty(ctx.BadgeText1) ? ElementGap * 2 : BadgeHeight + ElementGap * 2);
            int descHeight = Math.Max(DescMinHeight,
                drawingRect.Height - (descTop - drawingRect.Top) - Padding * 2 - 
                (ctx.ShowButton ? ButtonHeight + ElementGap + AccentLineThickness + ElementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                descTop,
                drawingRect.Width - Padding * 2,
                descHeight);
            
            // CTA button (centered at bottom)
            if (ctx.ShowButton)
            {
                int buttonWidth = Math.Min(drawingRect.Width - Padding * 2, ButtonMaxWidth);
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - buttonWidth) / 2,
                    drawingRect.Bottom - Padding - ButtonHeight,
                    buttonWidth,
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
            // Draw icon background circle with accent color
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                int circleSize = IconSize + IconCirclePadding * 2;
                var circleRect = new Rectangle(
                    ctx.ImageRect.Left - IconCirclePadding,
                    ctx.ImageRect.Top - IconCirclePadding,
                    circleSize,
                    circleSize);
                
                // Fill circle
                using var fillBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
                g.FillEllipse(fillBrush, circleRect);
                
                // Border circle
                using var borderPen = new Pen(Color.FromArgb(40, ctx.AccentColor), 2);
                g.DrawEllipse(borderPen, circleRect);
            }
            
            // Draw category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw decorative accent line above button
            if (ctx.ShowButton && !ctx.ButtonRect.IsEmpty)
            {
                int lineY = ctx.ButtonRect.Top - ElementGap;
                int lineX = ctx.DrawingRect.Left + (ctx.DrawingRect.Width - AccentLineWidth) / 2;
                
                using var pen = new Pen(ctx.AccentColor, AccentLineThickness);
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                
                g.DrawLine(pen, lineX, lineY, lineX + AccentLineWidth, lineY);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Category", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Category", ctx.BadgeRect));
            }
            
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Icon", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Icon", ctx.ImageRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _descFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

