using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Neon shadow painter - Intense neon glow effect
    /// Bright cyan/colored glow for nightclub/synthwave aesthetic
    /// High intensity state-aware glow
    /// </summary>
    public static class NeonShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Neon glow color - cyan by default or theme accent
            Color glowColor = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetPrimary(style);
            
            int glowRadius = StyleShadows.GetShadowBlur(style);

            // State-based intensity - Neon is always bright
            float intensity = state switch
            {
                ControlState.Hovered => 1.2f,   // Brighter on hover
                ControlState.Pressed => 0.7f,   // Dimmer when pressed
                ControlState.Focused => 1.3f,   // Brightest on focus
                ControlState.Disabled => 0.3f,  // Very dim when disabled
                _ => 1.0f                       // Default bright
            };

            // Use neon glow helper
            return ShadowPainterHelpers.PaintNeonGlow(
                g, path, radius,
                glowColor, intensity, glowRadius);
        }
    }
}
