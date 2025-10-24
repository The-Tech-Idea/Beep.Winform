using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Background painter for GlassAcrylic Style
    /// </summary>
    public static class GlassBackgroundPainter
    {
        /// <summary>
        /// Paint glass/acrylic background
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            // Base frosted glass color (semi-transparent white)
            Color glassColor = Color.FromArgb(180, 255, 255, 255);
            using (var glassBrush = new SolidBrush(glassColor))
            {
                g.FillPath(glassBrush, path);
            }
            
            // Subtle gradient overlay for depth
            RectangleF bounds = path.GetBounds();
            Color topGlass = Color.FromArgb(60, 255, 255, 255);
            Color bottomGlass = Color.FromArgb(20, 255, 255, 255);
            using (var gradientBrush = new LinearGradientBrush(
                bounds,
                topGlass,
                bottomGlass,
                LinearGradientMode.Vertical))
            {
                g.FillPath(gradientBrush, path);
            }
            
            // Subtle border highlight
            float borderWidth = 1f;
            Color borderHighlight = Color.FromArgb(100, 255, 255, 255);
            using (var borderPen = new Pen(borderHighlight, borderWidth))
            {
                g.DrawPath(borderPen, path);
            }
        }
    }
}

