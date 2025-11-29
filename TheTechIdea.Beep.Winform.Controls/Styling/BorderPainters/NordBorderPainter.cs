using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Nord border painter - cool frosted outline.
    /// </summary>
    public static class NordBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float width = StyleBorders.GetBorderWidth(style);
            if (width <= 0f) return path;
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(136, 192, 208));
            borderColor = BorderPainterHelpers.WithAlpha(borderColor, isFocused ? 220 : 160);

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, width, state);
            return path.CreateInsetPath(width);
        }
    }
}
