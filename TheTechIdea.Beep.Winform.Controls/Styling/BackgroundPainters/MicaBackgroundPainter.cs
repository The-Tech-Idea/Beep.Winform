using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for Windows 11 Mica style
    /// </summary>
    public static class MicaBackgroundPainter
    {
        /// <summary>
        /// Paint Windows 11 Mica background
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            
            // Mica has subtle noise/texture effect - simplified here
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }
            
            // Very subtle gradient for depth
            RectangleF bounds = path.GetBounds();
            Color topTint = Color.FromArgb(8, 255, 255, 255);
            Color bottomTint = Color.FromArgb(4, 0, 0, 0);
            using (var gradientBrush = new LinearGradientBrush(
                bounds,
                topTint,
                bottomTint,
                LinearGradientMode.Vertical))
            {
                g.FillPath(gradientBrush, path);
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
