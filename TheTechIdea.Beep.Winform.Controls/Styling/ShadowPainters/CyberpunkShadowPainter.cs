using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Cyberpunk shadow painter - Cyan/magenta neon glow
    /// Night city aesthetic with intense neon lighting
    /// State-aware for interactive feedback
    /// </summary>
    public static class CyberpunkShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Cyberpunk glow - cyan or theme accent
            Color glowColor = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetPrimary(style);
            
            int glowRadius = StyleShadows.GetShadowBlur(style);

            // Cyberpunk: Intense neon state-based
            // Increased intensity for maximum neon effect
            float intensity = state switch
            {
                ControlState.Hovered => 1.6f,   // Brighter on hover
                ControlState.Pressed => 1.0f,   // Dimmer when pressed
                ControlState.Focused => 1.8f,   // Brightest on focus
                ControlState.Disabled => 0.4f, // Very dim when disabled
                _ => 1.4f                       // Default bright
            };

            // Use neon glow helper
            return ShadowPainterHelpers.PaintNeonGlow(
                g, path, radius,
                glowColor, intensity, glowRadius);
        }
    }
}
