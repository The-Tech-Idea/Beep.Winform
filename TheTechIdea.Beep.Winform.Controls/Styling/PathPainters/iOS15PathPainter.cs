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
    /// iOS 15 clean minimal path painter.
    /// Uses system accent blue with a soft translucent gradient to mimic iOS filled controls.
    /// </summary>
    public static class iOS15PathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            // iOS accent blue
            Color accent = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Primary",
                Color.FromArgb(0, 122, 255));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // iOS gradients often have multiple stops - use multi-stop for authentic iOS look
                // Translucent, slightly lighter towards the top with multiple intermediate stops
                var stops = new[]
                {
                    (0.0f, PathPainterHelpers.WithAlpha(ColorAccessibilityHelper.LightenColor(accent, 0.18f), 175)), // Top - lightest
                    (0.2f, PathPainterHelpers.WithAlpha(ColorAccessibilityHelper.LightenColor(accent, 0.12f), 180)), // Upper
                    (0.5f, PathPainterHelpers.WithAlpha(accent, 190)), // Middle
                    (0.8f, PathPainterHelpers.WithAlpha(ColorAccessibilityHelper.DarkenColor(accent, 0.03f), 195)), // Lower
                    (1.0f, PathPainterHelpers.WithAlpha(ColorAccessibilityHelper.DarkenColor(accent, 0.06f), 200))  // Bottom - darkest
                };

                // Apply state adjustments to stops
                var stateStops = stops.Select(s => (
                    s.Item1,
                    PathPainterHelpers.ApplyState(s.Item2, state)
                )).ToArray();

                BackgroundPainterHelpers.PaintMultiStopGradientBackground(g, path, stateStops, LinearGradientMode.Vertical, state, BackgroundPainterHelpers.StateIntensity.Normal);

                // Add radial highlight for iOS button appearance (iOS buttons have radial highlights)
                Color centerColor = PathPainterHelpers.WithAlpha(ColorAccessibilityHelper.LightenColor(accent, 0.2f), 200);
                Color edgeColor = PathPainterHelpers.WithAlpha(accent, 180);
                BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, state, BackgroundPainterHelpers.StateIntensity.Subtle);
            }
        }
    }
}

