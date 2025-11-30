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
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _labelFont?.Dispose(); } catch { }
            try { _valueFont?.Dispose(); } catch { }
            try { _trendFont?.Dispose(); } catch { }
            try { _secondaryFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _labelFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _valueFont = new Font(fontFamily, 26f, FontStyle.Bold);
            _trendFont = new Font(fontFamily, 9f, FontStyle.Bold);
            _secondaryFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Icon or chart thumbnail (optional)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Top + Padding,
                    IconSize,
                    IconSize);
            }
            
            // Metric label/name
            int labelLeft = drawingRect.Left + Padding + (ctx.ShowImage ? IconSize + ElementGap : 0);
            int labelWidth = drawingRect.Width - Padding * 2 - (ctx.ShowImage ? IconSize + ElementGap : 0);
            
            ctx.HeaderRect = new Rectangle(
                labelLeft,
                drawingRect.Top + Padding,
                labelWidth,
                LabelHeight);
            
            // Category badge (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - BadgeWidth,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Large metric value
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.HeaderRect.Bottom + ElementGap,
                drawingRect.Width - Padding * 2,
                ValueHeight);
            
            // Trend indicator (left side)
            ctx.RatingRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.SubtitleRect.Bottom + ElementGap,
                TrendPillWidth,
                TrendHeight);
            
            // Secondary metric/comparison (right of trend)
            ctx.ParagraphRect = new Rectangle(
                ctx.RatingRect.Right + ElementGap * 2,
                ctx.SubtitleRect.Bottom + ElementGap,
                Math.Max(60, drawingRect.Right - Padding - ctx.RatingRect.Right - ElementGap * 2),
                TrendHeight);
            
            // Status/progress bar at bottom
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(
                    drawingRect.Left,
                    drawingRect.Bottom - StatusBarHeight,
                    drawingRect.Width,
                    StatusBarHeight);
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
                    Math.Min(TrendPillWidth, ctx.RatingRect.Width), ctx.RatingRect.Height);
                
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
            
            _labelFont?.Dispose();
            _valueFont?.Dispose();
            _trendFont?.Dispose();
            _secondaryFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
