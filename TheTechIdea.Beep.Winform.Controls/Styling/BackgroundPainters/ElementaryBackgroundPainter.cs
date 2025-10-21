using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Elementary background painter - elementary OS Pantheon desktop
    /// Very subtle, clean white, 8px generous radius, Open Sans font aesthetic
    /// </summary>
    public static class ElementaryBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color cleanWhite = useThemeColors ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Elementary);
            Color subtleBlue = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Elementary);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color fillColor = state switch
            {
                ControlState.Hovered => ColorUtils.Darken(cleanWhite, 0.03f),
                ControlState.Pressed => ColorUtils.Darken(cleanWhite, 0.06f),
                ControlState.Selected => Color.FromArgb(20, subtleBlue),
                ControlState.Disabled => ColorUtils.Lighten(cleanWhite, 0.02f),
                _ => cleanWhite
            };

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
