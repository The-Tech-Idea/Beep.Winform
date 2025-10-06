using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for special effect styles (Neumorphism, GlassAcrylic, DarkGlow)
    /// These styles typically have no traditional borders
    /// </summary>
    public static class EffectBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Effect UX: Special effects (Neumorphism, Glass, Glow) with enhanced state support
            // These styles typically have no traditional borders, but can have glow effects

            if (style == BeepControlStyle.DarkGlow)
            {
                // DarkGlow: Dynamic glow intensity based on state
                Color glowColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
                float glowIntensity = 0.6f; // Normal state

                if (state == ControlState.Hovered)
                    glowIntensity = 1.1f; // Hover: brighter glow
                else if (isFocused)
                    glowIntensity = 1.5f; // Focus: brightest glow

                Color stateGlow = BorderPainterHelpers.WithAlpha(glowColor, (int)(glowIntensity * 100));
                BorderPainterHelpers.PaintGlowBorder(g, path, stateGlow, StyleBorders.GetGlowWidth(style) * glowIntensity, glowIntensity);
            }
            else if (style == BeepControlStyle.GlassAcrylic && isFocused)
            {
                // GlassAcrylic: Subtle white focus glow
                Color focusGlow = BorderPainterHelpers.WithAlpha(Color.White, 40);
                BorderPainterHelpers.PaintGlowBorder(g, path, focusGlow, StyleBorders.GetGlowWidth(style));
            }
            // Neumorphism: No borders, effect is in background painter
        }
        
        private static Color GetColor(BeepControlStyle style, System.Func<BeepControlStyle, Color> styleColorFunc, string themeColorKey, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }
    }
}
