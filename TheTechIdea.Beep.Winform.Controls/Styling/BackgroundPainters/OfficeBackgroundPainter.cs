using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Office background painter - Microsoft Office Ribbon UI style
    /// Clean white backgrounds with subtle professional gradients
    /// </summary>
    public static class OfficeBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Office: clean white background
            Color backgroundColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Office);
            Color primaryColor = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetPrimary(BeepControlStyle.Office);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Get Office-style colors for state
            var (topColor, bottomColor) = GetOfficeStateColors(backgroundColor, primaryColor, state);

            // Office ribbon-style vertical gradient
            var brush = PaintersFactory.GetLinearGradientBrush(
                bounds, topColor, bottomColor, LinearGradientMode.Vertical);
            g.FillPath(brush, path);
        }

        private static (Color top, Color bottom) GetOfficeStateColors(Color baseColor, Color primaryColor, ControlState state)
        {
            return state switch
            {
                // Hover: very light blue tint
                ControlState.Hovered => (
                    Color.FromArgb(250, 251, 253),
                    Color.FromArgb(245, 248, 252)),
                
                // Pressed: stronger blue tint
                ControlState.Pressed => (
                    Color.FromArgb(210, 230, 250),
                    Color.FromArgb(190, 220, 245)),
                
                // Selected: subtle primary color tint
                ControlState.Selected => (
                    BackgroundPainterHelpers.BlendColors(baseColor, primaryColor, 0.15f),
                    BackgroundPainterHelpers.BlendColors(baseColor, primaryColor, 0.20f)),
                
                // Focused: very subtle blue tint
                ControlState.Focused => (
                    Color.FromArgb(252, 253, 254),
                    Color.FromArgb(248, 250, 252)),
                
                // Disabled: gray gradient
                ControlState.Disabled => (
                    Color.FromArgb(240, 240, 240),
                    Color.FromArgb(230, 230, 230)),
                
                // Normal: pure clean white
                _ => (baseColor, baseColor)
            };
        }
    }
}
