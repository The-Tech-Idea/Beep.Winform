using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Chart
{

    /// <summary>
    /// CombinationChart - Multiple chart types combined
    /// </summary>
    internal sealed class CombinationChartPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 20);

            // Split chart area for combination
            int chartTop = ctx.HeaderRect.Bottom + 8;
            int chartHeight = ctx.DrawingRect.Bottom - chartTop - pad;

            ctx.ChartRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                chartTop,
                ctx.DrawingRect.Width - pad * 2,
                chartHeight
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
                using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }

            // Draw combination of bar and line
            if (ctx.Values?.Any() == true)
            {
                // Draw bars (first half of values)
                var halfCount = ctx.Values.Count / 2;
                if (halfCount > 0)
                {
                    var barValues = ctx.Values.Take(halfCount).ToList();
                    WidgetRenderingHelpers.DrawBarChart(g, ctx.ChartRect, barValues, Color.FromArgb(100, ctx.AccentColor), Color.FromArgb(10, Color.Gray));
                }

                // Draw line (second half of values)
                if (ctx.Values.Count > halfCount)
                {
                    var lineValues = ctx.Values.Skip(halfCount).ToList();
                    WidgetRenderingHelpers.DrawLineChart(g, ctx.ChartRect, lineValues, ctx.AccentColor, 2f);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw combination legend or indicators
        }
    }
}
