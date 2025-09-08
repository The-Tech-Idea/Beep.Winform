using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Helper for painting form borders with radius and thickness support.
    /// Handles both rectangular and rounded border rendering.
    /// </summary>
    internal class FormBorderPainter
    {
        private readonly IBeepModernFormHost _host;

        public FormBorderPainter(IBeepModernFormHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        /// <summary>
        /// Paints the border within the specified bounds.
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Paint bounds</param>
        public void Paint(Graphics g, Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0 || _host.BorderThickness <= 0)
                return;

            var borderColor = GetBorderColor();
            if (borderColor == Color.Transparent || borderColor == Color.Empty)
                return;

            try
            {
                using var pen = new Pen(borderColor, _host.BorderThickness)
                {
                    Alignment = PenAlignment.Inset
                };

                // Adjust bounds for pen thickness
                var adjustedBounds = new Rectangle(
                    bounds.X + _host.BorderThickness / 2,
                    bounds.Y + _host.BorderThickness / 2,
                    bounds.Width - _host.BorderThickness,
                    bounds.Height - _host.BorderThickness
                );

                if (_host.BorderRadius > 0)
                {
                    // Paint rounded border
                    using var path = CreateRoundedRectanglePath(adjustedBounds, _host.BorderRadius);
                    g.DrawPath(pen, path);
                }
                else
                {
                    // Paint rectangular border
                    g.DrawRectangle(pen, adjustedBounds);
                }
            }
            catch (Exception)
            {
                // Fallback to simple rectangle
                using var pen = new Pen(borderColor, Math.Max(1, _host.BorderThickness));
                g.DrawRectangle(pen, bounds);
            }
        }

        /// <summary>
        /// Gets the border color from theme or form properties.
        /// </summary>
        private Color GetBorderColor()
        {
            // Try to get border color from theme first
            if (_host.CurrentTheme?.BorderColor != Color.Empty)
                return _host.CurrentTheme.BorderColor;

            // Try to get from form if it has BorderColor property
            var form = _host.AsForm;
            var borderColorProperty = form.GetType().GetProperty("BorderColor");
            if (borderColorProperty != null && borderColorProperty.PropertyType == typeof(Color))
            {
                var color = (Color)borderColorProperty.GetValue(form);
                if (color != Color.Empty)
                    return color;
            }

            // Default border color
            return SystemColors.ControlDark;
        }

        /// <summary>
        /// Creates a rounded rectangle graphics path.
        /// </summary>
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                if (rect.Width > 0 && rect.Height > 0)
                    path.AddRectangle(rect);
                return path;
            }

            int diameter = Math.Min(Math.Min(rect.Width, rect.Height), radius * 2);
            if (diameter <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            try
            {
                var arcRect = new Rectangle(rect.X, rect.Y, diameter, diameter);
                path.AddArc(arcRect, 180, 90);
                
                arcRect.X = rect.Right - diameter;
                path.AddArc(arcRect, 270, 90);
                
                arcRect.Y = rect.Bottom - diameter;
                path.AddArc(arcRect, 0, 90);
                
                arcRect.X = rect.Left;
                path.AddArc(arcRect, 90, 90);
                
                path.CloseFigure();
            }
            catch (ArgumentException)
            {
                // Fallback to rectangle on error
                path.Reset();
                if (rect.Width > 0 && rect.Height > 0)
                    path.AddRectangle(rect);
            }

            return path;
        }

        /// <summary>
        /// Applies theme-specific border styling.
        /// </summary>
        public void ApplyTheme()
        {
            // Trigger repaint to apply new theme colors
            _host.Invalidate();
        }
    }
}