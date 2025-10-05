using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// NotionMinimal border painter - Very subtle 1px border
    /// </summary>
    public static class NotionMinimalBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            BorderPainterHelpers.ControlState state = BorderPainterHelpers.ControlState.Normal)
        {
            Color borderColor = isFocused
                ? BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(55, 53, 47))
                : BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(227, 226, 224));

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, 1f, state);
        }
    }
}

