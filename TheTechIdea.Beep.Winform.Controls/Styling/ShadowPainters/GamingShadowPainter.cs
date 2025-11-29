using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Gaming shadow painter - RGB-style intense glow
    /// Neon green or accent color glow for gaming/esports UIs
    /// Angular, intense, state-aware
    /// </summary>
    public static class GamingShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Gaming glow - neon green or theme accent
            Color glowColor = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetPrimary(style);
            
            int glowRadius = StyleShadows.GetShadowBlur(style);

            // Gaming: Intense state-based glow
            float intensity = state switch
            {
                ControlState.Hovered => 1.1f,   // Brighter on hover
                ControlState.Pressed => 0.8f,   // Dimmer when pressed
                ControlState.Focused => 1.2f,   // Bright on focus
                ControlState.Selected => 1.0f,  // Normal when selected
                ControlState.Disabled => 0.2f,  // Very dim when disabled
                _ => 0.9f                       // Default
            };

            // Use neon glow helper (gaming uses same technique)
            return ShadowPainterHelpers.PaintNeonGlow(
                g, path, radius,
                glowColor, intensity, glowRadius);
        }
    }
}
