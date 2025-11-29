using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Arc Linux border painter - thin 1px outline with subtle highlight.
    /// </summary>
    public static class ArcLinuxBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color fallback = Color.FromArgb(96, 82, 92, 110);
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", fallback);
            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;

            if (isFocused || state == ControlState.Selected)
            {
                borderColor = BorderPainterHelpers.Lighten(borderColor, 0.15f);
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
            return path.CreateInsetPath(borderWidth);
        }
    }
}
