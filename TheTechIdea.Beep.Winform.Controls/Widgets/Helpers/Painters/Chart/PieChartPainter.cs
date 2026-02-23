using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Chart
{
    /// <summary>
    /// PieChart - Pie/donut chart with hover and hit area
    /// </summary>
    internal sealed class PieChartPainter : WidgetPainterBase
    {
        private Rectangle _chartRectCache;
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 20);
            
            int chartSize = Math.Min(ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - pad * 2 - 40);
            ctx.ChartRect = new Rectangle(
                ctx.DrawingRect.Left + (ctx.DrawingRect.Width - chartSize) / 2,
                ctx.HeaderRect.Bottom + 10,
                chartSize, chartSize
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
            
            if (ctx.Values?.Any() == true && ctx.Colors?.Any() == true)
            {
                var values = ctx.Values.ToList();
                var total = values.Sum();
                
                if (total > 0)
                {
                    float currentAngle = 0;
                    for (int i = 0; i < values.Count; i++)
                    {
                        float sweepAngle = (float)(values[i] / total * 360);
                        Color color = ctx.Colors != null && ctx.Colors.Count > 0 
                            ? ctx.Colors[i % ctx.Colors.Count] 
                            : ctx.AccentColor;
                        
                        WidgetRenderingHelpers.DrawPieSlice(g, ctx.ChartRect, currentAngle, sweepAngle, color);
                        currentAngle += sweepAngle;
                    }
                }
            }

            if (IsAreaHovered("PieChart_Chart"))
            {
                using var hover = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                g.FillEllipse(hover, Rectangle.Inflate(ctx.ChartRect, 2, 2));
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw percentage labels
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            if (!_chartRectCache.IsEmpty)
            {
                owner.AddHitArea("PieChart_Chart", _chartRectCache, null, () =>
                {
                    ctx.PieChartClicked = true;
                    notifyAreaHit?.Invoke("PieChart_Chart", _chartRectCache);
                    Owner?.Invalidate();
                });
            }
        }
    }
}