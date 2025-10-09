using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Provides unified layout rectangles so painting and child layout stay consistent.
    /// </summary>
    internal sealed class FormLayoutHelper
    {
        private readonly IBeepModernFormHost _host;

        public FormLayoutHelper(IBeepModernFormHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        /// <summary>
        /// Full paint bounds inside current Padding.
        /// </summary>
        public Rectangle GetPaintBounds()
        {
            var form = _host.AsForm;
            if (form.ClientSize.Width <= 0 || form.ClientSize.Height <= 0) return Rectangle.Empty;
            var pad = _host.Padding;
            return new Rectangle(pad.Left, pad.Top,
                Math.Max(0, form.ClientSize.Width - pad.Horizontal),
                Math.Max(0, form.ClientSize.Height - pad.Vertical));
        }

        /// <summary>
        /// Content bounds (same as paint bounds for now; can add caption/ribbon offsets later).
        /// </summary>
        public Rectangle GetContentBounds() => GetPaintBounds();

        /// <summary>
        /// Full window bounds in window coordinates (0,0,Width,Height).
        /// Useful for non-client painting and region creation.
        /// </summary>
        public Rectangle GetWindowBounds()
        {
            var form = _host.AsForm;
            return new Rectangle(0, 0, Math.Max(0, form.Width), Math.Max(0, form.Height));
        }

        /// <summary>
        /// Creates a rounded rectangle path from a rectangle and corner radius.
        /// Returns a new GraphicsPath that the caller must dispose.
        /// </summary>
        public GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
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
                path.Reset();
                if (rect.Width > 0 && rect.Height > 0)
                    path.AddRectangle(rect);
            }

            return path;
        }
    }
}
