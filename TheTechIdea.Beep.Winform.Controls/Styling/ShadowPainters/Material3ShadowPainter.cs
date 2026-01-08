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
    /// Material Design 3 shadow painter
    /// Enhanced with proper Material 3 elevation system and state-aware transitions
    /// Uses ElevationSystem for consistent depth perception
    /// </summary>
    public static class Material3ShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Convert MaterialElevation to ElevationLevel
            ElevationLevel baseElevation = ConvertToElevationLevel(elevation);

            // Adjust elevation based on state using ElevationSystem
            ElevationLevel effectiveElevation = ElevationSystem.GetElevationForState(baseElevation, state);

            if (effectiveElevation == ElevationLevel.Level0) return path;

            // Get shadow color from style/theme (use darker base color, not pure black)
            Color shadowColor = StyleShadows.GetShadowColor(style);
            if (useThemeColors && theme?.ShadowColor != null && theme.ShadowColor != Color.Empty)
            {
                shadowColor = theme.ShadowColor;
            }

            // Use enhanced dual-layer shadow with proper Material 3 elevation specs
            int elevationValue = (int)effectiveElevation;
            return ShadowPainterHelpers.PaintDualLayerShadow(
                g, path, radius,
                elevationValue,
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
