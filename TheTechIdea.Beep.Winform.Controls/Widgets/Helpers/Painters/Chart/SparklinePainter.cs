using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Chart
{
    /// <summary>
    /// Sparkline - Mini trend line
    /// </summary>
    internal sealed class SparklinePainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Sparkline takes most of the space (minimal padding)
            ctx.ChartRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - pad * 2
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Minimal background for sparklines
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw sparkline
            if (ctx.Values?.Any() == true)
            {
                WidgetRenderingHelpers.DrawSparkline(g, ctx.ChartRect, ctx.Values, ctx.AccentColor);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Highlight last point
            if (ctx.Values?.Any() == true && ctx.ChartRect.Width > 0)
            {
                var values = ctx.Values.ToList();
                var minValue = values.Min();
                var maxValue = values.Max();
                var valueRange = maxValue - minValue;

                if (valueRange > 0)
                {
                    float x = ctx.ChartRect.Right - 2;
                    float y = (float)(ctx.ChartRect.Bottom - (float)(values.Last() - minValue) / valueRange * ctx.ChartRect.Height);

                    using var pointBrush = new SolidBrush(ctx.AccentColor);
                    g.FillEllipse(pointBrush, x - 2, y - 2, 4, 4);
                }
            }
        }
    }
}
