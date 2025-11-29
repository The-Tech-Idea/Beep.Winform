using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    public static class ElementaryBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return path;

            Color borderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(style);
            Color subtleBlue = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(style);
            float borderWidth = StyleBorders.GetBorderWidth(style);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color finalColor = (state == ControlState.Selected || state == ControlState.Focused) ? subtleBlue : borderColor;
            BorderPainterHelpers.PaintSimpleBorder(g, path, finalColor, borderWidth);

            if (isFocused)
            {
                BorderPainterHelpers.PaintRing(g, path, subtleBlue, 2.0f, 2.0f);
            }

            return path;
        }
    }
}
