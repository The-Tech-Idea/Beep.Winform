using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Charts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Charts
{
    internal sealed class OutlineChartPainter : ChartPainterBase
    {
        public override ChartLayout AdjustLayout(Rectangle drawingRect, ChartLayout ctx)
        {
            // Outer bounds — 1 px inset for the outline border
            ctx.DrawingRect = SafeInflate(drawingRect, -1, -1);

            // Inner content area — padding inside the outline for title, axes, data
            int contentPadding = 8;
            ctx.PlotRect = SafeInflate(ctx.DrawingRect, -contentPadding, -contentPadding);
            return ctx;
        }

        private static Rectangle SafeInflate(Rectangle r, int dx, int dy)
        {
            var result = Rectangle.Inflate(r, dx, dy);
            if (result.Width < 1) result.Width = 1;
            if (result.Height < 1) result.Height = 1;
            return result;
        }

        public override void DrawBackground(Graphics g, ChartLayout ctx)
        {
            var t = Theme ?? BeepThemesManager.CurrentTheme;
            var bg = PaintersFactory.GetSolidBrush(t.ChartBackColor);
            using var path = Round(ctx.DrawingRect, ctx.Radius);
            g.FillPath(bg, path);
            var pen = PaintersFactory.GetPen(t.ChartAxisColor, 1);
            g.DrawPath(pen, path);
        }

        public override void DrawForeground(Graphics g, ChartLayout ctx) { }
    }

    internal sealed class GlassChartPainter2 : ChartPainterBase
    {
        public override ChartLayout AdjustLayout(Rectangle drawingRect, ChartLayout ctx)
        {
            // Outer bounds — 2 px inset for the glass border
            ctx.DrawingRect = SafeInflate(drawingRect, -2, -2);

            // Inner content area — padding inside the glass for title, axes, data
            int contentPadding = 10;
            ctx.PlotRect = SafeInflate(ctx.DrawingRect, -contentPadding, -contentPadding);
            return ctx;
        }

        private static Rectangle SafeInflate(Rectangle r, int dx, int dy)
        {
            var result = Rectangle.Inflate(r, dx, dy);
            if (result.Width < 1) result.Width = 1;
            if (result.Height < 1) result.Height = 1;
            return result;
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