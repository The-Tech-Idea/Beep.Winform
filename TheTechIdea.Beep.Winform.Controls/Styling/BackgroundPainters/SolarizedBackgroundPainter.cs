using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class SolarizedBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors ? theme.BackgroundColor : Color.FromArgb(0x00, 0x2B, 0x36);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
