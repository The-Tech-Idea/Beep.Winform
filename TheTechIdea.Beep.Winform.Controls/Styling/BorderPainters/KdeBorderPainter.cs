using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// KDE border painter - Breeze design system
    /// 1px clean borders with 4px radius
    /// Blue glow on focus/hover (KDE signature effect)
    /// Breeze blue (#3DAEE9) accent color
    /// </summary>
    public static class KdeBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return path;

            // KDE Breeze colors
            Color borderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.Kde);
            Color breezeBlue = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Kde);

            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Kde); // 1.0f
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // State-based border color
            Color finalBorderColor = state switch
            {
                ControlState.Hovered => breezeBlue, // Blue on hover (Breeze signature)
                ControlState.Pressed => ColorUtils.Darken(breezeBlue, 0.1f),
                ControlState.Selected => breezeBlue,
                ControlState.Disabled => ColorUtils.Lighten(borderColor, 0.3f),
                _ => borderColor
            };

            // Paint 1px border
            BorderPainterHelpers.PaintSimpleBorder(g, path, finalBorderColor, borderWidth);

            // KDE signature: Blue glow on focus (Breeze effect - 3px wide glow)
            if (isFocused || state == ControlState.Focused)
            {
                float glowWidth = StyleBorders.GetGlowWidth(BeepControlStyle.Kde); // 3.0f
                BorderPainterHelpers.PaintGlowBorder(g, path, breezeBlue, glowWidth, 0.4f);
            }

            return path;
        }
    }
}
