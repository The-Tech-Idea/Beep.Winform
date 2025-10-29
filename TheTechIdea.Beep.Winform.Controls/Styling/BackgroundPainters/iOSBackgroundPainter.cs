using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for iOS15 Style
    /// </summary>
    public static class iOSBackgroundPainter
    {
        /// <summary>
        /// Paint iOS background with subtle blur effect
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            
            // Base background
            var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
            g.FillPath(bgBrush, path);
            
            // iOS has a subtle translucent overlay effect
            Color overlay = Color.FromArgb(10, 255, 255, 255);
            var overlayBrush = PaintersFactory.GetSolidBrush(overlay);
            g.FillPath(overlayBrush, path);
            
            // Very subtle bottom shadow for depth using clipped region
            RectangleF bounds = path.GetBounds();
            var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(20, 0, 0, 0));
            using (var bottomRegion = new Region(path))
            {
                // Clip to bottom 1px line
                using (var clipRect = new GraphicsPath())
                {
                    clipRect.AddRectangle(new RectangleF(bounds.X, bounds.Bottom - 1, bounds.Width, 1));
                    bottomRegion.Intersect(clipRect);
                    g.SetClip(bottomRegion, CombineMode.Replace);
                    g.FillPath(shadowBrush, path);
                    g.ResetClip();
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

