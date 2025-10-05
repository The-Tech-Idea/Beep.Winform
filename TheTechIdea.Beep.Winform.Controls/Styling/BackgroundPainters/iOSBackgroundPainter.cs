using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for iOS15 style
    /// </summary>
    public static class iOSBackgroundPainter
    {
        /// <summary>
        /// Paint iOS background with subtle blur effect
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            
            // Base background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }
            
            // iOS has a subtle translucent overlay effect
            Color overlay = Color.FromArgb(10, 255, 255, 255);
            using (var overlayBrush = new SolidBrush(overlay))
            {
                g.FillPath(overlayBrush, path);
            }
            
            // Very subtle bottom shadow for depth
            Rectangle bottomShadow = new Rectangle(bounds.X, bounds.Bottom - 1, bounds.Width, 1);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
            {
                g.SetClip(path);
                g.FillRectangle(shadowBrush, bottomShadow);
                g.ResetClip();
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

