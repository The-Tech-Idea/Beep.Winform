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
    /// Gradient Modern shadow painter - Deep shadows for gradient backgrounds
    /// Rich, prominent shadows that complement gradient surfaces
    /// State and elevation aware
    /// </summary>
    public static class GradientModernShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Gradient Modern uses more prominent shadows - tint with gradient colors for cohesion
            Color shadowColor = StyleShadows.GetShadowColor(style);
            if (useThemeColors && theme?.ShadowColor != null && theme.ShadowColor != Color.Empty)
            {
                shadowColor = theme.ShadowColor;
            }
            else
            {
                // Use darker gray, could be tinted with gradient colors for better cohesion
                shadowColor = Color.FromArgb(25, 25, 35); // Slightly blue-tinted for gradient aesthetic
            }
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Use ElevationSystem for state-aware elevation
            ElevationLevel baseElevation = ConvertToElevationLevel(elevation);
            ElevationLevel effectiveElevation = ElevationSystem.GetElevationForState(baseElevation, state);
            
            // Get shadow parameters from ElevationSystem
            var (ambientAlpha, keyAlpha, elevationOffsetY, elevationSpread) = ElevationSystem.GetShadowParameters(effectiveElevation);
            
            // Base alpha higher for gradient backgrounds (use elevation system values)
            int baseAlpha = keyAlpha > 0 ? keyAlpha : ((int)elevation switch
            {
                0 => 35,
                1 => 45,
                2 => 55,
                3 => 65,
                4 => 75,
                _ => 85
            });

            // State-based adjustments
            int alpha = state switch
            {
                ControlState.Hovered => (int)(baseAlpha * 1.4f),
                ControlState.Pressed => (int)(baseAlpha * 0.6f),
                ControlState.Focused => (int)(baseAlpha * 1.3f),
                ControlState.Disabled => (int)(baseAlpha * 0.4f),
                _ => baseAlpha
            };

            int spread = elevationSpread > 0 ? elevationSpread : Math.Max(3, (int)elevation + 2);
            int effectiveOffsetY = elevationOffsetY > 0 ? elevationOffsetY : offsetY;

            // Use clean drop shadow (more prominent for gradients)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, effectiveOffsetY,
                shadowColor, alpha,
                spread);
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
                MaterialElevation.Level3 => ElevationLevel.Level4,
                MaterialElevation.Level4 => ElevationLevel.Level8,
                MaterialElevation.Level5 => ElevationLevel.Level12,
                _ => ElevationLevel.Level2
            };
        }
    }
}
