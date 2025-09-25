using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DataControls.Helpers
{
    internal sealed class FlatUnderlineNavigatorPainter : NavigatorPainterBase
    {
        public override NavigatorLayout AdjustLayout(Rectangle drawingRect, NavigatorLayout ctx)
        {
            ctx.DrawingRect = drawingRect; // no padding
            return ctx;
        }

        public override void DrawBackground(Graphics g, NavigatorLayout ctx)
        {
            using var bg = new SolidBrush(Theme?.PanelBackColor ?? Color.Transparent);
            g.FillRectangle(bg, ctx.DrawingRect);
        }

        public override void DrawForeground(Graphics g, NavigatorLayout ctx)
        {
            var line = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Bottom - 2, ctx.DrawingRect.Width, 2);
            using var accent = new SolidBrush(ctx.AccentColor);
            g.FillRectangle(accent, line);
        }
    }
}
