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
        /// Paints the border using the provided graphics path.
        /// Simple approach like old working code - draws border on same path as background.
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="formPath">The form path to draw border on</param>
        public void PaintBorder(Graphics g, GraphicsPath formPath)
        {
            if (g == null || formPath == null) return;
            if (_host.BorderThickness <= 0 || _host.AsForm.WindowState == FormWindowState.Maximized) return;

            var borderColor = GetBorderColor();
            // DEBUG: Log what we're getting
            System.Diagnostics.Debug.WriteLine($"[FormBorderPainter] BorderThickness={_host.BorderThickness}, BorderColor={borderColor}, IsEmpty={borderColor == Color.Empty}, IsTransparent={borderColor == Color.Transparent}");
            
            if (borderColor == Color.Transparent || borderColor == Color.Empty) return;

            try
            {
                using var pen = new Pen(borderColor, _host.BorderThickness)
                {
                    // Draw fully inside the window bounds so all sides look identical
                    Alignment = PenAlignment.Inset
                };

                // Draw border on the same path used for background and Region
                // This is the simple approach that worked perfectly in old code
                System.Diagnostics.Debug.WriteLine($"[FormBorderPainter] Drawing border with pen width {pen.Width}, color {pen.Color}");
                g.DrawPath(pen, formPath);
            }
            catch (Exception ex)
            {
                // Silent fallback - don't crash on border painting errors
                System.Diagnostics.Debug.WriteLine($"[FormBorderPainter] ERROR: {ex.Message}");
            }
        }

        /// <summary>
        /// Paints the border within the specified bounds (legacy method).
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
                // Simplified: Use Center alignment like old code
                using var pen = new Pen(borderColor, _host.BorderThickness)
                {
                    Alignment = PenAlignment.Center
                };

                if (_host.BorderRadius > 0)
                {
                    // Paint rounded border on full bounds
                    using var path = CreateRoundedRectanglePath(bounds, _host.BorderRadius);
                    g.DrawPath(pen, path);
                }
                else
                {
                    // Paint rectangular border on full bounds
                    g.DrawRectangle(pen, bounds);
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
            // Prefer explicit form BorderColor if set (lets runtime changes win)
            var form = _host.AsForm;
            var borderColorProperty = form.GetType().GetProperty("BorderColor");
            if (borderColorProperty != null && borderColorProperty.PropertyType == typeof(Color))
            {
                var color = (Color)borderColorProperty.GetValue(form);
                if (color != Color.Empty)
                    return color;
            }

            // Otherwise fall back to theme
            if (_host.CurrentTheme?.BorderColor != Color.Empty)
                return _host.CurrentTheme.BorderColor;

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

        /// <summary>
        /// Paints the non-client window border for the form using window coordinates.
        /// All geometry (inset to avoid clipping, rounded path, and pen alignment) lives here
        /// so that BeepiForm only delegates work to helpers.
        /// </summary>
        /// <param name="g">Graphics from GetWindowDC</param>
        /// <param name="windowBounds">Full window bounds (0,0,Width,Height)</param>
        /// <param name="borderRadius">Corner radius</param>
        /// <param name="borderThickness">Border thickness</param>
        public void PaintWindowBorder(Graphics g, Rectangle windowBounds, int borderRadius, int borderThickness)
        {
            if (g == null) return;
            if (borderThickness <= 0 || _host.AsForm.WindowState == FormWindowState.Maximized) return;

            var color = GetBorderColor();
            if (color == Color.Empty || color == Color.Transparent) return;

            // Inset by half the thickness so the stroke remains fully inside the window
            int half = Math.Max(0, borderThickness / 2);
            var inset = new Rectangle(
                windowBounds.X + half,
                windowBounds.Y + half,
                Math.Max(0, windowBounds.Width - half * 2),
                Math.Max(0, windowBounds.Height - half * 2)
            );

            using var pen = new Pen(color, borderThickness) { Alignment = PenAlignment.Inset };

            if (borderRadius > 0)
            {
                using var path = CreateRoundedRectanglePath(inset, borderRadius);
                g.DrawPath(pen, path);
            }
            else
            {
                // Using DrawRectangle with Inset alignment keeps consistent thickness visually
                g.DrawRectangle(pen, inset);
            }
        }
    }
}