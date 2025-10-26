using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class DraculaBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors ? theme.BackgroundColor : Color.FromArgb(0x28, 0x2A, 0x36);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            using (var vignette = new PathGradientBrush(path))
            {
                vignette.CenterColor = Color.FromArgb(0, fillColor);
                vignette.SurroundColors = new[] { Color.FromArgb(60, 0, 0, 0) };
                g.FillPath(vignette, path);
            }
        }
    }
}
