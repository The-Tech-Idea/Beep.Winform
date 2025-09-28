using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Chart
{
    /// <summary>
    /// LineChart - Line/area chart
    /// </summary>
    internal sealed class LineChartPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 20);
            
            if (ctx.ShowLegend)
            {
                ctx.LegendRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Bottom - pad - 20, ctx.DrawingRect.Width - pad * 2, 20);
            }
            
            int chartTop = ctx.HeaderRect.Bottom + 8;
            int chartBottom = ctx.ShowLegend ? ctx.LegendRect.Top - 8 : ctx.DrawingRect.Bottom - pad;
            ctx.ChartRect = new Rectangle(ctx.DrawingRect.Left + pad, chartTop, ctx.DrawingRect.Width - pad * 2, chartBottom - chartTop);
            
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
                using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }
            
            if (ctx.Values?.Any() == true)
            {
                WidgetRenderingHelpers.DrawLineChart(g, ctx.ChartRect, ctx.Values, ctx.AccentColor, 3f);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (ctx.Values?.Any() == true && ctx.ChartRect.Width > 0)
            {
                var values = ctx.Values.ToList();
                var minValue = values.Min();
                var maxValue = values.Max();
                var valueRange = maxValue - minValue;
                
                if (valueRange > 0)
                {
                    using var pointBrush = new SolidBrush(ctx.AccentColor);
                    for (int i = 0; i < values.Count; i++)
                    {
                        float x = ctx.ChartRect.X + (float)i / (values.Count - 1) * ctx.ChartRect.Width;
                        float y = (float)(ctx.ChartRect.Bottom - (float)(values[i] - minValue) / valueRange * ctx.ChartRect.Height);
                        g.FillEllipse(pointBrush, x - 3, y - 3, 6, 6);
                    }
                }
            }
        }
    }
}