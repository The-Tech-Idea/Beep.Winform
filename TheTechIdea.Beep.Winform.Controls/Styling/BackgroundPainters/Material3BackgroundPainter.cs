using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.DesignTokens;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material3 background painter - Google's Material Design 3 / Material You
    /// Enhanced with proper tonal elevation system and state layer overlays
    /// </summary>
    public static class Material3BackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal,
            ElevationLevel elevation = ElevationLevel.Level2)
        {
            if (g == null || path == null) return;

            // Material3: tonal surface color
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Material3);

            // Get primary color for state layers
            Color primaryColor = useThemeColors && theme != null
                ? theme.PrimaryColor
                : StyleColors.GetPrimary(BeepControlStyle.Material3);

            // Use ElevationSystem for state-aware elevation
            ElevationLevel effectiveElevation = ElevationSystem.GetElevationForState(elevation, state);

            // Material 3 uses subtle multi-stop tonal gradients for better tonal elevation
            var bounds = path.GetBounds();
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                // Create subtle multi-stop gradient for tonal elevation (Material 3 uses tonal colors)
                var stops = new[]
                {
                    (0.0f, ColorAccessibilityHelper.LightenColor(baseColor, 0.02f)), // Top - slightly lighter
                    (0.5f, baseColor), // Middle - base
                    (1.0f, ColorAccessibilityHelper.DarkenColor(baseColor, 0.01f))  // Bottom - slightly darker
                };

                // Apply state adjustments to stops
                var stateStops = stops.Select(s => (
                    s.Item1,
                    BackgroundPainterHelpers.GetStateAdjustedColor(
                        s.Item2, state, BackgroundPainterHelpers.StateIntensity.Strong)
                )).ToArray();

                // Use multi-stop gradient for subtle tonal elevation
                BackgroundPainterHelpers.PaintMultiStopGradientBackground(g, path, stateStops, LinearGradientMode.Vertical, state, BackgroundPainterHelpers.StateIntensity.Strong);
            }
            else
            {
                // Fallback to solid background if bounds are invalid
                BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                    BackgroundPainterHelpers.StateIntensity.Strong);
            }

            // Material3 elevation: white tonal overlay based on elevation level
            int elevationAlpha = GetElevationOverlayAlpha(effectiveElevation);
            if (elevationAlpha > 0)
            {
                var elevationBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(elevationAlpha, Color.White));
                g.FillPath(elevationBrush, path);
            }

            // Material3 state layer overlay (for interactive states)
            PaintStateLayerOverlay(g, path, primaryColor, state);

            // Material 3 buttons often have radial highlights - add for button states
            if (state == ControlState.Hovered || state == ControlState.Focused)
            {
                Color centerColor = ColorAccessibilityHelper.LightenColor(baseColor, 0.05f);
                Color edgeColor = baseColor;
                BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, ControlState.Normal, BackgroundPainterHelpers.StateIntensity.Subtle);
            }
        }

        /// <summary>
        /// Get elevation overlay alpha based on Material 3 elevation system
        /// Use ElevationSystem for consistency with shadow parameters
        /// </summary>
        private static int GetElevationOverlayAlpha(ElevationLevel elevation)
        {
            // Use ElevationSystem for consistency - get shadow parameters and derive overlay alpha
            var (ambientAlpha, keyAlpha, offsetY, spread) = ElevationSystem.GetShadowParameters(elevation);
            
            // Material 3 elevation overlay alpha is proportional to elevation level
            // Use a formula that matches Material 3 specs
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
                var overlayBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(overlayAlpha, primaryColor));
                g.FillPath(overlayBrush, path);
            }
        }
    }
}
