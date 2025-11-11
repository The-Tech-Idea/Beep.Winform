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
    /// ChartGrid - Multiple small charts with enhanced visual presentation
    /// </summary>
    internal sealed class ChartGridPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;
        private readonly List<Rectangle> _cellRects = new();
        private Rectangle _expandRect;

        public ChartGridPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 12, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);

            // Precompute grid cell rectangles for hit testing
            _cellRects.Clear();
            if (ctx.Metrics != null)
            {
                var metrics = ctx.Metrics.Cast<Dictionary<string, object>>().ToList();
                _cellRects.AddRange(CalculateGridCellRects(ctx.ContentRect, metrics?.Count ?? 0, ctx.Columns, ctx.Rows));
            }

            // Expand button location (if expandable)
            _expandRect = new Rectangle(ctx.DrawingRect.Right - 28, ctx.DrawingRect.Y + 8, 20, 20);
            
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

            // Draw enhanced title with grid icon
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                DrawGridTitle(g, ctx);
            }
            
            // Draw enhanced chart grid
            if (ctx.Metrics != null)
            {
                var metrics = ctx.Metrics.Cast<Dictionary<string, object>>().ToList();
                int columns = ctx.Columns;
                int rows = ctx.Rows;
                
                DrawChartsGrid(g, ctx.ContentRect, metrics, columns, rows, ctx.AccentColor);
            }
        }

        private void DrawGridTitle(Graphics g, WidgetContext ctx)
        {
            // Title background with subtle gradient
            using var titleBrush = new LinearGradientBrush(
                ctx.HeaderRect,
                Color.FromArgb(15, Theme?.PrimaryColor ?? Color.Blue),
                Color.FromArgb(5, Theme?.PrimaryColor ?? Color.Blue),
                LinearGradientMode.Horizontal);
            g.FillRoundedRectangle(titleBrush, ctx.HeaderRect, 4);

            // Grid icon
            var iconRect = new Rectangle(ctx.HeaderRect.X + 8, ctx.HeaderRect.Y + 4, 16, 16);
            _imagePainter.DrawSvg(g, "grid", iconRect, 
                Theme?.PrimaryColor ?? Color.Blue, 0.8f);

            // Title text
            var titleTextRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                ctx.HeaderRect.Width - iconRect.Width - 16, ctx.HeaderRect.Height);
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var titleTextBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.Title, titleFont, titleTextBrush, titleTextRect, format);
        }

        private void DrawChartsGrid(Graphics g, Rectangle rect, List<Dictionary<string, object>> metrics, int columns, int rows, Color accentColor)
        {
            if (!metrics.Any()) return;
            
            int cellWidth = rect.Width / columns;
            int cellHeight = rect.Height / rows;
            int cellPad = 8;
            
            // Sample data for demonstration
            var sampleData = new List<double> { 10, 15, 12, 18, 14, 20, 16 };
            string[] chartIcons = { "bar-chart", "pie-chart", "trending-up", "activity" };
            
            for (int i = 0; i < Math.Min(metrics.Count, columns * rows); i++)
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
                
                // Ensure cache aligns with drawn rects
                if (i < _cellRects.Count) _cellRects[i] = cellRect;
                
                // Enhanced cell background with gradient
                Color cellColor = metric.ContainsKey("Color") ? (Color)metric["Color"] : accentColor;
                using var cellBrush = new LinearGradientBrush(
                    cellRect,
                    Color.FromArgb(15, cellColor),
                    Color.FromArgb(5, cellColor),
                    LinearGradientMode.Vertical);
                using var cellPath = CreateRoundedPath(cellRect, 8);
                g.FillPath(cellBrush, cellPath);
                
                // Cell border
                using var borderPen = new Pen(Color.FromArgb(20, Theme?.BorderColor ?? Color.Gray), 1);
                g.DrawPath(borderPen, cellPath);
                
                // Hover effect
                if (IsAreaHovered($"ChartGrid_Cell_{i}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(10, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRoundedRectangle(hover, Rectangle.Inflate(cellRect, 2, 2), 8);
                }
                
                // Header area with icon and title
                var headerRect = new Rectangle(cellRect.X + 8, cellRect.Y + 8, cellRect.Width - 16, 20);
                var iconRect = new Rectangle(headerRect.X, headerRect.Y + 2, 16, 16);
                
                string iconName = metric.ContainsKey("Icon") ? metric["Icon"].ToString() : chartIcons[i % chartIcons.Length];
                _imagePainter.DrawSvg(g, iconName, iconRect, cellColor, 0.8f);
                
                // Chart title
                if (metric.ContainsKey("Title"))
                {
                    var titleTextRect = new Rectangle(iconRect.Right + 6, headerRect.Y, 
                        headerRect.Width - iconRect.Width - 6, headerRect.Height);
                    using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f,FontStyle.Bold);
                    using var titleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                    var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    g.DrawString(metric["Title"].ToString(), titleFont, titleBrush, titleTextRect, format);
                }
                
                // Value display area
                if (metric.ContainsKey("Value"))
                {
                    var valueRect = new Rectangle(cellRect.X + 8, headerRect.Bottom + 4, cellRect.Width - 16, 18);
                    using var valueFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
                    using var valueBrush = new SolidBrush(cellColor);
                    var valueFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    g.DrawString(metric["Value"].ToString(), valueFont, valueBrush, valueRect, valueFormat);
                }
                
                // Mini chart area (bottom portion)
                var chartRect = new Rectangle(cellRect.X + 8, cellRect.Y + cellRect.Height - 32, cellRect.Width - 16, 24);
                
                // Draw enhanced mini sparkline with fill
                WidgetRenderingHelpers.DrawSparkline(g, chartRect, sampleData, cellColor);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw expand indicator for full grid view
            if (ctx.IsExpandable)
            {
                using var expandBrush = new SolidBrush(Color.FromArgb(30, Theme?.AccentColor ?? Color.Gray));
                g.FillRoundedRectangle(expandBrush, _expandRect, 4);
                
                var iconRect = new Rectangle(_expandRect.X + 2, _expandRect.Y + 2, 16, 16);
                _imagePainter.DrawSvg(g, "maximize", iconRect, 
                    Color.FromArgb(150, Theme?.AccentColor ?? Color.Gray), 0.7f);
            }

            // Title underline on hover
            if (IsAreaHovered("ChartGrid_Title") && !string.IsNullOrEmpty(ctx.Title))
            {
                using var underlinePen = new Pen(Color.FromArgb(150, Theme?.PrimaryColor ?? Color.Blue), 2);
                g.DrawLine(underlinePen, ctx.HeaderRect.Left, ctx.HeaderRect.Bottom - 2, ctx.HeaderRect.Right, ctx.HeaderRect.Bottom - 2);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            // Header click area
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                owner.AddHitArea("ChartGrid_Title", ctx.HeaderRect, null, () =>
                {
                    ctx.TitleClicked = true;
                    notifyAreaHit?.Invoke("ChartGrid_Title", ctx.HeaderRect);
                    Owner?.Invalidate();
                });
            }

            // Cell click areas
            for (int i = 0; i < _cellRects.Count; i++)
            {
                int idx = i;
                var rect = _cellRects[i];
                if (rect.IsEmpty) continue;
                owner.AddHitArea($"ChartGrid_Cell_{idx}", rect, null, () =>
                {
                    ctx.SelectedCellIndex = idx;
                    notifyAreaHit?.Invoke($"ChartGrid_Cell_{idx}", rect);
                    Owner?.Invalidate();
                });
            }

            // Expand area
            if (ctx.IsExpandable)
            {
                owner.AddHitArea("ChartGrid_Expand", _expandRect, null, () =>
                {
                    ctx.ExpandRequested = true;
                    notifyAreaHit?.Invoke("ChartGrid_Expand", _expandRect);
                    Owner?.Invalidate();
                });
            }
        }

        private IEnumerable<Rectangle> CalculateGridCellRects(Rectangle rect, int metricsCount, int columns, int rows)
        {
            int cellWidth = rect.Width / Math.Max(columns, 1);
            int cellHeight = rect.Height / Math.Max(rows, 1);
            int cellPad = 8;
            int maxCells = Math.Min(metricsCount, columns * rows);

            for (int i = 0; i < maxCells; i++)
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