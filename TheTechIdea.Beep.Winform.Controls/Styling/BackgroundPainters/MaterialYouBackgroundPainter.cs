using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ColorSystems;
using TheTechIdea.Beep.Winform.Controls.Styling.DesignTokens;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material You background painter - personalized dynamic color system
    /// Tonal surface with primary color tinting for personalized feel
    /// </summary>
    public static class MaterialYouBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal,
            ElevationLevel elevation = ElevationLevel.Level2)
        {
            if (g == null || path == null) return;

            // Get base color
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.MaterialYou);
            Color primaryColor = useThemeColors && theme != null 
                ? theme.PrimaryColor 
                : StyleColors.GetPrimary(BeepControlStyle.MaterialYou);

            // Use MaterialYouColorSystem for dynamic palette generation
            bool isDarkMode = theme?.IsDarkMode ?? false;
            var palette = MaterialYouColorSystem.GenerateMaterialYouPalette(primaryColor, isDarkMode, ensureAccessibility: true);

            // Use ElevationSystem for state-aware elevation
            ElevationLevel effectiveElevation = ElevationSystem.GetElevationForState(elevation, state);

            // Apply tonal colors based on state (Material You uses tonal colors)
            Color fillColor = state switch
            {
                ControlState.Hovered => palette.Tonal300,
                ControlState.Pressed => palette.Tonal700,
                ControlState.Focused => palette.Tonal400,
                ControlState.Selected => palette.Tonal400,
                ControlState.Disabled => palette.Tonal100,
                _ => palette.Surface // Use surface color for background
            };

            // Ensure accessibility compliance
            if (useThemeColors && theme != null && theme.ForeColor != Color.Empty)
            {
                fillColor = ColorAccessibilityHelper.EnsureContrastRatio(
                    fillColor, theme.ForeColor, 
                    ColorAccessibilityHelper.WCAG_AA_Normal);
            }

            // Material You uses multi-stop gradients for tonal variations
            var bounds = path.GetBounds();
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                // Create multi-stop gradient for tonal variations (Material You uses tonal colors)
                var stops = new[]
                {
                    (0.0f, fillColor), // Top
                    (0.3f, ColorAccessibilityHelper.LightenColor(fillColor, 0.015f)), // Upper
                    (0.7f, fillColor), // Middle
                    (1.0f, ColorAccessibilityHelper.DarkenColor(fillColor, 0.01f))  // Bottom
                };

                // Use multi-stop gradient helper for tonal variations
                BackgroundPainterHelpers.PaintMultiStopGradientBackground(g, path, stops, LinearGradientMode.Vertical, state, BackgroundPainterHelpers.StateIntensity.Normal);
            }
            else
            {
                // Fallback to solid if bounds are invalid
                var brush = PaintersFactory.GetSolidBrush(fillColor);
                g.FillPath(brush, path);
            }

            // Material You elevation: white tonal overlay based on elevation level (similar to Material3)
            var (ambientAlpha, keyAlpha, offsetY, spread) = ElevationSystem.GetShadowParameters(effectiveElevation);
            int elevationAlpha = GetElevationOverlayAlpha(effectiveElevation);
            if (elevationAlpha > 0)
            {
                var elevationBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(elevationAlpha, Color.White));
                g.FillPath(elevationBrush, path);
            }

            // Material You signature: subtle tonal primary overlay for personal tinting
            var tonalBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(15, palette.Primary));
            g.FillPath(tonalBrush, path);
        }

        /// <summary>
        /// Get elevation overlay alpha based on Material You elevation system
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
    }
}
