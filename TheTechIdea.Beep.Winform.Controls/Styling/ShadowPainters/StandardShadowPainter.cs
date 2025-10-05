using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Shadow painter for standard single shadow styles
    /// Used by most styles that have simple drop shadows
    /// </summary>
    public static class StandardShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, BeepControlStyle style, GraphicsPath borderPath)
        {
            if (!StyleShadows.HasShadow(style))
                return;
            
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);
            Color shadowColor = StyleShadows.GetShadowColor(style);
            
            // Simple shadow implementation
            Rectangle shadowBounds = new Rectangle(
                bounds.X + offsetX,
                bounds.Y + offsetY,
                bounds.Width,
                bounds.Height
            );
            
            int radius = StyleBorders.GetRadius(style);
            
            // Draw shadow with reduced opacity
            using (var shadowBrush = new SolidBrush(shadowColor))
            using (var path = CreateRoundedRectangle(shadowBounds, radius))
            {
                g.FillPath(shadowBrush, path);
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
