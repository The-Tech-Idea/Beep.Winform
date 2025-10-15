using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for DarkGlow style with neon effects
    /// </summary>
    public static class GlowBackgroundPainter
    {
        /// <summary>
        /// Paint DarkGlow background with neon glow effect
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            Color glowColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            
            // Dark background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }
            
            // Inner glow effect
            int glowSize = 3;
            for (int i = glowSize; i > 0; i--)
            {
                int alpha = (int)(30 * (1 - (float)i / glowSize));
                Color glowStep = Color.FromArgb(alpha, glowColor);
                
                using (var innerPath = BackgroundPainterHelpers.CreateInsetPath(path, i))
                using (var glowPen = new Pen(glowStep, 2f))
                {
                    g.DrawPath(glowPen, innerPath);
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

