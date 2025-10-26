using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Ubuntu border painter - warm orange outline inspired by Unity.
    /// </summary>
    public static class UbuntuBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float width = StyleBorders.GetBorderWidth(style);
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(233, 84, 32));

            if (isFocused || state == ControlState.Selected)
            {
                borderColor = BorderPainterHelpers.Lighten(borderColor, 0.1f);
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, width, state);
            return path.CreateInsetPath(width);
        }
    }
}
