using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Dark Glow shadow painter - Colored glow on dark backgrounds
    /// Purple/accent colored glow instead of traditional shadow
    /// Creates vibrant depth on dark UIs
    /// </summary>
    public static class DarkGlowShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Dark Glow color - purple or theme accent
            Color glowColor = StyleShadows.GetShadowColor(style);
            if (useThemeColors && theme?.AccentColor != null && theme.AccentColor != Color.Empty)
            {
                glowColor = theme.AccentColor;
            }

            // State-aware glow intensity
            float intensity = state switch
            {
                ControlState.Hovered => 1.0f,   // Brighter on hover
                ControlState.Pressed => 0.6f,   // Dimmer when pressed
                ControlState.Focused => 1.2f,   // Brightest on focus
                ControlState.Disabled => 0.2f,  // Very dim when disabled
                _ => 0.7f                       // Default subtle glow
            };

            // Use glow helper
            return ShadowPainterHelpers.PaintGlow(
                g, path, radius,
                glowColor, intensity);
        }
    }
}
