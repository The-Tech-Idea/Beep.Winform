using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Gradient modern path painter with glassmorphism
    /// Uses vibrant gradient fills from indigo to purple
    /// </summary>
    public static class GradientModernPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, PathPainterHelpers.ControlState state = PathPainterHelpers.ControlState.Normal)
        {
            Color color1 = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(99, 102, 241));
            Color color2 = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Secondary", Color.FromArgb(139, 92, 246));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                PathPainterHelpers.PaintGradientPath(g, path, color1, color2, 135f, state);
            }
        }
    }
}

