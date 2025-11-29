using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// KDE shadow painter - Breeze design system
    /// Blue glow effect on hover/focus (signature KDE Breeze)
    /// No shadow in normal state, accent-colored glow on interaction
    /// </summary>
    public static class KdeShadowPainter
    {
        // KDE Breeze signature blue
        private static readonly Color BreezeBlue = Color.FromArgb(61, 174, 233);

        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // KDE Breeze: Glow only on interaction states
            if (state == ControlState.Normal || state == ControlState.Disabled)
            {
                // No shadow in normal/disabled state (flat Breeze style)
                return path;
            }

            // Get glow color - use theme accent if available, otherwise Breeze blue
            Color glowColor = useThemeColors && theme != null 
                ? theme.AccentColor 
                : BreezeBlue;

            // State-based glow intensity
            float intensity;
            int glowRadius;

            switch (state)
            {
                case ControlState.Hovered:
                    intensity = 0.5f;
                    glowRadius = 8;
                    break;
                case ControlState.Focused:
                    intensity = 0.7f;
                    glowRadius = 10;
                    break;
                case ControlState.Selected:
                    intensity = 0.6f;
                    glowRadius = 8;
                    break;
                case ControlState.Pressed:
                    intensity = 0.3f;
                    glowRadius = 6;
                    break;
                default:
                    return path;
            }

            // Paint Breeze-style border glow
            return ShadowPainterHelpers.PaintBorderGlow(
                g, path, radius,
                glowColor, glowRadius, intensity);
        }
    }
}
