using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Chart
{
    internal sealed class GaugeChartPainter : WidgetPainterBase
    {
        private Rectangle _chartRectCache;
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 20);
            
            int gaugeSize = Math.Min(ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - pad * 2 - 40);
            ctx.ChartRect = new Rectangle(
                ctx.DrawingRect.Left + (ctx.DrawingRect.Width - gaugeSize) / 2,
                ctx.HeaderRect.Bottom + 10,
                gaugeSize, gaugeSize
            );
            
            ctx.ValueRect = new Rectangle(
                ctx.ChartRect.Left + gaugeSize / 4,
                ctx.ChartRect.Top + gaugeSize / 2,
                gaugeSize / 2,
                gaugeSize / 4
            );
            
            _chartRectCache = ctx.ChartRect;
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
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }
            
            if (ctx.Values?.Any() == true)
            {
                double value = ctx.Values.First();
                double minValue = ctx.CustomData.ContainsKey("MinValue") ? Convert.ToDouble(ctx.CustomData["MinValue"]) : 0d;
                double maxValue = ctx.CustomData.ContainsKey("MaxValue") ? Convert.ToDouble(ctx.CustomData["MaxValue"]) : 100d;
                
                WidgetRenderingHelpers.DrawGauge(g, ctx.ChartRect, value, minValue, maxValue, ctx.AccentColor, Color.FromArgb(30, Theme?.BorderColor ?? Color.Gray), 10);
            }
            
            if (ctx.Values?.Any() == true)
            {
                using var valueFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 12f, FontStyle.Bold);
                using var valueBrush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Values.First().ToString("F1"), valueFont, valueBrush, ctx.ValueRect, format);
            }

            if (IsAreaHovered("GaugeChart_Gauge"))
            {
                using var hover = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                g.FillEllipse(hover, Rectangle.Inflate(ctx.ChartRect, 2, 2));
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            if (!_chartRectCache.IsEmpty)
            {
                owner.AddHitArea("GaugeChart_Gauge", _chartRectCache, null, () =>
                {
                    ctx.CustomData["GaugeClicked"] = true;
                    notifyAreaHit?.Invoke("GaugeChart_Gauge", _chartRectCache);
                    Owner?.Invalidate();
                });
            }
        }
    }
}