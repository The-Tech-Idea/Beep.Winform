using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Material Design 3 path painter with tonal elevation.
    /// Uses the primary color and creates a subtle vertical gradient to mimic M3 surfaces.
    /// </summary>
    public static class Material3PathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            // Base tonal color from theme or M3 default primary
            Color baseColor = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Primary",
                Color.FromArgb(103, 80, 164));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Create a subtle elevation effect – slightly lighter at the top, darker at the bottom
                Color top = PathPainterHelpers.Lighten(baseColor, 0.04f);
                Color bottom = PathPainterHelpers.Darken(baseColor, 0.04f);

                // Apply state adjustments (hover/pressed/disabled) on top of tonal colors
                PathPainterHelpers.PaintGradientPath(g, path, top, bottom, LinearGradientMode.Vertical, state);
            }
        }
    }
}

