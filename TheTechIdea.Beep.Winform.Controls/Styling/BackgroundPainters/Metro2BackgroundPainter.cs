using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Metro2 background painter - refined Windows 10/11 flat design
    /// Slightly softer than Metro, but still flat and geometric
    /// </summary>
    public static class Metro2BackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Metro2: slightly warmer off-white
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0xF7, 0xF7, 0xF9);

            // Flat design with normal state intensity
            using (var scope = new BackgroundPainterHelpers.SmoothingScope(g, SmoothingMode.None))
            {
                BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                    BackgroundPainterHelpers.StateIntensity.Normal);
            }
        }
    }
}
