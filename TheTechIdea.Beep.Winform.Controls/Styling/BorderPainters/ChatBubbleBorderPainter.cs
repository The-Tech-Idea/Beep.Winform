using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Chat bubble border painter - soft rounded outline for messaging styles.
    /// </summary>
    public static class ChatBubbleBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(120, 120, 160));

            if (isFocused || state == ControlState.Selected)
            {
                borderColor = BorderPainterHelpers.Lighten(borderColor, 0.25f);
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}
