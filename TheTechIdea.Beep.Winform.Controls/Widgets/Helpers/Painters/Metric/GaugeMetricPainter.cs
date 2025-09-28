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
    /// GaugeMetric - Circular gauge display with enhanced visual presentation
    /// </summary>
    internal sealed class GaugeMetricPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public GaugeMetricPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);

            // Gauge takes most of the space
            int gaugeSize = Math.Min(ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - pad * 2 - 40);
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + (ctx.DrawingRect.Width - gaugeSize) / 2,
                ctx.DrawingRect.Top + pad,
                gaugeSize, gaugeSize
            );

            // Value in center of gauge
            ctx.ValueRect = new Rectangle(
                ctx.ContentRect.Left + gaugeSize / 4,
                ctx.ContentRect.Top + gaugeSize / 3,
                gaugeSize / 2,
                gaugeSize / 3
            );

            // Title below gauge
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                20
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
            // Draw gauge
            WidgetRenderingHelpers.DrawGauge(g, ctx.ContentRect, ctx.TrendPercentage, 0, 100, ctx.AccentColor, Color.FromArgb(30, Color.Gray), 8);

            // Draw value in center
            if (!string.IsNullOrEmpty(ctx.Value))
            {
                using var valueFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
                DrawValue(g, ctx.ValueRect, ctx.Value, ctx.Units, valueFont, ctx.AccentColor);
            }

            // Draw title
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 9f);
                using var titleBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw percentage labels around gauge
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Gauge is clickable
            if (!ctx.ContentRect.IsEmpty)
            {
                owner.AddHitArea("Gauge", ctx.ContentRect, null, () => notifyAreaHit?.Invoke("Gauge", ctx.ContentRect));
            }
        }
    }
}
