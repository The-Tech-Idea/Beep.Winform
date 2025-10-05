using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Material3 border painter - Outlined style with 1px border
    /// Filled variants have no border
    /// </summary>
    public static class Material3BorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            BorderPainterHelpers.ControlState state = BorderPainterHelpers.ControlState.Normal)
        {
            // Material3 filled variants don't have borders
            // Only outlined variants have borders
            Color borderColor = isFocused
                ? BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(103, 80, 164))
                : BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(121, 116, 126));

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, 1f, state);
        }
    }
}

