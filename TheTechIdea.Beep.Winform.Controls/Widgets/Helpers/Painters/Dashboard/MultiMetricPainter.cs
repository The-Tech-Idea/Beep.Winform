using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Dashboard
{
    /// <summary>
    /// MultiMetric - Multiple KPIs in one widget with enhanced visual presentation and hit areas
    /// </summary>
    internal sealed class MultiMetricPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;
        private readonly List<Rectangle> _cellRects = new();
        private Rectangle _headerRectCache;

        public MultiMetricPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);
            
            // Title at top
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // Grid area for metrics
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3
            );

            _headerRectCache = ctx.HeaderRect;
            _cellRects.Clear();

            if (ctx.CustomData.TryGetValue("Metrics", out var raw) && raw is List<Dictionary<string, object>> metrics)
            {
                int columns = ctx.CustomData.ContainsKey("Columns") ? (int)ctx.CustomData["Columns"] : 2;
                int rows = ctx.CustomData.ContainsKey("Rows") ? (int)ctx.CustomData["Rows"] : 2;
                _cellRects.AddRange(CalculateGridCellRects(ctx.ContentRect, metrics.Count, columns, rows));
            }
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 16, layers: 5, offset: 3);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

            // Draw enhanced title with metrics icon
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                DrawMultiMetricTitle(g, ctx);
            }
            
            // Draw enhanced metrics grid
            if (ctx.CustomData.TryGetValue("Metrics", out var raw) && raw is List<Dictionary<string, object>> metrics)
            {
                int columns = ctx.CustomData.ContainsKey("Columns") ? (int)ctx.CustomData["Columns"] : 2;
                int rows = ctx.CustomData.ContainsKey("Rows") ? (int)ctx.CustomData["Rows"] : 2;
                
                DrawMetricsGrid(g, ctx.ContentRect, metrics, columns, rows, ctx.AccentColor);
            }
        }

        private void DrawMultiMetricTitle(Graphics g, WidgetContext ctx)
        {
            // Enhanced title background
            using var titleBrush = new LinearGradientBrush(
                ctx.HeaderRect,
                Color.FromArgb(20, Theme?.PrimaryColor ?? Color.Blue),
                Color.FromArgb(5, Theme?.PrimaryColor ?? Color.Blue),
                LinearGradientMode.Horizontal);
            g.FillRoundedRectangle(titleBrush, ctx.HeaderRect, 4);

            // KPI dashboard icon
            var iconRect = new Rectangle(ctx.HeaderRect.X + 8, ctx.HeaderRect.Y + 4, 16, 16);
            _imagePainter.DrawSvg(g, "activity", iconRect, 
                Theme?.PrimaryColor ?? Color.Blue, 0.8f);

            // Title text
            var titleTextRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                ctx.HeaderRect.Width - iconRect.Width - 16, ctx.HeaderRect.Height);
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var titleTextBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.Title, titleFont, titleTextBrush, titleTextRect, format);
        }

        private void DrawMetricsGrid(Graphics g, Rectangle rect, List<Dictionary<string, object>> metrics, int columns, int rows, Color accentColor)
        {
            if (!metrics.Any()) return;
            
            int cellWidth = rect.Width / Math.Max(columns, 1);
            int cellHeight = rect.Height / Math.Max(rows, 1);
            int cellPad = 6;
            
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Bold);
            using var valueFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 16f, FontStyle.Bold);
            using var trendFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 7f,FontStyle.Bold);
            
            // Default metric icons for KPIs
            string[] metricIcons = { "dollar-sign", "users", "shopping-cart", "trending-up", 
                                   "target", "zap", "award", "briefcase" };
            
            int max = Math.Min(metrics.Count, columns * rows);
            for (int i = 0; i < max; i++)
            {
                var metric = metrics[i];
                int col = i % columns;
                int row = i / columns;
                
                var cellRect = new Rectangle(
                    rect.X + col * cellWidth + cellPad,
                    rect.Y + row * cellHeight + cellPad,
                    cellWidth - cellPad * 2,
                    cellHeight - cellPad * 2
                );

                if (i < _cellRects.Count) _cellRects[i] = cellRect; else _cellRects.Add(cellRect);
                
                Color cellColor = metric.ContainsKey("Color") ? (Color)metric["Color"] : accentColor;
                
                // Enhanced cell background with gradient
                using var cellBrush = new LinearGradientBrush(
                    cellRect,
                    Color.FromArgb(20, cellColor),
                    Color.FromArgb(8, cellColor),
                    LinearGradientMode.Vertical);
                using var cellPath = CreateRoundedPath(cellRect, 10);
                g.FillPath(cellBrush, cellPath);
                
                // Subtle border
                using var borderPen = new Pen(Color.FromArgb(30, cellColor), 1);
                g.DrawPath(borderPen, cellPath);

                // Hover accent
                if (IsAreaHovered($"MultiMetric_Cell_{i}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillPath(hover, cellPath);
                }
                
                // Header section with icon
                var headerRect = new Rectangle(cellRect.X + 10, cellRect.Y + 10, cellRect.Width - 20, 20);
                var iconRect = new Rectangle(headerRect.X, headerRect.Y + 2, 16, 16);
                
                string iconName = metric.ContainsKey("Icon") ? metric["Icon"].ToString() : 
                                metricIcons[i % metricIcons.Length];
                _imagePainter.DrawSvg(g, iconName, iconRect, cellColor, 0.8f);
                
                // Metric title
                if (metric.ContainsKey("Title"))
                {
                    var titleTextRect = new Rectangle(iconRect.Right + 6, headerRect.Y, 
                        headerRect.Width - iconRect.Width - 6, headerRect.Height);
                    using var titleBrush = new SolidBrush(Color.FromArgb(130, Theme?.ForeColor ?? Color.Black));
                    var titleFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    g.DrawString(metric["Title"].ToString(), titleFont, titleBrush, titleTextRect, titleFormat);
                }
                
                // Value section (centered)
                var valueRect = new Rectangle(cellRect.X + 10, headerRect.Bottom + 8, cellRect.Width - 20, 28);
                if (metric.ContainsKey("Value"))
                {
                    using var valueBrush = new SolidBrush(cellColor);
                    var valueFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(metric["Value"].ToString(), valueFont, valueBrush, valueRect, valueFormat);
                }
                
                // Trend section with icon
                if (metric.ContainsKey("Trend"))
                {
                    var trendRect = new Rectangle(cellRect.X + 10, valueRect.Bottom + 4, cellRect.Width - 20, 16);
                    string trend = metric["Trend"].ToString();
                    bool isPositive = trend.StartsWith("+");
                    bool isNegative = trend.StartsWith("-");
                    
                    Color trendColor = isPositive ? Color.FromArgb(76, 175, 80) : 
                                     isNegative ? Color.FromArgb(244, 67, 54) : 
                                     Color.FromArgb(158, 158, 158);
                    
                    // Trend background
                    using var trendBgBrush = new SolidBrush(Color.FromArgb(15, trendColor));
                    var trendBgRect = new Rectangle(trendRect.X, trendRect.Y + 2, trendRect.Width, 12);
                    g.FillRoundedRectangle(trendBgBrush, trendBgRect, 3);
                    
                    // Trend icon
                    var trendIconRect = new Rectangle(trendRect.X + 4, trendRect.Y + 4, 8, 8);
                    string trendIconName = isPositive ? "arrow-up" : isNegative ? "arrow-down" : "minus";
                    _imagePainter.DrawSvg(g, trendIconName, trendIconRect, trendColor, 0.9f);
                    
                    // Trend text
                    var trendTextRect = new Rectangle(trendIconRect.Right + 2, trendRect.Y, 
                        trendRect.Width - trendIconRect.Width - 6, trendRect.Height);
                    using var trendBrush = new SolidBrush(trendColor);
                    var trendFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    g.DrawString(trend, trendFont, trendBrush, trendTextRect, trendFormat);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Title hover underline
            if (IsAreaHovered("MultiMetric_Title") && !string.IsNullOrEmpty(ctx.Title))
            {
                using var underlinePen = new Pen(Color.FromArgb(150, Theme?.PrimaryColor ?? Color.Blue), 2);
                g.DrawLine(underlinePen, ctx.HeaderRect.Left, ctx.HeaderRect.Bottom - 2, ctx.HeaderRect.Right, ctx.HeaderRect.Bottom - 2);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            if (!_headerRectCache.IsEmpty)
            {
                owner.AddHitArea("MultiMetric_Title", _headerRectCache, null, () =>
                {
                    ctx.CustomData["TitleClicked"] = true;
                    notifyAreaHit?.Invoke("MultiMetric_Title", _headerRectCache);
                    Owner?.Invalidate();
                });
            }

            for (int i = 0; i < _cellRects.Count; i++)
            {
                int idx = i;
                var rect = _cellRects[i];
                owner.AddHitArea($"MultiMetric_Cell_{idx}", rect, null, () =>
                {
                    ctx.CustomData["SelectedMetricIndex"] = idx;
                    notifyAreaHit?.Invoke($"MultiMetric_Cell_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        private IEnumerable<Rectangle> CalculateGridCellRects(Rectangle rect, int metricsCount, int columns, int rows)
        {
            int cellWidth = rect.Width / Math.Max(columns, 1);
            int cellHeight = rect.Height / Math.Max(rows, 1);
            int cellPad = 6;
            int max = Math.Min(metricsCount, Math.Max(1, columns * rows));
            for (int i = 0; i < max; i++)
            {
                int col = i % columns;
                int row = i / columns;
                yield return new Rectangle(
                    rect.X + col * cellWidth + cellPad,
                    rect.Y + row * cellHeight + cellPad,
                    cellWidth - cellPad * 2,
                    cellHeight - cellPad * 2
                );
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}