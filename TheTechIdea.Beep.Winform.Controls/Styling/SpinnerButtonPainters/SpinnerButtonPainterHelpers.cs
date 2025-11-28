using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;


namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Shared helper methods for all button painters
    /// </summary>
    public static class SpinnerButtonPainterHelpers
    {
    

   
        /// <summary>
        /// Create a rounded rectangle path
        /// </summary>
        public static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || bounds.Width < radius * 2 || bounds.Height < radius * 2)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Draw an arrow (up or down) centered in the bounds
        /// </summary>
        public static void DrawArrow(Graphics g, Rectangle bounds, ArrowDirection direction, Color color)
        {
            int centerX = bounds.X + bounds.Width / 2;
            int centerY = bounds.Y + bounds.Height / 2;
            int arrowSize = Math.Min(bounds.Width, bounds.Height) / 3;

            Point[] points = direction == ArrowDirection.Up
                ? new Point[]
                {
                    new Point(centerX, centerY - arrowSize / 2),
                    new Point(centerX - arrowSize / 2, centerY + arrowSize / 2),
                    new Point(centerX + arrowSize / 2, centerY + arrowSize / 2)
                }
                : new Point[]
                {
                    new Point(centerX, centerY + arrowSize / 2),
                    new Point(centerX - arrowSize / 2, centerY - arrowSize / 2),
                    new Point(centerX + arrowSize / 2, centerY - arrowSize / 2)
                };

            using (var brush = new SolidBrush(color))
            {
                g.FillPolygon(brush, points);
            }
        }

        /// <summary>
        /// Lighten a color by a percentage
        /// </summary>
        public static Color Lighten(Color color, float percent)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, color.R + (int)(255 * percent)),
                Math.Min(255, color.G + (int)(255 * percent)),
                Math.Min(255, color.B + (int)(255 * percent))
            );
        }

        /// <summary>
        /// Darken a color by a percentage
        /// </summary>
        public static Color Darken(Color color, float percent)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, color.R - (int)(color.R * percent)),
                Math.Max(0, color.G - (int)(color.G * percent)),
                Math.Max(0, color.B - (int)(color.B * percent))
            );
        }

        /// <summary>
        /// Add alpha transparency to a color
        /// </summary>
        public static Color WithAlpha(Color color, int alpha)
        {
            return Color.FromArgb(Math.Max(0, Math.Min(255, alpha)), color);
        }

        /// <summary>
        /// Add alpha transparency to a color, unless the base color is Color.Empty. Returns Color.Empty in that case.
        /// </summary>
        public static Color WithAlphaIfNotEmpty(Color color, int alpha)
        {
            if (color == Color.Empty) return Color.Empty;
            return WithAlpha(color, alpha);
        }

        /// <summary>
        /// Create color with RGB and alpha
        /// </summary>
        public static Color WithAlpha(int r, int g, int b, int alpha)
        {
            return Color.FromArgb(Math.Max(0, Math.Min(255, alpha)), r, g, b);
        }

        /// <summary>
        /// Apply state-based color modification to base color
        /// </summary>
        public static Color ApplyState(Color baseColor, ControlState state)
        {
            switch (state)
            {
                case ControlState.Hovered:
                    return Lighten(baseColor, 0.05f); // 5% lighter
                
                case ControlState.Pressed:
                    return Darken(baseColor, 0.10f); // 10% darker
                
                case ControlState.Selected:
                    return Lighten(baseColor, 0.08f); // 8% lighter
                
                case ControlState.Disabled:
                    return WithAlpha(baseColor, (int)(baseColor.A * 0.6f)); // 40% opacity reduction
                
                case ControlState.Focused:
                    return Lighten(baseColor, 0.03f); // 3% lighter
                
                default:
                    return baseColor;
            }
        }

        /// <summary>
        /// Get state overlay color to be painted as final layer
        /// </summary>
        public static Color GetStateOverlay(ControlState state)
        {
            switch (state)
            {
                case ControlState.Hovered:
                    return WithAlpha(255, 255, 255, 20); // 20α white
                
                case ControlState.Pressed:
                    return WithAlpha(0, 0, 0, 30); // 30α black
                
                case ControlState.Selected:
                    return WithAlpha(255, 255, 255, 25); // 25α white
                
                case ControlState.Disabled:
                    return WithAlpha(128, 128, 128, 80); // 80α gray
                
                case ControlState.Focused:
                    return WithAlpha(255, 255, 255, 15); // 15α white
                
                default:
                    return Color.Transparent;
            }
        }
    }
}
