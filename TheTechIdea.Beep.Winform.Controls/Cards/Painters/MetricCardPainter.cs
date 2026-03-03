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
    /// MetricCard - Data visualization card with large metric value, trend indicator, and status bar.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class MetricCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Metric card fonts
        private Font _labelFont;
        private Font _valueFont;
        private Font _trendFont;
        private Font _secondaryFont;
        private Font _badgeFont;
        
        // Metric card spacing
        private const int Padding = 16;
        private const int IconSize = 40;
        private const int LabelHeight = 20;
        private const int ValueHeight = 40;
        private const int TrendHeight = 20;
        private const int TrendPillWidth = 100;
        private const int BadgeWidth = 70;
        private const int BadgeHeight = 20;
        private const int StatusBarHeight = 6;
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
            _secondaryFont = bodyFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int iconSize = DpiScalingHelper.ScaleValue(IconSize, _owner);
            int labelHeight = DpiScalingHelper.ScaleValue(LabelHeight, _owner);
            int valueHeight = DpiScalingHelper.ScaleValue(ValueHeight, _owner);
            int trendHeight = DpiScalingHelper.ScaleValue(TrendHeight, _owner);
            int trendPillWidth = DpiScalingHelper.ScaleValue(TrendPillWidth, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int statusBarHeight = DpiScalingHelper.ScaleValue(StatusBarHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Icon or chart thumbnail (optional)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + padding,
                    drawingRect.Top + padding,
                    iconSize,
                    iconSize);
            }
            
            // Metric label/name
            int labelLeft = drawingRect.Left + padding + (ctx.ShowImage ? iconSize + elementGap : 0);
            int labelWidth = drawingRect.Width - padding * 2 - (ctx.ShowImage ? iconSize + elementGap : 0);
            
            ctx.HeaderRect = new Rectangle(
                labelLeft,
                drawingRect.Top + padding,
                labelWidth,
                labelHeight);
            
            // Category badge (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - padding - badgeWidth,
                    drawingRect.Top + padding,
                    badgeWidth,
                    badgeHeight);
            }
            
            // Large metric value
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + padding,
                ctx.HeaderRect.Bottom + elementGap,
                drawingRect.Width - padding * 2,
                valueHeight);
            
            // Trend indicator (left side)
            ctx.RatingRect = new Rectangle(
                drawingRect.Left + padding,
                ctx.SubtitleRect.Bottom + elementGap,
                trendPillWidth,
                trendHeight);
            
            // Secondary metric/comparison (right of trend)
            ctx.ParagraphRect = new Rectangle(
                ctx.RatingRect.Right + elementGap * 2,
                ctx.SubtitleRect.Bottom + elementGap,
                Math.Max(DpiScalingHelper.ScaleValue(60, _owner), drawingRect.Right - padding - ctx.RatingRect.Right - elementGap * 2),
                trendHeight);
            
            // Status/progress bar at bottom
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(
                    drawingRect.Left,
                    drawingRect.Bottom - statusBarHeight,
                    drawingRect.Width,
                    statusBarHeight);
            }
            
            // No buttons for metric cards
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
            // Draw large metric value
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                using var brush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _valueFont, brush, ctx.SubtitleRect, format);
            }
            
            // Draw trend indicator with color-coded pill background
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.RatingRect.IsEmpty)
            {
                // Determine trend color
                Color trendColor = Color.FromArgb(158, 158, 158); // Gray default
                
                if (ctx.StatusText.StartsWith("+") || ctx.StatusText.Contains("↑"))
                {
                    trendColor = Color.FromArgb(76, 175, 80); // Green
                }
                else if (ctx.StatusText.StartsWith("-") || ctx.StatusText.Contains("↓"))
                {
                    trendColor = Color.FromArgb(244, 67, 54); // Red
                }
                
                // Draw pill background
                var pillRect = new Rectangle(ctx.RatingRect.X, ctx.RatingRect.Y, 
                    Math.Min(DpiScalingHelper.ScaleValue(TrendPillWidth, _owner), ctx.RatingRect.Width), ctx.RatingRect.Height);
                
                using var pillPath = CardRenderingHelpers.CreateRoundedPath(pillRect, 10);
                using var pillBrush = new SolidBrush(Color.FromArgb(25, trendColor));
                g.FillPath(pillBrush, pillPath);
                
                // Draw trend text
                using var textBrush = new SolidBrush(trendColor);
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Center, 
                    LineAlignment = StringAlignment.Center 
                };
                g.DrawString(ctx.StatusText, _trendFont, textBrush, pillRect, format);
            }
            
            // Draw category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw status/progress bar
            if (ctx.ShowStatus && !ctx.StatusRect.IsEmpty)
            {
                using var brush = new SolidBrush(ctx.StatusColor);
                g.FillRectangle(brush, ctx.StatusRect);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Category", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Category", ctx.BadgeRect));
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
