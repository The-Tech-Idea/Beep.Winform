using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;


namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Minimal border painter - Simple 1px border, always visible
    /// </summary>
    public static class MinimalBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            BorderPainterHelpers.ControlState state = BorderPainterHelpers.ControlState.Normal)
        {
            Color borderColor = isFocused
                ? BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(64, 64, 64))
                : BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(224, 224, 224));

            float borderWidth = (Color.FromArgb(64, 64, 64) == Color.Transparent    ) ? 0f : 1f;
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
        }
    }
}
