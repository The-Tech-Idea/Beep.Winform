using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Cinnamon border painter - Linux Mint Cinnamon desktop
    /// 1px borders, 6px radius, green accent (#73D216) on selection
    /// </summary>
    public static class CinnamonBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return path;

            Color borderColor = useThemeColors && theme != null ? theme.BorderColor : StyleColors.GetBorder(style);
            Color mintGreen = useThemeColors && theme != null ? theme.AccentColor : StyleColors.GetPrimary(style);
            float borderWidth = StyleBorders.GetBorderWidth(style);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color finalBorderColor = state switch
            {
                ControlState.Selected => mintGreen,
                ControlState.Focused => mintGreen,
                _ => borderColor
            };

            BorderPainterHelpers.PaintSimpleBorder(g, path, finalBorderColor, borderWidth);

            if (isFocused || state == ControlState.Focused)
            {
                float ringWidth = StyleBorders.GetRingWidth(style);
                float ringOffset = StyleBorders.GetRingOffset(style);
                BorderPainterHelpers.PaintRing(g, path, mintGreen, ringWidth, ringOffset);
            }

            return path;
        }
    }
}
