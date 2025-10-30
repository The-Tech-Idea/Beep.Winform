using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Figma card path painter
    /// Uses Figma's signature blue color
    /// </summary>
    public static class FigmaCardPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            Color fillColor = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(24, 160, 251));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                PathPainterHelpers.PaintSolidPath(g, path, fillColor, state);
            }
        }
    }
}

