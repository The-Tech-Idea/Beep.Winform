using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for Material Design styles (Material3, MaterialYou)
    /// </summary>
    public static class MaterialBorderPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, bool isFocused, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            // Material uses filled style, no border
            if (StyleBorders.IsFilled(style))
                return;
            
            Color borderColor = isFocused ? 
                GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors) : 
                GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            
            float borderWidth = StyleBorders.GetBorderWidth(style);
            
            if (borderWidth > 0)
            {
                using (var borderPen = new Pen(borderColor, borderWidth))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawPath(borderPen, path);
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
