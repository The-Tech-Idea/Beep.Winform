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
    /// TrendMetric - Value with prominent trend indicator and enhanced visual presentation
    /// </summary>
    internal sealed class TrendMetricPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public TrendMetricPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);

            // Title at top
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                18
            );

            // Main value - left side
            ctx.ValueRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                (ctx.DrawingRect.Width - pad * 3) * 2 / 3,
                36
            );

            // Trend indicator - right side (prominent)
            ctx.TrendRect = new Rectangle(
                ctx.ValueRect.Right + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Right - ctx.ValueRect.Right - pad * 2,
                36
            );

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
                using var titleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var titleBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }

            // Draw main value
            if (!string.IsNullOrEmpty(ctx.Value))
            {
                using var valueFont = new Font(Owner.Font.FontFamily, 16f, FontStyle.Bold);
                using var valueBrush = new SolidBrush(ctx.AccentColor);
                DrawValue(g, ctx.ValueRect, ctx.Value, ctx.Units, valueFont, valueBrush.Color, StringAlignment.Near);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw prominent trend indicator
            if (ctx.ShowTrend && !string.IsNullOrEmpty(ctx.TrendValue))
            {
                // Background for trend
                using var trendBgBrush = new SolidBrush(Color.FromArgb(20, ctx.TrendColor));
                using var trendBgPath = CreateRoundedPath(ctx.TrendRect, 6);
                g.FillPath(trendBgBrush, trendBgPath);

                // Trend arrow (larger)
                var trendArrowRect = new Rectangle(ctx.TrendRect.X + 8, ctx.TrendRect.Y + 8, 20, 20);
                DrawTrendArrow(g, trendArrowRect, ctx.TrendDirection, ctx.TrendColor);

                // Trend text
                var trendTextRect = new Rectangle(
                    trendArrowRect.Right + 4,
                    ctx.TrendRect.Y,
                    ctx.TrendRect.Width - 32,
                    ctx.TrendRect.Height
                );
                using var trendFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var trendBrush = new SolidBrush(ctx.TrendColor);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.TrendValue, trendFont, trendBrush, trendTextRect, format);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Add clickable trend area (enhanced interactivity)
            if (ctx.ShowTrend && !ctx.TrendRect.IsEmpty)
            {
                owner.AddHitArea("TrendDetails", ctx.TrendRect, null, () => notifyAreaHit?.Invoke("TrendDetails", ctx.TrendRect));
            }
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }

}
