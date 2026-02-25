using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal sealed class CardChartPainter : ChartPainterBase
    {
        public override ChartLayout AdjustLayout(Rectangle drawingRect, ChartLayout ctx)
        {
            // Card outer bounds — leave space for the soft shadow
            int shadowMargin = 6;
            ctx.DrawingRect = SafeInflate(drawingRect, -shadowMargin, -shadowMargin);

            // Inner content area — padding inside the card for title, axes, data
            int cardPadding = 12;
            ctx.PlotRect = SafeInflate(ctx.DrawingRect, -cardPadding, -cardPadding);
            return ctx;
        }

        /// <summary>Inflate that never produces negative Width / Height.</summary>
        private static Rectangle SafeInflate(Rectangle r, int dx, int dy)
        {
            var result = Rectangle.Inflate(r, dx, dy);
            if (result.Width < 1) result.Width = 1;
            if (result.Height < 1) result.Height = 1;
            return result;
        }

        public override void DrawBackground(Graphics g, ChartLayout ctx)
        {
            var bgColor = (Theme ?? BeepThemesManager.CurrentTheme).ChartBackColor;
            SoftShadow(g, ctx.DrawingRect, ctx.Radius + 2, layers: 5, offset: 3);
            var bg = PaintersFactory.GetSolidBrush(bgColor);
            using var path = Round(ctx.DrawingRect, ctx.Radius + 2);
            g.FillPath(bg, path);
        }

        public override void DrawForeground(Graphics g, ChartLayout ctx)
        {
            // accent underline drawn just below the title section
            // TitleBottom is the absolute Y where title text ends; fall back to PlotRect.Top
            int lineY = ctx.TitleBottom > ctx.DrawingRect.Top
                ? ctx.TitleBottom + 2
                : ctx.PlotRect.Top;
            // clamp so line doesn't go below the plot area
            lineY = Math.Min(lineY, ctx.PlotRect.Bottom - 4);
            var line = new Rectangle(ctx.DrawingRect.Left + 12, lineY, ctx.DrawingRect.Width - 24, 2);
            var accent = PaintersFactory.GetSolidBrush(System.Drawing.Color.FromArgb(64, ctx.AccentColor));
            g.FillRectangle(accent, line);
        }
    }
}
