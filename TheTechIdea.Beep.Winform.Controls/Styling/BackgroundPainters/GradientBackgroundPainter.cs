using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for GradientModern Style
    /// </summary>
    public static class GradientBackgroundPainter
    {
        /// <summary>
        /// Paint gradient background for GradientModern Style
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color primary = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            Color secondary = GetColor(style, StyleColors.GetSecondary, "Secondary", theme, useThemeColors);

            RectangleF bounds = path.GetBounds();
            // Use PaintersFactory to obtain a cached linear gradient brush for the given size and colors
            var brush = PaintersFactory.GetLinearGradientBrush(bounds, primary, secondary, LinearGradientMode.Vertical);
            g.FillPath(brush, path);
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

