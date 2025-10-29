using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for DarkGlow Style with neon effects
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

            var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
            g.FillPath(bgBrush, path);

            int glowSize =3;
            for (int i = glowSize; i >0; i--)
            {
                int alpha = (int)(30 * ((float)(glowSize - i +1) / glowSize));
                Color glowStep = Color.FromArgb(alpha, glowColor);

                var innerPath = GraphicsExtensions.CreateInsetPath(path, i);
                var glowPen = PaintersFactory.GetPen(glowStep,2f);
                g.DrawPath(glowPen, innerPath);
                innerPath.Dispose();
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

