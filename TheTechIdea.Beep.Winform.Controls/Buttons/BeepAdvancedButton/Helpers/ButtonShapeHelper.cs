using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers
{
    /// <summary>
    /// Shared helper class for creating button shapes used by all painters.
    /// Provides consistent shape rendering across all button styles.
    /// </summary>
    public static class ButtonShapeHelper
    {
        /// <summary>
        /// Get the appropriate corner radius based on the button shape
        /// </summary>
        public static int GetCornerRadiusForShape(ButtonShape shape, Rectangle bounds, int defaultRadius)
        {
            return shape switch
            {
                ButtonShape.Circle => Math.Min(bounds.Width, bounds.Height) / 2,
                ButtonShape.Pill => bounds.Height / 2,
                ButtonShape.Rectangle => 0,
                ButtonShape.Square => defaultRadius,
                ButtonShape.RoundedRectangle => defaultRadius,
                ButtonShape.Split => defaultRadius,
                _ => defaultRadius
            };
        }

        /// <summary>
        /// Create a GraphicsPath for the specified button shape
        /// </summary>
        public static GraphicsPath CreateShapePath(ButtonShape shape, Rectangle bounds, int cornerRadius)
        {
            GraphicsPath path = new GraphicsPath();

            switch (shape)
            {
                case ButtonShape.Circle:
                    // Perfect circle (or ellipse if bounds aren't square)
                    path.AddEllipse(bounds);
                    break;

                case ButtonShape.Rectangle:
                    // Sharp rectangle with no rounded corners
                    path.AddRectangle(bounds);
                    break;

                case ButtonShape.Pill:
                case ButtonShape.RoundedRectangle:
                case ButtonShape.Square:
                    // Rounded rectangle with specified corner radius
                    if (cornerRadius > 0)
                    {
                        int diameter = Math.Min(cornerRadius * 2, Math.Min(bounds.Width, bounds.Height));
                        
                        // Top-left
                        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
                        
                        // Top-right
                        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
                        
                        // Bottom-right
                        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
                        
                        // Bottom-left
                        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
                        
                        path.CloseFigure();
                    }
                    else
                    {
                        path.AddRectangle(bounds);
                    }
                    break;

                case ButtonShape.Split:
                    // For split buttons, we'll create the full outer shape
                    // Individual painters handle the internal split rendering
                    if (cornerRadius > 0)
                    {
                        int diameter = Math.Min(cornerRadius * 2, Math.Min(bounds.Width, bounds.Height));
                        
                        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
                        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
                        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
                        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
                        path.CloseFigure();
                    }
                    else
                    {
                        path.AddRectangle(bounds);
                    }
                    break;

                default:
                    // Default to rounded rectangle
                    path.AddRectangle(bounds);
                    break;
            }

            return path;
        }

        /// <summary>
        /// Create a rounded rectangle path with selective corner rounding.
        /// Used by Split buttons to round only specific corners.
        /// </summary>
        public static GraphicsPath CreatePartialRoundedRectangle(
            Rectangle bounds, 
            int radius,
            bool roundTopLeft = true, 
            bool roundTopRight = true,
            bool roundBottomRight = true, 
            bool roundBottomLeft = true)
        {
            GraphicsPath path = new GraphicsPath();
            
            if (radius <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));

            // Top left corner
            if (roundTopLeft)
                path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            else
                path.AddLine(bounds.X, bounds.Y, bounds.X, bounds.Y);

            // Top edge
            path.AddLine(
                roundTopLeft ? bounds.X + radius : bounds.X, 
                bounds.Y,
                roundTopRight ? bounds.Right - radius : bounds.Right, 
                bounds.Y);

            // Top right corner
            if (roundTopRight)
                path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            else
                path.AddLine(bounds.Right, bounds.Y, bounds.Right, bounds.Y);

            // Right edge
            path.AddLine(
                bounds.Right, 
                roundTopRight ? bounds.Y + radius : bounds.Y,
                bounds.Right, 
                roundBottomRight ? bounds.Bottom - radius : bounds.Bottom);

            // Bottom right corner
            if (roundBottomRight)
                path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            else
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Right, bounds.Bottom);

            // Bottom edge
            path.AddLine(
                roundBottomRight ? bounds.Right - radius : bounds.Right, 
                bounds.Bottom,
                roundBottomLeft ? bounds.X + radius : bounds.X, 
                bounds.Bottom);

            // Bottom left corner
            if (roundBottomLeft)
                path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            else
                path.AddLine(bounds.X, bounds.Bottom, bounds.X, bounds.Bottom);

            // Left edge
            path.AddLine(
                bounds.X, 
                roundBottomLeft ? bounds.Bottom - radius : bounds.Bottom,
                bounds.X, 
                roundTopLeft ? bounds.Y + radius : bounds.Y);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Fill a shape with the specified brush
        /// </summary>
        public static void FillShape(Graphics g, ButtonShape shape, Rectangle bounds, int cornerRadius, Brush brush)
        {
            using (var path = CreateShapePath(shape, bounds, cornerRadius))
            {
                g.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Draw a shape outline with the specified pen
        /// </summary>
        public static void DrawShape(Graphics g, ButtonShape shape, Rectangle bounds, int cornerRadius, Pen pen)
        {
            using (var path = CreateShapePath(shape, bounds, cornerRadius))
            {
                g.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Adjust bounds for proper centering based on shape
        /// </summary>
        public static Rectangle GetAdjustedBoundsForShape(ButtonShape shape, Rectangle originalBounds)
        {
            if (shape == ButtonShape.Circle || shape == ButtonShape.Square)
            {
                // Make square
                int size = Math.Min(originalBounds.Width, originalBounds.Height);
                int x = originalBounds.X + (originalBounds.Width - size) / 2;
                int y = originalBounds.Y + (originalBounds.Height - size) / 2;
                return new Rectangle(x, y, size, size);
            }

            return originalBounds;
        }
    }
}
