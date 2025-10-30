using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    internal sealed class OutlineChartPainter : ChartPainterBase
    {
        public override ChartLayout AdjustLayout(Rectangle drawingRect, ChartLayout ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -1, -1);
            ctx.PlotRect = Rectangle.Empty;
            return ctx;
        }

        public override void DrawBackground(Graphics g, ChartLayout ctx)
        {
            var bg = PaintersFactory.GetSolidBrush(Theme?.CardBackColor ?? Color.White);
            using var path = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(bg, path);
            var pen = PaintersFactory.GetPen(Theme?.StatsCardBorderColor ?? Color.Silver, 1);
            g.DrawPath(pen, path);
        }

        public override void DrawForeground(Graphics g, ChartLayout ctx) { }
    }

    internal sealed class GlassChartPainter2 : ChartPainterBase
    {
        public override ChartLayout AdjustLayout(Rectangle drawingRect, ChartLayout ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -2, -2);
            return ctx;
        }

        public override void DrawBackground(Graphics g, ChartLayout ctx)
        {
            var glass = PaintersFactory.GetSolidBrush(Color.FromArgb(28, Color.White));
            using var path = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(glass, path);
            var pen = PaintersFactory.GetPen(Color.FromArgb(64, Color.White), 1);
            g.DrawPath(pen, path);
        }

        public override void DrawForeground(Graphics g, ChartLayout ctx)
        {
            var top = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Top, ctx.DrawingRect.Width, ctx.DrawingRect.Height / 3);
            var lg = PaintersFactory.GetLinearGradientBrush(top, Color.FromArgb(64, Color.White), Color.FromArgb(0, Color.White), LinearGradientMode.Vertical);
            g.FillRectangle(lg, top);
        }
    }
}