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
    /// Microsoft Fluent 2 shadow painter
    /// Modern soft shadows with subtle state feedback
    /// Refined elevation for contemporary Windows design
    /// </summary>
    public static class Fluent2ShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fluent 2 shadow - use darker theme color instead of pure black for realism
            Color shadowColor = StyleShadows.GetShadowColor(style);
            if (useThemeColors && theme?.ShadowColor != null && theme.ShadowColor != Color.Empty)
            {
                shadowColor = theme.ShadowColor;
            }
            else
            {
                // Use darker gray instead of pure black for more realistic shadows
                shadowColor = Color.FromArgb(30, 30, 30);
            }
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Use ElevationSystem for state-aware elevation transitions
            ElevationLevel baseElevation = ConvertToElevationLevel(elevation);
            ElevationLevel effectiveElevation = ElevationSystem.GetElevationForState(baseElevation, state);
            
            // Get shadow parameters from ElevationSystem
            var (ambientAlpha, keyAlpha, elevationOffsetY, elevationSpread) = ElevationSystem.GetShadowParameters(effectiveElevation);
            
            // Fluent 2 state-based shadow intensity (combine with elevation system)
            int baseAlpha = ambientAlpha > 0 ? ambientAlpha : 40;
            int alpha = state switch
            {
                ControlState.Hovered => (int)(baseAlpha * 1.4f),    // More prominent
                ControlState.Pressed => (int)(baseAlpha * 0.6f),    // Reduced
                ControlState.Focused => (int)(baseAlpha * 1.25f),   // Moderate
                ControlState.Selected => (int)(baseAlpha * 1.5f),   // Most visible
                ControlState.Disabled => (int)(baseAlpha * 0.3f),   // Minimal
                _ => baseAlpha                                      // Default modern subtle
            };

            int spread = elevationSpread > 0 ? elevationSpread : 2;
            int effectiveOffsetY = elevationOffsetY > 0 ? elevationOffsetY : offsetY;

            // Use clean drop shadow (Fluent 2 modern style)
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
