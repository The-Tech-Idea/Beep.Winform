using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Tokyo border painter - cyan neon frame.
    /// </summary>
    public static class TokyoBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float width = StyleBorders.GetBorderWidth(style);
            if (width <= 0f) return path;
            Color neon = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(122, 162, 247));
            neon = BorderPainterHelpers.WithAlpha(neon, isFocused || state == ControlState.Selected ? 220 : 160);

            BorderPainterHelpers.PaintGlowBorder(g, path, BorderPainterHelpers.WithAlpha(neon, 120), 6f);
            BorderPainterHelpers.PaintSimpleBorder(g, path, neon, width, state);
            return path.CreateInsetPath(width + 2f);
        }
    }
}
