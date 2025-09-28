using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Dashboard
{
    /// <summary>
    /// AnalyticsPanel - Complex analytics layout with enhanced image rendering
    /// </summary>
    internal sealed class AnalyticsPanelPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public AnalyticsPanelPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);

            // Split content into sections
            int contentHeight = ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3;
            int topSectionHeight = contentHeight * 2 / 3;
            int bottomSectionHeight = contentHeight / 3;

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
            _imagePainter.Theme = Theme;
            _imagePainter.UseThemeColors = true;

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
                _imagePainter.DrawImage(g, iconSource, iconRect, 
                    BaseImage.ScalingMode.KeepAspectRatio, 0.8f);
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
            using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
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
            using var chartTitleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.SemiBold);
            using var chartTitleBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString("Analytics Overview", chartTitleFont, chartTitleBrush, titleRect, format);

            // Sample line chart with enhanced styling
            var sampleData = new List<double> { 20, 25, 30, 28, 35, 40, 38, 45, 42, 50 };
            var chartArea = new Rectangle(rect.X + 16, headerRect.Bottom + 8, rect.Width - 32, rect.Height - headerRect.Height - 24);
            WidgetRenderingHelpers.DrawLineChart(g, chartArea, sampleData, 
                Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 3f);
        }

        private void DrawBottomMetrics(Graphics g, Rectangle rect, List<Dictionary<string, object>> metrics)
        {
            if (!metrics.Any()) return;

            int metricWidth = rect.Width / Math.Min(metrics.Count, 4);
            using var valueFont = new Font(Owner.Font.FontFamily, 14f, FontStyle.Bold);
            using var labelFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);

            string[] defaultIcons = { "dollar-sign", "users", "bar-chart", "trending-up" };

            for (int i = 0; i < Math.Min(metrics.Count, 4); i++)
            {
                var metric = metrics[i];
                var metricRect = new Rectangle(rect.X + i * metricWidth, rect.Y, metricWidth - 12, rect.Height);

                // Metric card background
                using var cardBrush = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRoundedRectangle(cardBrush, metricRect, 6);

                // Icon area
                var iconRect = new Rectangle(metricRect.X + 8, metricRect.Y + 8, 20, 20);
                string iconName = metric.ContainsKey("Icon") ? metric["Icon"].ToString() : defaultIcons[i % defaultIcons.Length];
                Color iconColor = metric.ContainsKey("Color") ? (Color)metric["Color"] : Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
                
                _imagePainter.DrawSvg(g, iconName, iconRect, iconColor, 0.7f);

                // Value area
                var valueRect = new Rectangle(metricRect.X + 8, iconRect.Bottom + 4, metricRect.Width - 16, 20);
                if (metric.ContainsKey("Value"))
                {
                    using var valueBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                    var valueFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    g.DrawString(metric["Value"].ToString(), valueFont, valueBrush, valueRect, valueFormat);
                }

                // Label area
                var labelRect = new Rectangle(metricRect.X + 8, valueRect.Bottom + 2, metricRect.Width - 16, 16);
                if (metric.ContainsKey("Title"))
                {
                    using var labelBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
                    var labelFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                    g.DrawString(metric["Title"].ToString(), labelFont, labelBrush, labelRect, labelFormat);
                }

                // Subtle border
                using var borderPen = new Pen(Color.FromArgb(20, Theme?.BorderColor ?? Color.Gray), 1);
                g.DrawRoundedRectangle(borderPen, metricRect, 6);
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

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}
