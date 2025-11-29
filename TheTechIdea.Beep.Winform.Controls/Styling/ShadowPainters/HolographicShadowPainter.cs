using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Holographic shadow painter - Prismatic/rainbow glow effect
    /// Creates iridescent glow for futuristic UIs
    /// State-aware with intensity variations
    /// </summary>
    public static class HolographicShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Holographic uses style shadow color (typically iridescent/purple)
            Color glowColor = StyleShadows.GetShadowColor(style);
            int glowRadius = StyleShadows.GetShadowBlur(style);

            // State-aware intensity
            float intensity = state switch
            {
                ControlState.Hovered => 1.1f,   // Brighter on hover
                ControlState.Pressed => 0.6f,   // Dimmer when pressed
                ControlState.Focused => 1.2f,   // Bright on focus
                ControlState.Disabled => 0.3f,  // Dim when disabled
                _ => 0.9f                       // Default
            };

            // Use neon glow helper
            return ShadowPainterHelpers.PaintNeonGlow(
                g, path, radius,
                glowColor, intensity, glowRadius);
        }
    }
}
