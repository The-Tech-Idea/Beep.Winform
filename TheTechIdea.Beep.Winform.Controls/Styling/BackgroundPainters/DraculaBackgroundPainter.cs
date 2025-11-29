using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Dracula theme background painter - dark purple-tinted background
    /// Subtle vignette effect for depth, iconic developer theme aesthetic
    /// </summary>
    public static class DraculaBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Dracula's signature dark purple-gray background
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0x28, 0x2A, 0x36);

            // Paint solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            // Add subtle vignette for depth (darker edges)
            using (var vignette = new PathGradientBrush(path))
            {
                Color stateColor = BackgroundPainterHelpers.GetStateAdjustedColor(
                    baseColor, state, BackgroundPainterHelpers.StateIntensity.Subtle);
                vignette.CenterColor = Color.FromArgb(0, stateColor);
                vignette.SurroundColors = new[] { Color.FromArgb(40, 0, 0, 0) };
                g.FillPath(vignette, path);
            }
        }
    }
}
