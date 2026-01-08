using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.DesignTokens;
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
            ControlState state = ControlState.Normal,
            ElevationLevel elevation = ElevationLevel.Level2)
        {
            if (g == null || path == null) return;

            // Gradient Modern: use primary color as base
            Color primaryColor = useThemeColors && theme != null 
                ? theme.PrimaryColor 
                : StyleColors.GetBackground(BeepControlStyle.GradientModern);

            // Use ElevationSystem for state-aware depth
            ElevationLevel effectiveElevation = ElevationSystem.GetElevationForState(elevation, state);

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

            // Use multi-stop gradient for richer gradient (3-4 stops for better transitions)
            var stops = new[]
            {
                (0.0f, primaryColor), // Top - primary
                (0.3f, ColorAccessibilityHelper.DarkenColor(primaryColor, darkAmount * 0.5f)), // Upper middle
                (0.7f, ColorAccessibilityHelper.DarkenColor(primaryColor, darkAmount * 0.8f)), // Lower middle
                (1.0f, ColorAccessibilityHelper.DarkenColor(primaryColor, darkAmount))  // Bottom - darkest
            };

            // Apply disabled state alpha to stops
            if (state == ControlState.Disabled)
            {
                stops = stops.Select(s => (
                    s.Item1,
                    BackgroundPainterHelpers.WithAlpha(s.Item2, 100)
                )).ToArray();
            }

            // Use multi-stop gradient helper for richer visual effect
            BackgroundPainterHelpers.PaintMultiStopGradientBackground(g, path, stops, LinearGradientMode.Vertical, state, BackgroundPainterHelpers.StateIntensity.Normal);

            // Add radial gradient overlay for depth (combine linear + radial)
            if (state == ControlState.Hovered || state == ControlState.Focused)
            {
                Color centerColor = ColorAccessibilityHelper.LightenColor(primaryColor, 0.06f);
                Color edgeColor = primaryColor;
                BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, ControlState.Normal, BackgroundPainterHelpers.StateIntensity.Subtle);
            }
        }
    }
}
