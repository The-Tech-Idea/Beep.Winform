using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Tokyo shadow painter - Tokyo Night theme glow
    /// Night city aesthetic with subtle colored glow
    /// Based on popular Tokyo Night color scheme
    /// </summary>
    public static class TokyoShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Tokyo Night glow color
            Color glowColor = StyleShadows.GetShadowColor(style);
            int glowRadius = StyleShadows.GetShadowBlur(style);

            // State-aware - Tokyo Night is more subtle than Cyberpunk
            float intensity = state switch
            {
                ControlState.Hovered => 1.0f,   // Brighter on hover
                ControlState.Pressed => 0.5f,   // Dimmer when pressed
                ControlState.Focused => 1.1f,   // Bright on focus
                ControlState.Disabled => 0.25f, // Dim when disabled
                _ => 0.8f                       // Default subtle
            };

            // Use neon glow helper
            return ShadowPainterHelpers.PaintNeonGlow(
                g, path, radius,
                glowColor, intensity, glowRadius);
        }
    }
}
