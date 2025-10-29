using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class ArcLinuxBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(0x2F,0x2E,0x2E);
            Color accent = useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(0x8A,0xCB,0x49);

            var fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);
            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            var bounds = path.GetBounds();
            var topRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height /5f);
            var grad = PaintersFactory.GetLinearGradientBrush(topRect, Color.FromArgb(30, accent), Color.Transparent, LinearGradientMode.Vertical);
            g.FillRectangle(grad, topRect);
        }
    }
}
