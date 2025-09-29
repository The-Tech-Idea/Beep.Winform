using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Base; // Added for BaseControl
using BaseImage = TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Dashboard
{
    /// <summary>
    /// AnalyticsPanel - Complex analytics layout with enhanced image rendering
    /// Updated: Uses BaseControl.DrawingRect, adds BeepAppBar-style hit areas and hover states
    /// </summary>
    internal sealed class AnalyticsPanelPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        // Cached metric rects for hit testing
        private List<Rectangle> _metricRects = new();
        private Rectangle _chartHitRect;

        public AnalyticsPanelPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            // Base on Owner.DrawingRect as per migration plan
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);

            // Split content into sections
            int contentHeight = ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3;
            int topSectionHeight = Math.Max(40, contentHeight * 2 / 3);
            int bottomSectionHeight = Math.Max(24, contentHeight - topSectionHeight);

            // Top section for main chart
            ctx.ChartRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                topSectionHeight
            );

            // Bottom section for metrics
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ChartRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                bottomSectionHeight
            );

            // Precompute metric rectangles for hit testing
            _metricRects = CalculateMetricRects(ctx.ContentRect, ctx);
            _chartHitRect = ctx.ChartRect;

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

            // Draw title with enhanced styling
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                DrawAnalyticsTitle(g, ctx);
            }

            // Draw main chart area with enhanced visuals
            DrawMainChart(g, ctx.ChartRect);

            // Draw bottom metrics with icons
            if (ctx.CustomData.ContainsKey("Metrics"))
            {
                var metrics = (List<Dictionary<string, object>>)ctx.CustomData["Metrics"];
                DrawBottomMetrics(g, ctx.ContentRect, metrics);
            }
        }

        private void DrawAnalyticsTitle(Graphics g, WidgetContext ctx)
        {
            // Background gradient for title area
            using var titleBrush = new LinearGradientBrush(
                ctx.HeaderRect,
                Color.FromArgb(20, Theme?.PrimaryColor ?? Color.Blue),
                Color.FromArgb(5, Theme?.PrimaryColor ?? Color.Blue),
                LinearGradientMode.Horizontal);
            
            g.FillRoundedRectangle(titleBrush, ctx.HeaderRect, 4);

            // Title icon
            var iconRect = new Rectangle(ctx.HeaderRect.X + 8, ctx.HeaderRect.Y + 4, 16, 16);
            if (ctx.CustomData.ContainsKey("TitleIcon"))
            {
                var iconSource = ctx.CustomData["TitleIcon"].ToString();
                _imagePainter.DrawSvg(g, iconSource, iconRect, 
                    Theme?.ForeColor ?? Color.Black, 0.8f);
            }
            else
            {
                // Default analytics icon
                _imagePainter.DrawSvg(g, "bar-chart-2", iconRect, 
                    Theme?.ForeColor ?? Color.Black, 0.7f);
            }

            // Title text
            var titleTextRect = new Rectangle(iconRect.Right + 8, ctx.HeaderRect.Y, 
                ctx.HeaderRect.Width - iconRect.Width - 24, ctx.HeaderRect.Height);
            using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush2 = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.Title, titleFont, titleBrush2, titleTextRect, format);
        }

        private void DrawMainChart(Graphics g, Rectangle rect)
        {
            // Enhanced chart background with gradient
            using var chartBrush = new LinearGradientBrush(
                rect,
                Color.FromArgb(15, Theme?.BackColor ?? Color.White),
                Color.FromArgb(5, Theme?.SecondaryColor ?? Color.Gray),
                LinearGradientMode.Vertical);
            using var chartPath = CreateRoundedPath(rect, 8);
            g.FillPath(chartBrush, chartPath);

            // Border
            using var borderPen = new Pen(Color.FromArgb(30, Theme?.BorderColor ?? Color.Gray), 1);
            g.DrawPath(borderPen, chartPath);

            // Chart header with icon
            var headerRect = new Rectangle(rect.X + 12, rect.Y + 8, rect.Width - 24, 24);
            var iconRect = new Rectangle(headerRect.X, headerRect.Y + 4, 16, 16);
            
            _imagePainter.DrawSvg(g, "trending-up", iconRect, 
                Theme?.PrimaryColor ?? Color.Blue, 0.8f);

            var titleRect = new Rectangle(iconRect.Right + 8, headerRect.Y, 
                headerRect.Width - iconRect.Width - 8, headerRect.Height);
            using var chartTitleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 10f,FontStyle.Bold);
            using var chartTitleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString("Analytics Overview", chartTitleFont, chartTitleBrush, titleRect, format);

            // Sample line chart with enhanced styling
            var sampleData = new List<double> { 20, 25, 30, 28, 35, 40, 38, 45, 42, 50 };
            var chartArea = new Rectangle(rect.X + 16, headerRect.Bottom + 8, rect.Width - 32, rect.Height - headerRect.Height - 24);
            WidgetRenderingHelpers.DrawLineChart(g, chartArea, sampleData, 
                Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 3f);

            // Hover effect for chart area
            if (IsAreaHovered("AnalyticsPanel_Chart"))
            {
                using var glow = new SolidBrush(Color.FromArgb(20, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRoundedRectangle(glow, Rectangle.Inflate(rect, 3, 3), 10);
            }
        }

        private void DrawBottomMetrics(Graphics g, Rectangle rect, List<Dictionary<string, object>> metrics)
        {
            if (!metrics.Any()) return;

            // Limit to 4 metrics like web dashboards
            int count = Math.Min(metrics.Count, 4);
            int metricWidth = rect.Width / count;
            using var valueFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 14f, FontStyle.Bold);
            using var labelFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);

            string[] defaultIcons = { "dollar-sign", "users", "bar-chart", "trending-up" };

            for (int i = 0; i < count; i++)
            {
                var metric = metrics[i];
                var metricRect = new Rectangle(rect.X + i * metricWidth, rect.Y, metricWidth - 12, rect.Height);

                // Cache rect for hover/hit area visuals
                if (i < _metricRects.Count) _metricRects[i] = metricRect;

                bool hovered = IsAreaHovered($"AnalyticsPanel_Metric_{i}");

                // Metric card background
                using var cardBrush = new SolidBrush(Color.FromArgb(hovered ? 14 : 8, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRoundedRectangle(cardBrush, metricRect, 6);

                // Hover border
                if (hovered)
                {
                    using var hoverPen = new Pen(Color.FromArgb(140, Theme?.PrimaryColor ?? Color.Blue), 1.5f);
                    g.DrawRoundedRectangle(hoverPen, metricRect, 6);
                }

                // Icon area
                var iconRect = new Rectangle(metricRect.X + 8, metricRect.Y + 8, 20, 20);
                string iconName = metric.ContainsKey("Icon") ? metric["Icon"].ToString() : defaultIcons[i % defaultIcons.Length];
                Color iconColor = metric.ContainsKey("Color") ? (Color)metric["Color"] : Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
                
                _imagePainter.DrawSvg(g, iconName, iconRect, iconColor, hovered ? 0.85f : 0.7f);

                // Value area
                var valueRect = new Rectangle(metricRect.X + 8, iconRect.Bottom + 4, metricRect.Width - 16, 20);
                if (metric.ContainsKey("Value"))
                {
                    using var valueBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                    var valueFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    string valueText = metric["Value"].ToString();
                    if (hovered) valueText += "  ›";
                    g.DrawString(valueText, valueFont, valueBrush, valueRect, valueFormat);
                }

                // Label area
                var labelRect = new Rectangle(metricRect.X + 8, valueRect.Bottom + 2, metricRect.Width - 16, 16);
                if (metric.ContainsKey("Title"))
                {
                    using var labelBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
                    var labelFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    g.DrawString(metric["Title"].ToString(), labelFont, labelBrush, labelRect, labelFormat);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw drill-down indicator if interactive
            if (ctx.CustomData.ContainsKey("IsInteractive") && (bool)ctx.CustomData["IsInteractive"])
            {
                var indicatorRect = new Rectangle(ctx.DrawingRect.Right - 24, ctx.DrawingRect.Y + 8, 16, 16);
                _imagePainter.DrawSvg(g, "external-link", indicatorRect, 
                    Color.FromArgb(100, Theme?.AccentColor ?? Color.Gray), 0.6f);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            // Chart area hit
            if (!_chartHitRect.IsEmpty)
            {
                owner.AddHitArea("AnalyticsPanel_Chart", _chartHitRect, null, () =>
                {
                    ctx.CustomData["ChartClicked"] = true;
                    notifyAreaHit?.Invoke("AnalyticsPanel_Chart", _chartHitRect);
                    Owner?.Invalidate();
                });
            }

            // Metric cards hit areas
            for (int i = 0; i < _metricRects.Count; i++)
            {
                int idx = i;
                var rect = _metricRects[i];
                if (rect.IsEmpty) continue;
                owner.AddHitArea($"AnalyticsPanel_Metric_{idx}", rect, null, () =>
                {
                    ctx.CustomData["SelectedMetricIndex"] = idx;
                    notifyAreaHit?.Invoke($"AnalyticsPanel_Metric_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }

        private List<Rectangle> CalculateMetricRects(Rectangle area, WidgetContext ctx)
        {
            var result = new List<Rectangle>();
            if (!ctx.CustomData.ContainsKey("Metrics")) return result;
            var metrics = (List<Dictionary<string, object>>)ctx.CustomData["Metrics"];
            if (metrics == null || metrics.Count == 0) return result;
            int count = Math.Min(metrics.Count, 4);
            int metricWidth = area.Width / count;
            for (int i = 0; i < count; i++)
            {
                result.Add(new Rectangle(area.X + i * metricWidth, area.Y, metricWidth - 12, area.Height));
            }
            return result;
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}
