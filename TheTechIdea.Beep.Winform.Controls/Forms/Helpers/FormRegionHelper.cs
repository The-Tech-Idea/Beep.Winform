using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Handles creation and application of the rounded region for the host form.
    /// Defers updates while the form is actively being resized/moved if requested.
    /// </summary>
    internal sealed class FormRegionHelper : IDisposable
    {
        private readonly IBeepModernFormHost _host;
        private GraphicsPath _cachedPath;
        private Size _cachedSize;
        private int _cachedRadius;

        public bool DeferWhileResizing { get; set; } = true;

        public FormRegionHelper(IBeepModernFormHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        public void EnsureRegion(bool force = false)
        {
            var form = _host.AsForm;
            if (form.WindowState == System.Windows.Forms.FormWindowState.Maximized || _host.BorderRadius <= 0)
            {
                if (form.Region != null)
                {
                    form.Region.Dispose();
                    form.Region = null;
                }
                return;
            }

            if (!force && _cachedPath != null && _cachedSize == form.ClientSize && _cachedRadius == _host.BorderRadius)
                return;

            _cachedPath?.Dispose();
            // Use full ClientRectangle like the old working code
            var rect = new Rectangle(0, 0, form.ClientSize.Width, form.ClientSize.Height);
            _cachedPath = BuildPath(rect, _host.BorderRadius);
            _cachedSize = form.ClientSize;
            _cachedRadius = _host.BorderRadius;

            form.Region = new Region(_cachedPath);
        }

        private static GraphicsPath BuildPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0 || radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            if (d <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            var arc = new Rectangle(rect.X, rect.Y, d, d);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - d; path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - d; path.AddArc(arc, 0, 90);
            arc.X = rect.Left; path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        public void InvalidateRegion()
        {
            _cachedSize = Size.Empty;
        }

        public void Dispose()
        {
            _cachedPath?.Dispose();
            _cachedPath = null;
        }
    }
}
