using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for Material Design styles (Material3, MaterialYou)
    /// </summary>
    public static class MaterialBackgroundPainter
    {
        /// <summary>
        /// Paint Material Design background with subtle elevation
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }
            
            // Subtle top highlight for material elevation
            Color highlight = Color.FromArgb(15, 255, 255, 255);
            Rectangle highlightRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, 2);
            using (var highlightBrush = new SolidBrush(highlight))
            {
                g.SetClip(path);
                g.FillRectangle(highlightBrush, highlightRect);
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

