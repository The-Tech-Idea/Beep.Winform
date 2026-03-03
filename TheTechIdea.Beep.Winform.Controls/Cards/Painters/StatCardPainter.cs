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
    /// StatCard - Dashboard KPI/metric display with large value, trend indicator.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class StatCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Stat card fonts
        private Font _labelFont;
        private Font _valueFont;
        private Font _trendFont;
        private Font _subtitleFont;
        
        // Stat card spacing
        private const int Padding = 20;
        private const int IconSize = 36;
        private const int LabelHeight = 20;
        private const int ValueHeight = 44;
        private const int TrendHeight = 18;
        private const int StatusBarHeight = 4;
        private const int ElementGap = 8;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_labelFont = captionFont;
            _valueFont = bodyFont;
            _trendFont = bodyFont;
            _subtitleFont = titleFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int iconSize = DpiScalingHelper.ScaleValue(IconSize, _owner);
            int labelHeight = DpiScalingHelper.ScaleValue(LabelHeight, _owner);
            int valueHeight = DpiScalingHelper.ScaleValue(ValueHeight, _owner);
            int trendHeight = DpiScalingHelper.ScaleValue(TrendHeight, _owner);
            int statusBarHeight = DpiScalingHelper.ScaleValue(StatusBarHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            int contentTop = drawingRect.Top + padding;
            int contentWidth = drawingRect.Width - padding * 2;
            
            // Icon at top-left (optional)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + padding,
                    contentTop,
                    iconSize,
                    iconSize);
                contentTop = ctx.ImageRect.Bottom + elementGap;
            }
            
            // Label (e.g., "Active Users")
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + padding,
                contentTop,
                contentWidth,
                labelHeight);
            
            // Large value (e.g., "12,458")
            ctx.HeaderRect = new Rectangle(
                ctx.ParagraphRect.Left,
                ctx.ParagraphRect.Bottom + elementGap / 2,
                contentWidth,
                valueHeight);
            
            // Trend indicator (e.g., "+18.2% from last month")
            ctx.SubtitleRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + elementGap,
                contentWidth,
                trendHeight);
            
            // Status bar at bottom (optional)
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(
                    drawingRect.Left,
                    drawingRect.Bottom - statusBarHeight,
                    drawingRect.Width,
                    statusBarHeight);
            }
            
            // No buttons for stat cards
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
            // Draw trend indicator with color coding
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.SubtitleRect.IsEmpty)
            {
                Color trendColor = ctx.StatusText.StartsWith("+") ? Color.FromArgb(76, 175, 80) :
                                   ctx.StatusText.StartsWith("-") ? Color.FromArgb(244, 67, 54) :
                                   Color.FromArgb(158, 158, 158);
                
                using var brush = new SolidBrush(trendColor);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _trendFont, brush, ctx.SubtitleRect, format);
            }
            
            // Draw status accent bar at bottom
            if (ctx.ShowStatus && !ctx.StatusRect.IsEmpty)
            {
                using var brush = new SolidBrush(ctx.StatusColor);
                g.FillRectangle(brush, ctx.StatusRect);
            }
            
            // Draw badge if present
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _labelFont);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Stat cards typically don't have clickable areas
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
