using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Map
{
    /// <summary>
    /// GeographicHeatmap - Location-based data heatmap painter
    /// </summary>
    internal sealed class GeographicHeatmapPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            using var font = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var brush = new SolidBrush(Color.FromArgb(140, Color.Black));
            g.DrawString("Geographic Heatmap implementation pending...", font, brush, ctx.DrawingRect);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }
    }
}