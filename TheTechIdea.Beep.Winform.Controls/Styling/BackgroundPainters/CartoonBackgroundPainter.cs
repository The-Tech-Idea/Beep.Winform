using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class CartoonBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color bg = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(0xFF,0xF5,0xE6);
            Color accent = useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(0xFF,0x99,0x33);

            var fill = BackgroundPainterHelpers.ApplyState(bg, state);
            var brush = PaintersFactory.GetSolidBrush(fill);
            g.FillPath(brush, path);

            var bounds = path.GetBounds();
            var topGrad = PaintersFactory.GetLinearGradientBrush(new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height /4f), Color.FromArgb(40, Color.White), Color.Transparent, LinearGradientMode.Vertical);
            g.FillRectangle(topGrad, new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height /4f));

            var pen = PaintersFactory.GetPen(Color.FromArgb(30, accent),1f);
            g.DrawPath(pen, path);
        }
    }
}
