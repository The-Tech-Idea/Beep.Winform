using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Cartoon border painter - bold comic outline with rounded corners.
    /// </summary>
    public static class CartoonBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float borderWidth = StyleBorders.GetBorderWidth(style);
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(60, 40, 30));

            if (isFocused || state == ControlState.Selected)
            {
                borderColor = BorderPainterHelpers.Darken(borderColor, 0.15f);
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, ControlState.Normal);
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}
