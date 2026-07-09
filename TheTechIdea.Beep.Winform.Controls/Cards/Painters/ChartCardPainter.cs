using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// ChartCard - Card with embedded chart/graph placeholder.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class ChartCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Chart card fonts
        private Font _titleFont;
        private Font _valueFont;
        private Font _legendFont;
        private Font _badgeFont;
        
        // Chart card spacing
        private const int Padding = 16;
        private const int TitleHeight = 26;
        private const int ValueHeight = 36;
        private const int ChartMinHeight = 100;
        private const int LegendHeight = 24;
        private const int BadgeWidth = 70;
        private const int BadgeHeight = 20;
        private const int ElementGap = 10;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _valueFont = bodyFont;
            _legendFont = bodyFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Title
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Top + Padding,
                drawingRect.Width - Padding * 2 - (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : BadgeWidth + ElementGap),
                TitleHeight);
            
            // Period/filter badge (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - BadgeWidth,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Main value
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                drawingRect.Width / 2,
                ValueHeight);
            
            // Trend indicator
            ctx.StatusRect = new Rectangle(
                ctx.SubtitleRect.Right + ElementGap,
                ctx.SubtitleRect.Top + 8,
                100,
                24);
            
            // Chart area (main visualization)
            int chartTop = ctx.SubtitleRect.Bottom + ElementGap;
            int chartHeight = Math.Max(ChartMinHeight,
                drawingRect.Height - (chartTop - drawingRect.Top) - Padding - LegendHeight - ElementGap);
            
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + Padding,
                chartTop,
                drawingRect.Width - Padding * 2,
                chartHeight);
            
            // Legend area
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.ImageRect.Bottom + ElementGap,
                drawingRect.Width - Padding * 2,
                LegendHeight);
            
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
            // Draw period badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw main value
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                TextRenderer.DrawText(g, ctx.SubtitleText, _valueFont, ctx.SubtitleRect, ctx.AccentColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
            }
            
            // Draw trend indicator
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.StatusRect.IsEmpty)
            {
                DrawTrendIndicator(g, ctx);
            }
            
            // Draw chart placeholder
            if (!ctx.ImageRect.IsEmpty)
            {
                DrawChartPlaceholder(g, ctx);
            }
            
            // Draw legend
            if (!ctx.ParagraphRect.IsEmpty)
            {
                DrawLegend(g, ctx);
            }
        }
        
        private void DrawTrendIndicator(Graphics g, LayoutContext ctx)
        {
            // Determine trend direction and color
            bool isPositive = ctx.StatusText.StartsWith("+") || ctx.StatusText.Contains("↑");
            bool isNegative = ctx.StatusText.StartsWith("-") || ctx.StatusText.Contains("↓");
            
            Color trendColor = isPositive ? (_theme?.SuccessColor ?? Color.FromArgb(76, 175, 80)) :
                               isNegative ? (_theme?.ErrorColor ?? Color.FromArgb(244, 67, 54)) :
                               Color.FromArgb(158, 158, 158);

            // Draw pill background
            using var pillPath = CardRenderingHelpers.CreateRoundedPath(ctx.StatusRect, DpiScalingHelper.ScaleValue(12, _owner));
            g.FillPath(CardPaintCache.Brush(Color.FromArgb(20, trendColor)), pillPath);

            // Draw text
            TextRenderer.DrawText(g, ctx.StatusText, _badgeFont, ctx.StatusRect, trendColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }
        
        private void DrawChartPlaceholder(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw chart background
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, DpiScalingHelper.ScaleValue(8, _owner));
            g.FillPath(CardPaintCache.Brush(Color.FromArgb(5, ctx.AccentColor)), bgPath);

            // Draw sample line chart
            int barCount = 12;
            int barWidth = (ctx.ImageRect.Width - DpiScalingHelper.ScaleValue(20, _owner)) / barCount;
            int maxHeight = ctx.ImageRect.Height - DpiScalingHelper.ScaleValue(30, _owner);
            
            Random rnd = new Random(42); // Fixed seed for consistent rendering
            Point[] points = new Point[barCount];
            
            for (int i = 0; i < barCount; i++)
            {
                int height = rnd.Next(maxHeight / 3, maxHeight);
                points[i] = new Point(
                    ctx.ImageRect.Left + DpiScalingHelper.ScaleValue(10, _owner) + i * barWidth + barWidth / 2,
                    ctx.ImageRect.Bottom - DpiScalingHelper.ScaleValue(15, _owner) - height);
            }

            // Draw area under line
            Point[] areaPoints = new Point[barCount + 2];
            Array.Copy(points, 0, areaPoints, 0, barCount);
            areaPoints[barCount] = new Point(points[barCount - 1].X, ctx.ImageRect.Bottom - DpiScalingHelper.ScaleValue(15, _owner));
            areaPoints[barCount + 1] = new Point(points[0].X, ctx.ImageRect.Bottom - DpiScalingHelper.ScaleValue(15, _owner));

            g.FillPolygon(CardPaintCache.Brush(Color.FromArgb(30, ctx.AccentColor)), areaPoints);

            // Draw line
            using var linePen = new Pen(ctx.AccentColor, DpiScalingHelper.ScaleValue(2, _owner));
            linePen.LineJoin = LineJoin.Round;
            g.DrawLines(linePen, points);
            
            // Draw points
            var pointBrush = CardPaintCache.Brush(ctx.AccentColor);
            foreach (var point in points)
            {
                g.FillEllipse(pointBrush, point.X - DpiScalingHelper.ScaleValue(3, _owner), point.Y - DpiScalingHelper.ScaleValue(3, _owner), DpiScalingHelper.ScaleValue(6, _owner), DpiScalingHelper.ScaleValue(6, _owner));
            }
        }
        
        private void DrawLegend(Graphics g, LayoutContext ctx)
        {
            if (ctx.Tags == null) return;
            
            int x = ctx.ParagraphRect.Left;
            Color[] legendColors = { ctx.AccentColor, Color.FromArgb(156, 39, 176), Color.FromArgb(255, 152, 0) };
            int colorIndex = 0;
            
            foreach (var item in ctx.Tags)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                
                // Color dot
                var dotRect = new Rectangle(x, ctx.ParagraphRect.Top + DpiScalingHelper.ScaleValue(6, _owner), DpiScalingHelper.ScaleValue(10, _owner), DpiScalingHelper.ScaleValue(10, _owner));
                g.FillEllipse(CardPaintCache.Brush(legendColors[colorIndex % legendColors.Length]), dotRect);

                // Label
                var textRect = new Rectangle(dotRect.Right + DpiScalingHelper.ScaleValue(4, _owner), ctx.ParagraphRect.Top, DpiScalingHelper.ScaleValue(60, _owner), ctx.ParagraphRect.Height);
                TextRenderer.DrawText(g, item, _legendFont, textRect, Color.FromArgb(120, _theme?.CardTextForeColor ?? Color.Black),
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);

                x += DpiScalingHelper.ScaleValue(80, _owner);
                colorIndex++;
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Chart area hit area
            if (!ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Chart", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Chart", ctx.ImageRect));
            }
            
            // Badge hit area
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Period", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Period", ctx.BadgeRect));
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

