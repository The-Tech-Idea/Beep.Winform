using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _descFont = bodyFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int iconSize = DpiScalingHelper.ScaleValue(IconSize, _owner);
            int iconCirclePadding = DpiScalingHelper.ScaleValue(IconCirclePadding, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int descHeightMin = DpiScalingHelper.ScaleValue(DescHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Center the icon horizontally at top
            int iconAreaWidth = drawingRect.Width - padding * 2;
            int iconX = drawingRect.Left + (drawingRect.Width - iconSize) / 2;
            
            ctx.ImageRect = new Rectangle(
                iconX,
                drawingRect.Top + padding + iconCirclePadding,
                iconSize,
                iconSize);
            
            // Title centered below icon
            int contentTop = ctx.ImageRect.Bottom + iconCirclePadding + elementGap;
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + padding,
                contentTop,
                drawingRect.Width - padding * 2,
                titleHeight);
            
            // Description below title
            int descHeight = Math.Max(descHeightMin, drawingRect.Bottom - ctx.HeaderRect.Bottom - padding - elementGap);
            ctx.ParagraphRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + elementGap,
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
                int iconCirclePadding = DpiScalingHelper.ScaleValue(IconCirclePadding, _owner);
                int iconSize = DpiScalingHelper.ScaleValue(IconSize, _owner);
                int circleSize = iconSize + iconCirclePadding * 2;
                var circleRect = new Rectangle(
                    ctx.ImageRect.X - iconCirclePadding,
                    ctx.ImageRect.Y - iconCirclePadding,
                    circleSize,
                    circleSize);
                
                using var brush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(brush, circleRect);
            }
            
            // Draw badge if present
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _descFont);
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
_disposed = true;
        }
        
        #endregion
    }
}
