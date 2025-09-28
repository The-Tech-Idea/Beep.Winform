using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Chart
{
    /// <summary>
    /// BarChart - Vertical/horizontal bar chart
    /// </summary>
    internal sealed class BarChartPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                20
            );
            
            if (ctx.ShowLegend)
            {
                ctx.LegendRect = new Rectangle(
                    ctx.DrawingRect.Left + pad,
                    ctx.DrawingRect.Bottom - pad - 20,
                    ctx.DrawingRect.Width - pad * 2,
                    20
                );
            }
            
            int chartTop = ctx.HeaderRect.Bottom + 8;
            int chartBottom = ctx.ShowLegend ? ctx.LegendRect.Top - 8 : ctx.DrawingRect.Bottom - pad;
            ctx.ChartRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                chartTop,
                ctx.DrawingRect.Width - pad * 2,
                chartBottom - chartTop
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
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }
            
            if (ctx.Values?.Any() == true)
            {
                WidgetRenderingHelpers.DrawBarChart(g, ctx.ChartRect, ctx.Values, ctx.AccentColor, Color.FromArgb(10, Color.Gray));
            }
        }
        
        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            if (ctx.ShowLegend && ctx.Labels?.Any() == true)
            {
                DrawSimpleLegend(g, ctx.LegendRect, ctx.Labels.Take(ctx.Colors.Count).ToList(), ctx.Colors);
            }
        }
        
        private void DrawSimpleLegend(Graphics g, Rectangle rect, System.Collections.Generic.List<string> labels, System.Collections.Generic.List<Color> colors)
        {
            if (!labels.Any() || !colors.Any()) return;
            
            int itemWidth = rect.Width / Math.Min(labels.Count, 4);
            using var legendFont = new Font(Owner.Font.FontFamily, 8f);
            
            for (int i = 0; i < Math.Min(Math.Min(labels.Count, colors.Count), 4); i++)
            {
                int x = rect.X + i * itemWidth;
                var colorRect = new Rectangle(x, rect.Y + 6, 12, 8);
                var textRect = new Rectangle(x + 16, rect.Y, itemWidth - 16, rect.Height);
                
                using var colorBrush = new SolidBrush(colors[i]);
                using var textBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                
                g.FillRectangle(colorBrush, colorRect);
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(labels[i], legendFont, textBrush, textRect, format);
            }
        }
    }
}