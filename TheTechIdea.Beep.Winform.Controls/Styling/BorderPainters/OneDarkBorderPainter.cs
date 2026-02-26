using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// OneDark border painter - subdued editor outline.
    /// </summary>
    public static class OneDarkBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float width = StyleBorders.GetBorderWidth(style);
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(90, 97, 110));
            borderColor = BorderPainterHelpers.WithAlpha(borderColor, isFocused ? 200 : 150);
            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, width, state);
            return BorderPainterHelpers.CreateStrokeInsetPath(path, width);
        }
    }
}
