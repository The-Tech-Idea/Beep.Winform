using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Cinnamon background painter - Linux Mint desktop
    /// Light gray backgrounds with signature mint green accent
    /// Friendly, approachable design language
    /// </summary>
    public static class CinnamonBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Cinnamon: light gray
            Color lightGray = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : StyleColors.GetBackground(BeepControlStyle.Cinnamon);
            Color mintGreen = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetPrimary(BeepControlStyle.Cinnamon);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Cinnamon state handling
            Color fillColor = state switch
            {
                ControlState.Hovered => BackgroundPainterHelpers.Lighten(lightGray, 0.05f),
                ControlState.Pressed => BackgroundPainterHelpers.Darken(lightGray, 0.08f),
                ControlState.Selected => BackgroundPainterHelpers.BlendColors(lightGray, mintGreen, 0.15f),
                ControlState.Focused => BackgroundPainterHelpers.Lighten(lightGray, 0.03f),
                ControlState.Disabled => BackgroundPainterHelpers.WithAlpha(lightGray, 100),
                _ => lightGray
            };

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);
        }
    }
}
