using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Cinnamon background painter - Linux Mint Cinnamon desktop
    /// Light gray with green accent (#73D216)
    /// 6px radius, generous spacing, Ubuntu font aesthetic
    /// </summary>
    public static class CinnamonBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color lightGray = useThemeColors && theme != null ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Cinnamon);
            Color mintGreen = useThemeColors && theme != null ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Cinnamon);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color fillColor = state switch
            {
                ControlState.Hovered => ColorUtils.Lighten(lightGray, 0.05f),
                ControlState.Pressed => ColorUtils.Darken(lightGray, 0.08f),
                ControlState.Selected => Color.FromArgb(30, mintGreen),
                ControlState.Disabled => ColorUtils.Lighten(lightGray, 0.1f),
                _ => lightGray
            };

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);
        }
    }
}
