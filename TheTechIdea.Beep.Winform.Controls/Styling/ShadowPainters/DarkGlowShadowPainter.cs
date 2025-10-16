using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Dark glow shadow painter
    /// Uses colored glow effect instead of traditional shadow
    /// </summary>
    public static class DarkGlowShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            // DarkGlow UX: Colored glow effect with state-aware intensity
            if (!StyleShadows.HasShadow(style)) return path;

            // DarkGlow uses colored glow instead of traditional shadow
            Color glowColor = StyleShadows.GetShadowColor(style); // Purple glow from StyleShadows

            // Get glow color from theme if available
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor("Accent");
                if (themeColor != Color.Empty)
                    glowColor = themeColor;
            }

            // State-aware glow intensity
            float glowIntensity = 0.6f; // Normal state
            if (state == ControlState.Hovered)
                glowIntensity = 1.1f; // Hover: brighter glow
            else if (state == ControlState.Focused)
                glowIntensity = 1.5f; // Focus: brightest glow
            else if (state == ControlState.Pressed)
                glowIntensity = 0.8f; // Press: moderate glow

            // Paint glow
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintGlow(g, path, radius, glowColor, glowIntensity);

            return remainingPath;
        }
    }
}
