using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.DesignTokens;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Material You shadow painter
    /// Material 3 elevation with dynamic color adaptation
    /// Shadows can be tinted by surface color for cohesive feel
    /// </summary>
    public static class MaterialYouShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Convert MaterialElevation to ElevationLevel and use ElevationSystem for state-aware elevation
            ElevationLevel baseElevation = ConvertToElevationLevel(elevation);
            ElevationLevel effectiveElevation = ElevationSystem.GetElevationForState(baseElevation, state);

            if (effectiveElevation == ElevationLevel.Level0) return path;
            
            int effectiveLevel = (int)effectiveElevation;

            // Material You: Dynamic shadow color from theme
            Color shadowColor = StyleShadows.GetShadowColor(style);
            
            // Adapt shadow color for Material You dynamic theming
            if (useThemeColors && theme != null)
            {
                // Material You can tint shadows slightly with surface color
                Color surfaceColor = theme.SurfaceColor;
                if (surfaceColor != Color.Empty)
                {
                    // Blend a hint of surface color into shadow for cohesion
                    int r = (int)(shadowColor.R * 0.85 + surfaceColor.R * 0.15);
                    int gr = (int)(shadowColor.G * 0.85 + surfaceColor.G * 0.15);
                    int b = (int)(shadowColor.B * 0.85 + surfaceColor.B * 0.15);
                    shadowColor = Color.FromArgb(r, gr, b);
                }
            }

            // Use enhanced dual-layer shadow for proper Material elevation
            return ShadowPainterHelpers.PaintDualLayerShadow(
                g, path, radius,
                effectiveLevel,
                shadowColor);
        }

        /// <summary>
        /// Convert MaterialElevation enum to ElevationLevel
        /// </summary>
        private static ElevationLevel ConvertToElevationLevel(MaterialElevation materialElevation)
        {
            return materialElevation switch
            {
                MaterialElevation.Level0 => ElevationLevel.Level0,
                MaterialElevation.Level1 => ElevationLevel.Level1,
                MaterialElevation.Level2 => ElevationLevel.Level2,
                MaterialElevation.Level3 => ElevationLevel.Level4,  // Level3 maps to 4dp
                MaterialElevation.Level4 => ElevationLevel.Level8,  // Level4 maps to 8dp
                MaterialElevation.Level5 => ElevationLevel.Level12, // Level5 maps to 12dp
                _ => ElevationLevel.Level2
            };
        }
    }
}
