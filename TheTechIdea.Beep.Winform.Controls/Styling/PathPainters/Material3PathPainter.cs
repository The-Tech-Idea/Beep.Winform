using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.DesignTokens;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Material Design 3 path painter with tonal elevation.
    /// Uses the primary color and creates a subtle vertical gradient to mimic M3 surfaces.
    /// </summary>
    public static class Material3PathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal, ElevationLevel elevation = ElevationLevel.Level2)
        {
            // Base tonal color from theme or M3 default primary
            Color baseColor = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Primary",
                Color.FromArgb(103, 80, 164));

            // Get primary color for state layers (mirror Material3BackgroundPainter)
            Color primaryColor = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Primary",
                Color.FromArgb(103, 80, 164));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Material 3 tonal elevation uses multi-stop gradients for better tonal transitions
                // Create multi-stop gradient for better tonal elevation (Material 3 uses tonal colors)
                var stops = new[]
                {
                    (0.0f, ColorAccessibilityHelper.LightenColor(baseColor, 0.06f)), // Top - lighter
                    (0.3f, ColorAccessibilityHelper.LightenColor(baseColor, 0.03f)), // Upper middle
                    (0.7f, ColorAccessibilityHelper.DarkenColor(baseColor, 0.02f)), // Lower middle
                    (1.0f, ColorAccessibilityHelper.DarkenColor(baseColor, 0.05f))  // Bottom - darker
                };

                // Apply state adjustments to stops
                var stateStops = stops.Select(s => (
                    s.Item1,
                    PathPainterHelpers.ApplyState(s.Item2, state)
                )).ToArray();

                BackgroundPainterHelpers.PaintMultiStopGradientBackground(g, path, stateStops, LinearGradientMode.Vertical, state, BackgroundPainterHelpers.StateIntensity.Normal);

                // Material3 elevation: white tonal overlay based on elevation level (mirror BackgroundPainter)
                int elevationAlpha = GetElevationOverlayAlpha(elevation);
                if (elevationAlpha > 0)
                {
                    var elevationBrush = TheTechIdea.Beep.Winform.Controls.Styling.PaintersFactory.GetSolidBrush(Color.FromArgb(elevationAlpha, Color.White));
                    g.FillPath(elevationBrush, path);
                }

                // Material3 state layer overlay (for interactive states) - mirror BackgroundPainter
                PaintStateLayerOverlay(g, path, primaryColor, state);

                // Add subtle radial highlight for button states (Material 3 buttons have radial highlights)
                if (state == ControlState.Hovered || state == ControlState.Focused)
                {
                    Color centerColor = ColorAccessibilityHelper.LightenColor(baseColor, 0.08f);
                    Color edgeColor = baseColor;
                    BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, ControlState.Normal, BackgroundPainterHelpers.StateIntensity.Subtle);
                }
            }
        }

        /// <summary>
        /// Get elevation overlay alpha based on Material 3 elevation system
        /// </summary>
        private static int GetElevationOverlayAlpha(ElevationLevel elevation)
        {
            return (int)elevation switch
            {
                0 => 0,
                1 => 5,
                2 => 10,
                4 => 15,
                8 => 20,
                12 => 25,
                16 => 30,
                24 => 35,
                _ => 10
            };
        }

        /// <summary>
        /// Paint Material 3 state layer overlay
        /// </summary>
        private static void PaintStateLayerOverlay(Graphics g, GraphicsPath path, Color primaryColor, ControlState state)
        {
            int overlayAlpha = state switch
            {
                ControlState.Hovered => 8,
                ControlState.Pressed => 12,
                ControlState.Selected => 12,
                ControlState.Focused => 12,
                _ => 0
            };

            if (overlayAlpha > 0)
            {
                var overlayBrush = TheTechIdea.Beep.Winform.Controls.Styling.PaintersFactory.GetSolidBrush(Color.FromArgb(overlayAlpha, primaryColor));
                g.FillPath(overlayBrush, path);
            }
        }
    }
}

