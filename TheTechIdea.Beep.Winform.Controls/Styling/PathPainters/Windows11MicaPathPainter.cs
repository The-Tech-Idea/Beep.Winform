using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Windows 11 Mica path painter
    /// Uses subtle fills with mica material effect
    /// </summary>
    public static class Windows11MicaPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState .Normal)
        {
            Color fillColor = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(0, 120, 212));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                PathPainterHelpers.PaintSolidPath(g, path, fillColor, state);
            }
        }
    }
}

