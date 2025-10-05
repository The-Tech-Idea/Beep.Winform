using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for Fluent Design (Fluent2, Windows11Mica)
    /// Includes accent bar support
    /// </summary>
    public static class FluentBorderPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, bool isFocused, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            // Draw border if not filled
            if (!StyleBorders.IsFilled(style))
            {
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
            
            // Draw accent bar if style uses it
            int accentWidth = StyleBorders.GetAccentBarWidth(style);
            if (accentWidth > 0 && isFocused)
            {
                Color accentColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
                Rectangle accentRect = new Rectangle(bounds.X, bounds.Y, accentWidth, bounds.Height);
                using (var accentBrush = new SolidBrush(accentColor))
                {
                    g.FillRectangle(accentBrush, accentRect);
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
