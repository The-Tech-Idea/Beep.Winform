using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Extension methods for Graphics class to support Material Design drawing operations
    /// </summary>
    public static class GraphicsExtensions
    {
        /// <summary>
        /// Fills a rounded rectangle with specified corner radii
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="brush">Brush to fill with</param>
        /// <param name="rect">Rectangle bounds</param>
        /// <param name="topLeftRadius">Top-left corner radius</param>
        /// <param name="topRightRadius">Top-right corner radius</param>
        /// <param name="bottomLeftRadius">Bottom-left corner radius</param>
        /// <param name="bottomRightRadius">Bottom-right corner radius</param>
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, RectangleF rect, 
            float topLeftRadius, float topRightRadius, float bottomLeftRadius, float bottomRightRadius)
        {
            using (var path = CreateRoundedRectanglePath(rect, topLeftRadius, topRightRadius, bottomLeftRadius, bottomRightRadius))
            {
                graphics.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Draws a rounded rectangle with specified corner radii
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="pen">Pen to draw with</param>
        /// <param name="rect">Rectangle bounds</param>
        /// <param name="topLeftRadius">Top-left corner radius</param>
        /// <param name="topRightRadius">Top-right corner radius</param>
        /// <param name="bottomLeftRadius">Bottom-left corner radius</param>
        /// <param name="bottomRightRadius">Bottom-right corner radius</param>
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, RectangleF rect, 
            float topLeftRadius, float topRightRadius, float bottomLeftRadius, float bottomRightRadius)
        {
            using (var path = CreateRoundedRectanglePath(rect, topLeftRadius, topRightRadius, bottomLeftRadius, bottomRightRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Creates a GraphicsPath for a rounded rectangle with specified corner radii
        /// </summary>
        /// <param name="rect">Rectangle bounds</param>
        /// <param name="topLeftRadius">Top-left corner radius</param>
        /// <param name="topRightRadius">Top-right corner radius</param>
        /// <param name="bottomLeftRadius">Bottom-left corner radius</param>
        /// <param name="bottomRightRadius">Bottom-right corner radius</param>
        /// <returns>GraphicsPath for the rounded rectangle</returns>
        public static GraphicsPath CreateRoundedRectanglePath(RectangleF rect, 
            float topLeftRadius, float topRightRadius, float bottomLeftRadius, float bottomRightRadius)
        {
            var path = new GraphicsPath();

            // Ensure radii don't exceed rectangle dimensions
            float maxRadius = Math.Min(rect.Width, rect.Height) / 2f;
            topLeftRadius = Math.Min(topLeftRadius, maxRadius);
            topRightRadius = Math.Min(topRightRadius, maxRadius);
            bottomLeftRadius = Math.Min(bottomLeftRadius, maxRadius);
            bottomRightRadius = Math.Min(bottomRightRadius, maxRadius);

            // Top-left corner
            if (topLeftRadius > 0)
            {
                path.AddArc(rect.X, rect.Y, topLeftRadius * 2, topLeftRadius * 2, 180, 90);
            }
            else
            {
                path.AddLine(rect.X, rect.Y, rect.X, rect.Y);
            }

            // Top edge
            path.AddLine(rect.X + topLeftRadius, rect.Y, rect.Right - topRightRadius, rect.Y);

            // Top-right corner
            if (topRightRadius > 0)
            {
                path.AddArc(rect.Right - topRightRadius * 2, rect.Y, topRightRadius * 2, topRightRadius * 2, 270, 90);
            }

            // Right edge
            path.AddLine(rect.Right, rect.Y + topRightRadius, rect.Right, rect.Bottom - bottomRightRadius);

            // Bottom-right corner
            if (bottomRightRadius > 0)
            {
                path.AddArc(rect.Right - bottomRightRadius * 2, rect.Bottom - bottomRightRadius * 2, 
                    bottomRightRadius * 2, bottomRightRadius * 2, 0, 90);
            }

            // Bottom edge
            path.AddLine(rect.Right - bottomRightRadius, rect.Bottom, rect.X + bottomLeftRadius, rect.Bottom);

            // Bottom-left corner
            if (bottomLeftRadius > 0)
            {
                path.AddArc(rect.X, rect.Bottom - bottomLeftRadius * 2, bottomLeftRadius * 2, bottomLeftRadius * 2, 90, 90);
            }

            // Left edge
            path.AddLine(rect.X, rect.Bottom - bottomLeftRadius, rect.X, rect.Y + topLeftRadius);

            path.CloseFigure();
            return path;
        }
    }
}