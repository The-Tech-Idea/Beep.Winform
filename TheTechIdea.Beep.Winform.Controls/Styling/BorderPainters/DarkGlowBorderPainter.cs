using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// DarkGlow border painter - Cyan glow border effect
    /// DarkGlow UX: Glow intensity varies dramatically by state
    /// </summary>
    public static class DarkGlowBorderPainter 
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get DarkGlow colors
            Color glowColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 255, 255));
            float glowWidth = StyleBorders.GetGlowWidth(style); // 2.0f for DarkGlow
            float glowIntensity;

            // DarkGlow UX: Dramatic glow intensity changes by state
            switch (state)
            {
                case ControlState.Hovered:
                    // DarkGlow: Brighter glow on hover (80% intensity)
                    glowIntensity = 0.8f;
                    break;

                case ControlState.Pressed:
                    // DarkGlow: Intense glow on press (100% intensity + thicker)
                    glowIntensity = 1.0f;
                    glowWidth *= 1.3f; // Thicker glow
                    break;

                case ControlState.Selected:
                    // DarkGlow: Full intensity glow for selection
                    glowIntensity = 1.2f; // Extra bright
                    break;

                case ControlState.Disabled:
                    // DarkGlow: Very dim glow (20% intensity)
                    glowIntensity = 0.2f;
                    break;

                default: // Normal
                    // DarkGlow: Standard subtle glow (60% intensity)
                    glowIntensity = 0.6f;
                    break;
            }

            // Focus overrides state (strong pulse)
            if (isFocused)
            {
                glowIntensity = 1.5f; // Strong pulse on focus
            }

            BorderPainterHelpers.PaintGlowBorder(g, path, glowColor, glowWidth, glowIntensity);
        }
    }
}
