using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Chart
{
    /// <summary>
    /// HeatmapChart - Calendar/grid heatmap with hover and hit areas
    /// </summary>
    internal sealed class HeatmapPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _cellRects = new();
        private Rectangle _chartRectCache;
        private int _cols = 7;
        private int _rows = 1;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 20);

            ctx.ChartRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3
            );

            _chartRectCache = ctx.ChartRect;
            // Defer cell rect calculation to Draw/Update where values are available
            _cellRects.Clear();
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }

            // Draw heatmap grid
            if (ctx.Values?.Any() == true)
            {
                BuildCellRects(ctx);
                var maxValue = ctx.Values.Max();

                for (int i = 0; i < Math.Min(ctx.Values.Count, _cellRects.Count); i++)
                {
                    var cellRect = _cellRects[i];
                    double intensity = maxValue > 0 ? ctx.Values[i] / maxValue : 0;
                    WidgetRenderingHelpers.DrawHeatmapCell(g, cellRect, intensity, ctx.AccentColor, 2);

                    // Hover outline
                    if (IsAreaHovered($"Heatmap_Cell_{i}"))
                    {
                        using var hover = new Pen(Theme?.PrimaryColor ?? Color.Blue, 1.5f);
                        g.DrawRoundedRectangle(hover, Rectangle.Inflate(cellRect, 1, 1), 2);
                    }
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional grid outlines when chart hovered
            if (IsAreaHovered("Heatmap_Chart") && _cellRects.Count > 0)
            {
                using var gridPen = new Pen(Color.FromArgb(20, Theme?.BorderColor ?? Color.Gray));
                foreach (var r in _cellRects)
                {
                    g.DrawRectangle(gridPen, r);
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            if (!_chartRectCache.IsEmpty)
            {
                owner.AddHitArea("Heatmap_Chart", _chartRectCache, null, () =>
                {
                    ctx.CustomData["HeatmapChartClicked"] = true;
                    notifyAreaHit?.Invoke("Heatmap_Chart", _chartRectCache);
                    Owner?.Invalidate();
                });
            }

            if (ctx.Values?.Any() == true)
            {
                BuildCellRects(ctx);
                for (int i = 0; i < _cellRects.Count; i++)
                {
                    int idx = i;
                    var rect = _cellRects[i];
                    owner.AddHitArea($"Heatmap_Cell_{idx}", rect, null, () =>
                    {
                        ctx.CustomData["SelectedCellIndex"] = idx;
                        notifyAreaHit?.Invoke($"Heatmap_Cell_{idx}", rect);
                        Owner?.Invalidate();
                    });
                }
            }
        }

        private void BuildCellRects(WidgetContext ctx)
        {
            _cellRects.Clear();
            _cols = 7; // Week view default
            _rows = (int)Math.Ceiling((ctx.Values?.Count ?? 0) / (double)_cols);
            if (_rows <= 0) _rows = 1;

            int cellWidth = Math.Max(4, ctx.ChartRect.Width / _cols);
            int cellHeight = Math.Max(4, ctx.ChartRect.Height / _rows);
            int max = Math.Min(ctx.Values?.Count ?? 0, _cols * _rows);

            for (int i = 0; i < max; i++)
            {
                int col = i % _cols;
                int row = i / _cols;
                _cellRects.Add(new Rectangle(
                    ctx.ChartRect.X + col * cellWidth + 1,
                    ctx.ChartRect.Y + row * cellHeight + 1,
                    cellWidth - 2,
                    cellHeight - 2
                ));
            }
        }
    }
}
