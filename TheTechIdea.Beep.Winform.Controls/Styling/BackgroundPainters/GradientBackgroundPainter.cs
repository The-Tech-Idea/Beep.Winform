using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Gradient background painter - vertical primary to secondary gradient
    /// Generic gradient painter with state awareness
    /// </summary>
    public static class GradientBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, 
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            Color primary = BackgroundPainterHelpers.GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            Color secondary = BackgroundPainterHelpers.GetColor(style, StyleColors.GetSecondary, "Secondary", theme, useThemeColors);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Use multi-stop gradient for richer visual effect (3 stops for smoother transitions)
            var stops = new[]
            {
                (0.0f, primary), // Top
                (0.5f, BackgroundPainterHelpers.BlendColors(primary, secondary, 0.5f)), // Middle
                (1.0f, secondary) // Bottom
            };

            // Apply state adjustment to stops
            var stateStops = stops.Select(s => (
                s.Item1,
                BackgroundPainterHelpers.GetStateAdjustedColor(
                    s.Item2, state, BackgroundPainterHelpers.StateIntensity.Normal)
            )).ToArray();

            // Use multi-stop gradient helper for richer visual effect
            BackgroundPainterHelpers.PaintMultiStopGradientBackground(g, path, stateStops, LinearGradientMode.Vertical, state, BackgroundPainterHelpers.StateIntensity.Normal);
        }
    }
}
