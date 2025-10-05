using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Neumorphism embossed path painter
    /// Uses subtle gradient fills to create soft embossed effect
    /// </summary>
    public static class NeumorphismPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, PathPainterHelpers.ControlState state = PathPainterHelpers.ControlState.Normal)
        {
            Color baseColor = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Background", Color.FromArgb(225, 225, 230));
            
            // Create subtle gradient for embossed effect
            Color lightColor = PathPainterHelpers.Lighten(baseColor, 0.05f);
            Color darkColor = PathPainterHelpers.Darken(baseColor, 0.05f);

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                PathPainterHelpers.PaintGradientPath(g, path, lightColor, darkColor, 135f, state);
            }
        }
    }
}

