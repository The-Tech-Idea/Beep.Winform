using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// FigmaCard border painter - Figma blue 1px border
    /// </summary>
    public static class FigmaCardBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            BorderPainterHelpers.ControlState state = BorderPainterHelpers.ControlState.Normal)
        {
            Color borderColor = isFocused
                ? BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(24, 160, 251))
                : BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(227, 227, 227));

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, 1f, state);
        }
    }
}

