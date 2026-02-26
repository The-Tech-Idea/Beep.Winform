using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// GruvBox border painter - warm-toned outline.
    /// </summary>
    public static class GruvBoxBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(251, 184, 108));
            borderColor = BorderPainterHelpers.WithAlpha(borderColor, isFocused || state == ControlState.Selected ? 200 : 140);
            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}
