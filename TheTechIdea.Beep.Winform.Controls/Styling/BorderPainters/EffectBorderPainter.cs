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
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (style == BeepControlStyle.DarkGlow)
            {
                Color glowColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
                float glowIntensity = 0.6f;
                if (state == ControlState.Hovered)
                    glowIntensity = 1.1f;
                else if (isFocused)
                    glowIntensity = 1.5f;
                Color stateGlow = BorderPainterHelpers.WithAlpha(glowColor, (int)(glowIntensity * 100));
                BorderPainterHelpers.PaintGlowBorder(g, path, stateGlow, StyleBorders.GetGlowWidth(style) * glowIntensity, glowIntensity);
                    // Return area inside border using shape-aware inset
                    return path.CreateInsetPath(StyleBorders.GetGlowWidth(style));
            }
            else if (style == BeepControlStyle.GlassAcrylic && isFocused)
            {
                Color focusGlow = BorderPainterHelpers.WithAlpha(Color.White, 40);
                BorderPainterHelpers.PaintGlowBorder(g, path, focusGlow, StyleBorders.GetGlowWidth(style));
                    return path.CreateInsetPath(StyleBorders.GetGlowWidth(style));
            }
            // Neumorphism: No borders, effect is in background painter
            // Return original path for Neumorphism (no border)
            return path;
        }
        
        private static Color GetColor(BeepControlStyle style, System.Func<BeepControlStyle, Color> styleColorFunc, string themeColorKey, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(theme, themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }
    }
}
