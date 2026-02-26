using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Nordic border painter - light Scandinavian outline.
    /// </summary>
    public static class NordicBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float width = StyleBorders.GetBorderWidth(style);
            if (width <= 0f) return path;
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(180, 200, 212));

            if (isFocused)
            {
                borderColor = BorderPainterHelpers.Lighten(borderColor, 0.15f);
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, width, state);
            return BorderPainterHelpers.CreateStrokeInsetPath(path, width);
        }
    }
}
