using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Linear style path painter - Ultra-clean solid fills
    /// </summary>
    public static class LinearPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style,
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            Color baseColor = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary",
                Color.FromArgb(99, 102, 241)); // Linear indigo

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                PathPainterHelpers.PaintSolidPath(g, path, baseColor, state);
            }
        }
    }
}
