using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Solarized border painter - simple outline using base tones.
    /// </summary>
    public static class SolarizedBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float width = StyleBorders.GetBorderWidth(style);
            if (width <= 0f) return path;
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(0, 43, 54));
            borderColor = BorderPainterHelpers.WithAlpha(borderColor, isFocused ? 220 : 180);
            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, width, state);
            return BorderPainterHelpers.CreateStrokeInsetPath(path, width);
        }
    }
}
