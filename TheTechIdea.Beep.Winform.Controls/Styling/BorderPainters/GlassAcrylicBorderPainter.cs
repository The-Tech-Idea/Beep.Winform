using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// GlassAcrylic border painter - Frosted glass with subtle translucent border
    /// Glass Acrylic UX: Translucent glow opacity varies by state
    /// </summary>
    public static class GlassAcrylicBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Glass Acrylic UX: Always subtle translucent border
            Color borderColor = BorderPainterHelpers.WithAlpha(255, 255, 255, 60);
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, StyleBorders.GetBorderWidth(style), state);

            // Glass Acrylic: Glow opacity varies by state
            bool showGlow = false;
            float glowIntensity = 0.7f;

            switch (state)
            {
                case ControlState.Hovered:
                    // Glass: Brighter glow on hover (70% opacity)
                    showGlow = true;
                    glowIntensity = 0.7f;
                    break;
                    
                case ControlState.Pressed:
                    // Glass: Prominent glow on press (90% opacity)
                    showGlow = true;
                    glowIntensity = 0.9f;
                    break;
                    
                case ControlState.Selected:
                    // Glass: Full glow with border (100% opacity)
                    showGlow = true;
                    glowIntensity = 1.0f;
                    break;
                    
                case ControlState.Disabled:
                    // Glass: Minimal glow (15% opacity)
                    showGlow = true;
                    glowIntensity = 0.15f;
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                showGlow = true;
                glowIntensity = 0.7f; // Subtle white glow on focus
            }

            // Glass: Add subtle focus glow
            if (showGlow)
            {
                Color glowColor = BorderPainterHelpers.WithAlpha(255, 255, 255, 40); // Subtle white glow
                float glowWidth = StyleBorders.GetGlowWidth(style); // 1.5f for GlassAcrylic
                BorderPainterHelpers.PaintGlowBorder(g, path, glowColor, glowWidth, glowIntensity);
            }
        }
    }
}

