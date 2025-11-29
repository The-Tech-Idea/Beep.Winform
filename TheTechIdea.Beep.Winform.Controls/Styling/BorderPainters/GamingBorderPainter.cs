using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    public static class GamingBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return path;

            Color borderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(style);
            Color neonGreen = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(style);
            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;

            g.SmoothingMode = SmoothingMode.None; // Angular

            Color finalColor = (state == ControlState.Selected || state == ControlState.Hovered) ? neonGreen : borderColor;
            BorderPainterHelpers.PaintSimpleBorder(g, path, finalColor, borderWidth);

            if (isFocused || state == ControlState.Focused)
            {
                BorderPainterHelpers.PaintGlowBorder(g, path, neonGreen, 4.0f, 0.8f);
            }

            return path;
        }
    }
}
