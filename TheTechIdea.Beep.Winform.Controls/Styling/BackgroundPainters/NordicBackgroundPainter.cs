using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class NordicBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors ? theme.BackgroundColor : Color.FromArgb(0xF2, 0xF5, 0xF8);
            Color accent = Color.FromArgb(0xA3, 0xB8, 0xC9);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            var bounds = path.GetBounds();
            using (var gradient = new LinearGradientBrush(bounds, Color.FromArgb(0, accent), Color.FromArgb(40, accent), LinearGradientMode.Vertical))
            {
                g.FillPath(gradient, path);
            }
        }
    }
}
