using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _valueFont?.Dispose(); } catch { }
            try { _legendFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 11f, FontStyle.Bold);
            _valueFont = new Font(fontFamily, 22f, FontStyle.Bold);
            _legendFont = new Font(fontFamily, 8f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
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
                using var brush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _valueFont, brush, ctx.SubtitleRect, format);
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
            
            Color trendColor = isPositive ? Color.FromArgb(76, 175, 80) :
                               isNegative ? Color.FromArgb(244, 67, 54) :
                               Color.FromArgb(158, 158, 158);
            
            // Draw pill background
            using var pillPath = CardRenderingHelpers.CreateRoundedPath(ctx.StatusRect, 12);
            using var pillBrush = new SolidBrush(Color.FromArgb(20, trendColor));
            g.FillPath(pillBrush, pillPath);
            
            // Draw text
            using var textBrush = new SolidBrush(trendColor);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.StatusText, _badgeFont, textBrush, ctx.StatusRect, format);
        }
        
        private void DrawChartPlaceholder(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw chart background
            using var bgBrush = new SolidBrush(Color.FromArgb(5, ctx.AccentColor));
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, 8);
            g.FillPath(bgBrush, bgPath);
            
            // Draw sample line chart
            int barCount = 12;
            int barWidth = (ctx.ImageRect.Width - 20) / barCount;
            int maxHeight = ctx.ImageRect.Height - 30;
            
            Random rnd = new Random(42); // Fixed seed for consistent rendering
            Point[] points = new Point[barCount];
            
            for (int i = 0; i < barCount; i++)
            {
                int height = rnd.Next(maxHeight / 3, maxHeight);
                points[i] = new Point(
                    ctx.ImageRect.Left + 10 + i * barWidth + barWidth / 2,
                    ctx.ImageRect.Bottom - 15 - height);
            }
            
            // Draw area under line
            Point[] areaPoints = new Point[barCount + 2];
            Array.Copy(points, 0, areaPoints, 0, barCount);
            areaPoints[barCount] = new Point(points[barCount - 1].X, ctx.ImageRect.Bottom - 15);
            areaPoints[barCount + 1] = new Point(points[0].X, ctx.ImageRect.Bottom - 15);
            
            using var areaBrush = new SolidBrush(Color.FromArgb(30, ctx.AccentColor));
            g.FillPolygon(areaBrush, areaPoints);
            
            // Draw line
            using var linePen = new Pen(ctx.AccentColor, 2);
            linePen.LineJoin = LineJoin.Round;
            g.DrawLines(linePen, points);
            
            // Draw points
            using var pointBrush = new SolidBrush(ctx.AccentColor);
            foreach (var point in points)
            {
                g.FillEllipse(pointBrush, point.X - 3, point.Y - 3, 6, 6);
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
                var dotRect = new Rectangle(x, ctx.ParagraphRect.Top + 6, 10, 10);
                using var dotBrush = new SolidBrush(legendColors[colorIndex % legendColors.Length]);
                g.FillEllipse(dotBrush, dotRect);
                
                // Label
                using var textBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var textRect = new Rectangle(dotRect.Right + 4, ctx.ParagraphRect.Top, 60, ctx.ParagraphRect.Height);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(item, _legendFont, textBrush, textRect, format);
                
                x += 80;
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
            
            _titleFont?.Dispose();
            _valueFont?.Dispose();
            _legendFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

