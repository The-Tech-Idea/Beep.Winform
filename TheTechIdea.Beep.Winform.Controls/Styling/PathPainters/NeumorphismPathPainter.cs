using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Neumorphism embossed path painter
    /// Uses subtle gradient fills to create soft embossed effect
    /// </summary>
    public static class NeumorphismPathPainter
    {
        // Converted to use PathPainterHelpers/PaintersFactory
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            Color baseColor = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Background", Color.FromArgb(225, 225, 230));
            
            // Create subtle gradient for embossed effect - use multi-stop for smoother neumorphic effect
            // Use HSL-based color manipulation for more natural neumorphic effect
            var stops = new[]
            {
                (0.0f, ColorAccessibilityHelper.LightenColor(baseColor, 0.06f)), // Top-left - lightest
                (0.3f, ColorAccessibilityHelper.LightenColor(baseColor, 0.03f)), // Upper
                (0.5f, baseColor), // Center - base
                (0.7f, ColorAccessibilityHelper.DarkenColor(baseColor, 0.03f)), // Lower
                (1.0f, ColorAccessibilityHelper.DarkenColor(baseColor, 0.06f))  // Bottom-right - darkest
            };

            // Apply state adjustments to stops
            var stateStops = stops.Select(s => (
                s.Item1,
                PathPainterHelpers.ApplyState(s.Item2, state)
            )).ToArray();

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Use multi-stop gradient at 135 degrees for neumorphic embossed effect
                BackgroundPainterHelpers.PaintMultiStopGradientBackground(g, path, stateStops, LinearGradientMode.Vertical, state, BackgroundPainterHelpers.StateIntensity.Subtle);
            }
        }
    }
}

