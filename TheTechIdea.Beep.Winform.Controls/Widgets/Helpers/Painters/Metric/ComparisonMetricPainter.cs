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
    /// ComparisonMetric - Two values side-by-side
    /// </summary>
    internal sealed class ComparisonMetricPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public ComparisonMetricPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 18);

            // Split into two columns
            int halfWidth = (ctx.DrawingRect.Width - pad * 3) / 2;
            ctx.ValueRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, halfWidth, 32);
            ctx.TrendRect = new Rectangle(ctx.ValueRect.Right + pad, ctx.HeaderRect.Bottom + 8, halfWidth, 32);

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
            // Configure ImagePainter with theme
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var titleBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }

            // Decide primary color safely
            var primaryColor = ctx.AccentColor != Color.Empty ? ctx.AccentColor : (Theme != null ? Theme.PrimaryColor : Color.FromArgb(33, 150, 243));

            // Current value with icon
            if (!string.IsNullOrEmpty(ctx.Value))
            {
                using var valueFont = new Font(Owner.Font.FontFamily, 16f, FontStyle.Bold);
                DrawModernValue(g, ctx.ValueRect, ctx.Value, ctx.Units, valueFont, 
                    primaryColor, "trending-up");
            }

            // Comparison value with icon
            if (!string.IsNullOrEmpty(ctx.TrendValue))
            {
                using var trendFont = new Font(Owner.Font.FontFamily, 16f, FontStyle.Bold);
                var comparisonColor = GetComparisonColor(ctx.Value, ctx.TrendValue);
                var comparisonIcon = GetComparisonIcon(ctx.Value, ctx.TrendValue);
                DrawModernValue(g, ctx.TrendRect, ctx.TrendValue, ctx.Units, trendFont, 
                    comparisonColor, comparisonIcon);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw separator line
            using var separatorPen = new Pen(Color.FromArgb(50, Color.Gray), 1);
            int centerX = ctx.DrawingRect.Left + ctx.DrawingRect.Width / 2;
            g.DrawLine(separatorPen, centerX, ctx.ValueRect.Top, centerX, ctx.ValueRect.Bottom);
        }

        private void DrawModernValue(Graphics g, Rectangle rect, string value, string units, Font font, Color color, string iconName)
        {
            // Icon area
            var iconRect = new Rectangle(rect.X, rect.Y + 2, 20, 20);
            _imagePainter.DrawSvg(g, iconName, iconRect, color, 0.9f);

            // Value text
            using var valueBrush = new SolidBrush(color);
            var textRect = new Rectangle(rect.X + 26, rect.Y, rect.Width - 26, rect.Height);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            
            string displayText = !string.IsNullOrEmpty(units) ? $"{value} {units}" : value;
            g.DrawString(displayText, font, valueBrush, textRect, format);
        }

        private Color GetComparisonColor(string current, string comparison)
        {
            if (decimal.TryParse(current, out var currentVal) && decimal.TryParse(comparison, out var compVal))
            {
                return currentVal >= compVal ? 
                    Color.FromArgb(76, 175, 80) : // Green for higher
                    Color.FromArgb(244, 67, 54);  // Red for lower
            }
            return Color.FromArgb(158, 158, 158); // Gray for non-numeric
        }

        private string GetComparisonIcon(string current, string comparison)
        {
            if (decimal.TryParse(current, out var currentVal) && decimal.TryParse(comparison, out var compVal))
            {
                return currentVal >= compVal ? "trending-up" : "trending-down";
            }
            return "bar-chart-2";
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Both values are clickable
            if (!ctx.ValueRect.IsEmpty)
                owner.AddHitArea("CurrentValue", ctx.ValueRect, null, () => notifyAreaHit?.Invoke("CurrentValue", ctx.ValueRect));

            if (!ctx.TrendRect.IsEmpty)
                owner.AddHitArea("ComparisonValue", ctx.TrendRect, null, () => notifyAreaHit?.Invoke("ComparisonValue", ctx.TrendRect));
        }
    }
}
