using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for DarkGlow style with neon effects
    /// </summary>
    public static class GlowBackgroundPainter
    {
        /// <summary>
        /// Paint DarkGlow background with neon glow effect
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color bgColor = GetColor(style, StyleColors.GetBackground, "Background", theme, useThemeColors);
            Color glowColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            
            // Dark background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillPath(bgBrush, path);
            }
            
            // Inner glow effect
            int glowSize = 3;
            for (int i = glowSize; i > 0; i--)
            {
                int alpha = (int)(30 * (1 - (float)i / glowSize));
                Color glowStep = Color.FromArgb(alpha, glowColor);
                
                Rectangle innerBounds = new Rectangle(
                    bounds.X + i,
                    bounds.Y + i,
                    bounds.Width - (i * 2),
                    bounds.Height - (i * 2)
                );
                
                if (innerBounds.Width > 0 && innerBounds.Height > 0)
                {
                    int innerRadius = System.Math.Max(0, StyleBorders.GetRadius(style) - i);
                    using (var innerPath = CreateRoundedRectangle(innerBounds, innerRadius))
                    using (var glowPen = new Pen(glowStep, 2f))
                    {
                        g.DrawPath(glowPen, innerPath);
                    }
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
            Size size = new Size(diameter, diameter);
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

