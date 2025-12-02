using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Glassmorphism shadow painter - Frosted glass effect shadow
    /// Subtle glow for glass/blur effect aesthetics
    /// State-aware with soft edges
    /// </summary>
    public static class GlassmorphismShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color glowColor = useThemeColors && theme != null 
                ? theme.AccentColor 
                : Color.White;

            // Increased intensity for better visibility
            float intensity = state switch
            {
                ControlState.Hovered => 0.6f,
                ControlState.Pressed => 0.3f,
                ControlState.Focused => 0.5f,
                ControlState.Disabled => 0.2f,
                _ => 0.4f
            };

            return ShadowPainterHelpers.PaintGlassShadow(
                g, path, radius, glowColor, intensity);
        }
    }
}
