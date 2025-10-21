using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Gnome border painter - Adwaita design system
    /// 1px subtle borders with 6px radius (friendly rounded)
    /// Blue focus ring for keyboard navigation
    /// Warm gray border colors matching Adwaita palette
    /// </summary>
    public static class GnomeBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return path;

            // Gnome Adwaita colors
            Color borderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.Gnome);
            Color blueAccent = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Gnome);

            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Gnome); // 1.0f
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // State-based border color
            Color finalBorderColor = state switch
            {
                ControlState.Hovered => ColorUtils.Lighten(borderColor, 0.1f),
                ControlState.Pressed => ColorUtils.Darken(borderColor, 0.1f),
                ControlState.Selected => blueAccent,
                ControlState.Disabled => ColorUtils.Lighten(borderColor, 0.3f),
                _ => borderColor
            };

            // Paint 1px border
            BorderPainterHelpers.PaintSimpleBorder(g, path, finalBorderColor, borderWidth);

            // Gnome: 2px blue focus ring when focused (Adwaita keyboard navigation)
            if (isFocused || state == ControlState.Focused)
            {
                float ringWidth = StyleBorders.GetRingWidth(BeepControlStyle.Gnome); // 2.0f
                float ringOffset = StyleBorders.GetRingOffset(BeepControlStyle.Gnome); // 2.0f
                BorderPainterHelpers.PaintRing(g, path, blueAccent, ringWidth, ringOffset);
            }

            return path;
        }
    }
}
