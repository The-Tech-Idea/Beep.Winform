using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal sealed class CardChartPainter : ChartPainterBase
    {
        public override ChartLayout AdjustLayout(Rectangle drawingRect, ChartLayout ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -6, -6);
            ctx.PlotRect = Rectangle.Inflate(ctx.DrawingRect, -12, -12);
            return ctx;
        }

        public override void DrawBackground(Graphics g, ChartLayout ctx)
        {
            SoftShadow(g, ctx.DrawingRect, ctx.Radius + 2, layers: 5, offset: 3);
            var bg = PaintersFactory.GetSolidBrush(Theme?.CardBackColor ?? Color.White);
            using var path = Round(ctx.DrawingRect, ctx.Radius + 2);
            g.FillPath(bg, path);
        }

        public override void DrawForeground(Graphics g, ChartLayout ctx)
        {
            // accent underline below title area
            var line = new Rectangle(ctx.DrawingRect.Left + 12, ctx.DrawingRect.Top + 36, ctx.DrawingRect.Width - 24, 2);
            var accent = PaintersFactory.GetSolidBrush(Color.FromArgb(64, ctx.AccentColor));
            g.FillRectangle(accent, line);
        }
    }
}
