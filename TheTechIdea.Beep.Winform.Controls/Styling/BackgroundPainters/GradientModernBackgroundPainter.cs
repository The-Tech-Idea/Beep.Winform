using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Gradient Modern background painter - smooth vertical gradient
    /// Primary color to darker shade with state-modulated intensity
    /// </summary>
    public static class GradientModernBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Gradient Modern: use primary color as base
            Color primaryColor = useThemeColors && theme != null 
                ? theme.PrimaryColor 
                : StyleColors.GetBackground(BeepControlStyle.GradientModern);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // State-modulated gradient intensity
            float darkAmount = state switch
            {
                ControlState.Hovered => 0.25f,   // Lighter gradient
                ControlState.Pressed => 0.40f,   // Darker gradient
                ControlState.Selected => 0.20f,  // Lighter gradient
                ControlState.Focused => 0.28f,   // Slightly lighter
                ControlState.Disabled => 0.15f,  // Very subtle gradient
                _ => 0.30f                        // Normal gradient
            };

            Color secondary = BackgroundPainterHelpers.Darken(primaryColor, darkAmount);
            
            // Apply disabled state alpha
            if (state == ControlState.Disabled)
            {
                primaryColor = BackgroundPainterHelpers.WithAlpha(primaryColor, 100);
                secondary = BackgroundPainterHelpers.WithAlpha(secondary, 100);
            }

            var gradientBrush = PaintersFactory.GetLinearGradientBrush(
                bounds, primaryColor, secondary, LinearGradientMode.Vertical);
            g.FillPath(gradientBrush, path);
        }
    }
}
