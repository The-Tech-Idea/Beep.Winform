using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Metric
{
    /// <summary>
    /// CardMetric - Card-style with icon and enhanced visual presentation
    /// </summary>
    internal sealed class CardMetricPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public CardMetricPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 20;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);

            // Icon in top-left
            int iconSize = 32;
            ctx.IconRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, iconSize, iconSize);

            // Title in top-right area
            ctx.HeaderRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - (ctx.IconRect.Right + 12 - ctx.DrawingRect.Left) - pad,
                20
            );

            // Value below icon and title
            ctx.ValueRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                Math.Max(ctx.IconRect.Bottom, ctx.HeaderRect.Bottom) + 12,
                ctx.DrawingRect.Width - pad * 2,
                36
            );

            // Trend at bottom-right
            if (ctx.ShowTrend)
            {
                ctx.TrendRect = new Rectangle(
                    ctx.DrawingRect.Right - pad - 80,
                    ctx.DrawingRect.Bottom - pad - 20,
                    75, 16
                );
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
            _imagePainter.UseThemeColors = true;

            // Draw modern metric card content
            DrawMetricIcon(g, ctx);
            DrawMetricTitle(g, ctx);
            DrawMetricValue(g, ctx);
            DrawMetricSubtext(g, ctx);
        }

        private void DrawMetricIcon(Graphics g, WidgetContext ctx)
        {
            if (!ctx.ShowIcon) return;

            // Modern icon background
            using var iconBgBrush = new SolidBrush(Color.FromArgb(15, ctx.AccentColor));
            g.FillRoundedRectangle(iconBgBrush, ctx.IconRect, 8);

            // Metric type icon
            string iconName = ctx.CustomData.ContainsKey("MetricType") 
                ? GetMetricIcon(ctx.CustomData["MetricType"].ToString())
                : "activity";
            
            var iconRect = Rectangle.Inflate(ctx.IconRect, -6, -6);
            _imagePainter.DrawSvg(g, iconName, iconRect, ctx.AccentColor, 0.9f);
        }

        private void DrawMetricTitle(Graphics g, WidgetContext ctx)
        {
            if (string.IsNullOrEmpty(ctx.Title)) return;

            using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Regular);
            using var titleBrush = new SolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Black));
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
        }

        private void DrawMetricValue(Graphics g, WidgetContext ctx)
        {
            if (string.IsNullOrEmpty(ctx.Value)) return;

            using var valueFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 24f, FontStyle.Bold);
            using var valueBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            
            string displayValue = ctx.Value;
            if (!string.IsNullOrEmpty(ctx.Units))
                displayValue += $" {ctx.Units}";
                
            g.DrawString(displayValue, valueFont, valueBrush, ctx.ValueRect, format);
        }

        private void DrawMetricSubtext(Graphics g, WidgetContext ctx)
        {
            if (!ctx.ShowTrend || string.IsNullOrEmpty(ctx.TrendValue)) return;

            // Trend indicator with icon
            var trendRect = ctx.TrendRect;
            var iconSize = 14;
            var trendIconRect = new Rectangle(trendRect.X, trendRect.Y + (trendRect.Height - iconSize) / 2, iconSize, iconSize);
            
            string trendIcon = ctx.TrendColor == Color.Green ? "trending-up" : 
                              ctx.TrendColor == Color.Red ? "trending-down" : "minus";
            _imagePainter.DrawSvg(g, trendIcon, trendIconRect, ctx.TrendColor, 0.8f);

            // Trend text
            var textRect = new Rectangle(trendIconRect.Right + 4, trendRect.Y, 
                trendRect.Width - iconSize - 4, trendRect.Height);
            using var trendFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular);
            using var trendBrush = new SolidBrush(ctx.TrendColor);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.TrendValue, trendFont, trendBrush, textRect, format);
        }

        private string GetMetricIcon(string metricType)
        {
            return metricType?.ToLower() switch
            {
                "sales" => "dollar-sign",
                "users" => "users",
                "revenue" => "trending-up",
                "orders" => "shopping-cart",
                "growth" => "arrow-up",
                "performance" => "zap",
                "conversion" => "target",
                "traffic" => "activity",
                "engagement" => "heart",
                _ => "bar-chart"
            };
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw icon background circle
            if (ctx.ShowIcon)
            {
                using var iconBgBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
                var iconBgRect = new Rectangle(ctx.IconRect.X - 4, ctx.IconRect.Y - 4, ctx.IconRect.Width + 8, ctx.IconRect.Height + 8);
                g.FillEllipse(iconBgBrush, iconBgRect);
            }

            // Draw trend
            if (ctx.ShowTrend && !string.IsNullOrEmpty(ctx.TrendValue))
            {
                using var trendFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
                using var trendBrush = new SolidBrush(ctx.TrendColor);
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.TrendValue, trendFont, trendBrush, ctx.TrendRect, format);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            
            // Icon is clickable if shown
            if (ctx.ShowIcon && !ctx.IconRect.IsEmpty)
            {
                owner.AddHitArea("CardMetric_Icon", ctx.IconRect, null, () =>
                {
                    ctx.CustomData["IconClicked"] = true;
                    notifyAreaHit?.Invoke("CardMetric_Icon", ctx.IconRect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}
