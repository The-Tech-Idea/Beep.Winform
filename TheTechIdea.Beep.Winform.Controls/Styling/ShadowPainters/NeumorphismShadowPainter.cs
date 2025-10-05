using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Shadow painter for Neumorphism style
    /// Draws dual shadows (light top-left, dark bottom-right)
    /// </summary>
    public static class NeumorphismShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, BeepControlStyle style, GraphicsPath borderPath)
        {
            if (!StyleShadows.UsesDualShadows(style))
                return;
            
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);
            Color shadowColor = StyleShadows.GetShadowColor(style);
            Color highlightColor = StyleShadows.GetNeumorphismHighlight(style);
            int radius = StyleBorders.GetRadius(style);
            
            // Dark shadow (bottom-right)
            Rectangle shadowBounds = new Rectangle(
                bounds.X + offsetX,
                bounds.Y + offsetY,
                bounds.Width,
                bounds.Height
            );
            
            using (var shadowBrush = new SolidBrush(shadowColor))
            using (var path = CreateRoundedRectangle(shadowBounds, radius))
            {
                g.FillPath(shadowBrush, path);
            }
            
            // Light highlight (top-left)
            Rectangle highlightBounds = new Rectangle(
                bounds.X - offsetX,
                bounds.Y - offsetY,
                bounds.Width,
                bounds.Height
            );
            
            using (var highlightBrush = new SolidBrush(highlightColor))
            using (var path = CreateRoundedRectangle(highlightBounds, radius))
            {
                g.FillPath(highlightBrush, path);
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
    }
}
