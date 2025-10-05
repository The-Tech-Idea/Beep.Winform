using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for web framework styles (Bootstrap, Tailwind, Stripe, Figma, Discord, AntDesign, Chakra)
    /// </summary>
    public static class WebFrameworkBorderPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, bool isFocused, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            // Most web frameworks use outlined borders
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
            
            // Tailwind ring effect when focused
            if (isFocused && style == BeepControlStyle.TailwindCard)
            {
                Color ringColor = Color.FromArgb(40, GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors));
                Rectangle ringBounds = new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, bounds.Height + 4);
                int ringRadius = StyleBorders.GetRadius(style) + 2;
                
                using (var ringPath = CreateRoundedRectangle(ringBounds, ringRadius))
                using (var ringPen = new Pen(ringColor, 3f))
                {
                    g.DrawPath(ringPen, ringPath);
                }
            }
        }
        
        private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            
            int diameter = radius * 2;
            System.Drawing.Size size = new System.Drawing.Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
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
