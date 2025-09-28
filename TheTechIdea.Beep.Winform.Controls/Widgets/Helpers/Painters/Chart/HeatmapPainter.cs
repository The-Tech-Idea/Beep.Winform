using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Chart
{
    /// <summary>
    /// HeatmapChart - Calendar/grid heatmap
    /// </summary>
    internal sealed class HeatmapPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 20);

            ctx.ChartRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3
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

            // Draw heatmap grid
            if (ctx.Values?.Any() == true)
            {
                int cols = 7; // Week view
                int rows = (int)Math.Ceiling(ctx.Values.Count / (double)cols);

                int cellWidth = ctx.ChartRect.Width / cols;
                int cellHeight = ctx.ChartRect.Height / rows;

                var maxValue = ctx.Values.Max();

                for (int i = 0; i < ctx.Values.Count && i < cols * rows; i++)
                {
                    int col = i % cols;
                    int row = i / cols;

                    var cellRect = new Rectangle(
                        ctx.ChartRect.X + col * cellWidth + 1,
                        ctx.ChartRect.Y + row * cellHeight + 1,
                        cellWidth - 2,
                        cellHeight - 2
                    );

                    double intensity = maxValue > 0 ? ctx.Values[i] / maxValue : 0;
                    WidgetRenderingHelpers.DrawHeatmapCell(g, cellRect, intensity, ctx.AccentColor, 2);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw grid lines or labels
        }
    }
}
