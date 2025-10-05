using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for special effect styles (Neumorphism, GlassAcrylic, DarkGlow)
    /// These styles typically have no traditional borders
    /// </summary>
    public static class EffectBorderPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, bool isFocused, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            // Neumorphism, Glass, and Glow don't use traditional borders
            // Their borders are part of the background effect
            // Only draw if focused and style allows
            
            if (isFocused && style == BeepControlStyle.DarkGlow)
            {
                // Draw focused glow border
                Color glowColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
                using (var glowPen = new Pen(Color.FromArgb(100, glowColor), 2f))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawPath(glowPen, path);
                }
            }
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
