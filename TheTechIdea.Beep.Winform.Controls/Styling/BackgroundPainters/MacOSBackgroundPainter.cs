using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for macOS Big Sur style
    /// </summary>
    public static class MacOSBackgroundPainter
    {
        /// <summary>
        /// Paint macOS background with system-style appearance
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            
            // Base background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }
            
            // macOS has a subtle gradient for depth
            RectangleF bounds = path.GetBounds();
            Color topTint = Color.FromArgb(12, 255, 255, 255);
            Color bottomTint = Color.FromArgb(5, 0, 0, 0);
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

