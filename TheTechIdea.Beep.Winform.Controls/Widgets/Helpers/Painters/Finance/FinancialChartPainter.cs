using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Finance
{
    /// <summary>
    /// FinancialChart - Specialized financial charts painter with enhanced visual presentation
    /// </summary>
    internal sealed class FinancialChartPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public FinancialChartPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Title area
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            }

            // Chart content area
            int contentTop = ctx.HeaderRect.IsEmpty ? ctx.DrawingRect.Top + pad : ctx.HeaderRect.Bottom + 8;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                contentTop,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - contentTop - pad
            );

            // Legend area at bottom
            if (ctx.ShowLegend)
            {
                ctx.LegendRect = new Rectangle(ctx.ContentRect.Left, ctx.ContentRect.Bottom - 20,
                                             ctx.ContentRect.Width, 20);
                ctx.ChartRect = new Rectangle(ctx.ContentRect.Left, ctx.ContentRect.Top,
                                            ctx.ContentRect.Width, ctx.ContentRect.Height - 24);
            }
            else
            {
                ctx.ChartRect = ctx.ContentRect;
            }

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Draw main background
            using var bgBrush = new SolidBrush(Theme?.CardBackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);

            // Draw chart area background
            using var chartBgBrush = new SolidBrush(Color.FromArgb(250, 250, 250));
            using var chartPath = CreateRoundedPath(ctx.ChartRect, 4);
            g.FillPath(chartBgBrush, chartPath);

            // Draw grid lines
            DrawGrid(g, ctx);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (!string.IsNullOrEmpty(ctx.Title) && !ctx.HeaderRect.IsEmpty)
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Theme?.CardTextForeColor ?? Color.Black);
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);
            }

            // Draw financial data
            DrawFinancialData(g, ctx);

            // Draw legend if enabled
            if (ctx.ShowLegend)
            {
                DrawLegend(g, ctx);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw trend indicators
            DrawTrendIndicators(g, ctx);
        }

        private void DrawGrid(Graphics g, WidgetContext ctx)
        {
            using var gridPen = new Pen(Color.FromArgb(230, 230, 230), 1);

            // Horizontal grid lines
            for (int i = 0; i <= 4; i++)
            {
                int y = ctx.ChartRect.Top + (i * ctx.ChartRect.Height / 4);
                g.DrawLine(gridPen, ctx.ChartRect.Left, y, ctx.ChartRect.Right, y);
            }

            // Vertical grid lines
            for (int i = 0; i <= 6; i++)
            {
                int x = ctx.ChartRect.Left + (i * ctx.ChartRect.Width / 6);
                g.DrawLine(gridPen, x, ctx.ChartRect.Top, x, ctx.ChartRect.Bottom);
            }
        }

        private void DrawFinancialData(Graphics g, WidgetContext ctx)
        {
            var financeItems = ctx.FinanceItems?.Cast<FinanceItem>().ToList();

            if (financeItems == null || financeItems.Count == 0)
            {
                // Draw sample data if no real data
                DrawSampleChart(g, ctx);
                return;
            }

            // Draw actual financial data
            DrawActualChart(g, ctx, financeItems);
        }

        private void DrawSampleChart(Graphics g, WidgetContext ctx)
        {
            // Draw a sample line chart with financial-looking data
            var points = new List<PointF>();
            var random = new Random(42); // Fixed seed for consistent sample

            for (int i = 0; i < 7; i++)
            {
                float x = ctx.ChartRect.Left + (i * ctx.ChartRect.Width / 6f);
                float y = ctx.ChartRect.Bottom - (random.Next(20, 80) * ctx.ChartRect.Height / 100f);
                points.Add(new PointF(x, y));
            }

            // Draw line
            using var linePen = new Pen(ctx.AccentColor, 2);
            if (points.Count > 1)
            {
                g.DrawLines(linePen, points.ToArray());
            }

            // Draw data points
            foreach (var point in points)
            {
                using var pointBrush = new SolidBrush(ctx.AccentColor);
                g.FillEllipse(pointBrush, point.X - 3, point.Y - 3, 6, 6);
            }
        }

        private void DrawActualChart(Graphics g, WidgetContext ctx, List<FinanceItem> financeItems)
        {
            if (financeItems.Count < 2) return;

            var points = new List<PointF>();
            decimal maxValue = financeItems.Max(item => Math.Abs(item.Value));
            decimal minValue = financeItems.Min(item => item.Value);
            decimal range = maxValue - minValue;
            if (range == 0) range = 1; // avoid division by zero

            for (int i = 0; i < financeItems.Count; i++)
            {
                var item = financeItems[i];
                float x = ctx.ChartRect.Left + (i * ctx.ChartRect.Width / (float)(financeItems.Count - 1));
                float normalizedValue = (float)((item.Value - minValue) / range);
                float y = ctx.ChartRect.Bottom - (normalizedValue * ctx.ChartRect.Height * 0.8f) - (ctx.ChartRect.Height * 0.1f);

                points.Add(new PointF(x, y));
            }

            // Draw line with gradient
            if (points.Count > 1)
            {
                using var linePen = new Pen(ctx.AccentColor, 2);
                g.DrawLines(linePen, points.ToArray());
            }

            // Draw data points with colors based on value
            foreach (var point in points)
            {
                Color pointColor = ctx.AccentColor;
                using var pointBrush = new SolidBrush(pointColor);
                g.FillEllipse(pointBrush, point.X - 3, point.Y - 3, 6, 6);
            }
        }

        private void DrawLegend(Graphics g, WidgetContext ctx)
        {
            if (ctx.LegendRect.IsEmpty) return;

            using var legendFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var legendBrush = new SolidBrush(Color.FromArgb(150, Color.Black));

            string legendText = "Financial Data";
            g.DrawString(legendText, legendFont, legendBrush, ctx.LegendRect,
                       new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

        private void DrawTrendIndicators(Graphics g, WidgetContext ctx)
        {
            var trend = ctx.Trend;
            if (string.IsNullOrEmpty(trend)) return;

            // Draw trend arrow in top-right corner
            var arrowRect = new Rectangle(ctx.DrawingRect.Right - 24, ctx.DrawingRect.Top + 8, 16, 16);
            DrawTrendArrow(g, arrowRect, trend, ctx.AccentColor);
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}