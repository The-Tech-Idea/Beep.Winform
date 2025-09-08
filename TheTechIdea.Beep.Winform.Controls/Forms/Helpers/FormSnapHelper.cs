using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Helper for managing snap hints and visual docking feedback.
    /// Shows overlay hints when dragging forms near screen edges.
    /// </summary>
    internal class FormSnapHelper : IDisposable
    {
        private readonly IBeepModernFormHost _host;
        private bool _disposed = false;
        private bool _showSnapHints = true;
        private bool _snapHintsVisible = false;
        private Rectangle _snapLeft, _snapRight, _snapTop, _snapBottom;
        private Rectangle _snapTopLeft, _snapTopRight, _snapBottomLeft, _snapBottomRight;
        private Color _snapHintColor = Color.FromArgb(80, 0, 120, 215); // Semi-transparent blue
        private int _snapThreshold = 20; // Distance from edge to trigger snap hint
        private int _snapHintThickness = 8;

        /// <summary>Gets or sets whether snap hints are enabled</summary>
        public bool EnableSnapHints // Renamed to avoid conflict
        {
            get => _showSnapHints;
            set { _showSnapHints = value; if (!value) HideSnapHints(); }
        }

        /// <summary>Gets or sets the snap hint color</summary>
        public Color SnapHintColor
        {
            get => _snapHintColor;
            set { _snapHintColor = value; if (_snapHintsVisible) _host.Invalidate(); }
        }

        /// <summary>Gets or sets the distance threshold for triggering snap hints</summary>
        public int SnapThreshold
        {
            get => _snapThreshold;
            set => _snapThreshold = Math.Max(1, value);
        }

        /// <summary>Gets or sets the thickness of snap hint overlays</summary>
        public int SnapHintThickness
        {
            get => _snapHintThickness;
            set { _snapHintThickness = Math.Max(1, value); UpdateSnapRegions(); }
        }

        /// <summary>Gets whether snap hints are currently visible</summary>
        public bool AreSnapHintsVisible => _snapHintsVisible;

        public FormSnapHelper(IBeepModernFormHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        /// <summary>
        /// Called when the form is being dragged to update snap hints.
        /// </summary>
        /// <param name="formLocation">Current form location</param>
        public void OnDragMove(Point formLocation)
        {
            if (_disposed || !_showSnapHints)
                return;

            try
            {
                var form = _host.AsForm;
                if (form.WindowState != FormWindowState.Normal)
                {
                    HideSnapHints();
                    return;
                }

                var screen = Screen.FromPoint(formLocation);
                var workingArea = screen.WorkingArea;
                
                // Check if form is near any edge
                bool nearLeft = formLocation.X <= workingArea.Left + _snapThreshold;
                bool nearRight = formLocation.X + form.Width >= workingArea.Right - _snapThreshold;
                bool nearTop = formLocation.Y <= workingArea.Top + _snapThreshold;
                bool nearBottom = formLocation.Y + form.Height >= workingArea.Bottom - _snapThreshold;

                if (nearLeft || nearRight || nearTop || nearBottom)
                {
                    ShowSnapHints(workingArea, nearLeft, nearRight, nearTop, nearBottom);
                }
                else
                {
                    HideSnapHints();
                }
            }
            catch (Exception)
            {
                // Ignore errors during drag operations
                HideSnapHints();
            }
        }

        /// <summary>
        /// Called when dragging ends to hide snap hints.
        /// </summary>
        public void OnDragEnd()
        {
            HideSnapHints();
        }

        /// <summary>
        /// Shows snap hints based on proximity to screen edges.
        /// </summary>
        private void ShowSnapHints(Rectangle workingArea, bool nearLeft, bool nearRight, bool nearTop, bool nearBottom)
        {
            if (_disposed)
                return;

            UpdateSnapRegions(workingArea);

            // Determine which snap regions to highlight
            bool showLeft = nearLeft && !nearTop && !nearBottom;
            bool showRight = nearRight && !nearTop && !nearBottom;
            bool showTop = nearTop && !nearLeft && !nearRight;
            bool showBottom = nearBottom && !nearLeft && !nearRight;
            bool showTopLeft = nearTop && nearLeft;
            bool showTopRight = nearTop && nearRight;
            bool showBottomLeft = nearBottom && nearLeft;
            bool showBottomRight = nearBottom && nearRight;

            // Show hints if any snap region is active
            if (showLeft || showRight || showTop || showBottom || showTopLeft || showTopRight || showBottomLeft || showBottomRight)
            {
                _snapHintsVisible = true;
                _host.AsForm.Invalidate(); // Trigger repaint to show overlay
            }
            else
            {
                HideSnapHints();
            }
        }

        /// <summary>
        /// Hides snap hints.
        /// </summary>
        public void HideSnapHints()
        {
            if (_snapHintsVisible)
            {
                _snapHintsVisible = false;
                _host.AsForm.Invalidate(); // Trigger repaint to hide overlay
            }
        }

        /// <summary>
        /// Updates snap region rectangles based on screen working area.
        /// </summary>
        private void UpdateSnapRegions(Rectangle? workingArea = null)
        {
            if (_disposed)
                return;

            var area = workingArea ?? Screen.FromControl(_host.AsForm).WorkingArea;
            var thickness = _snapHintThickness;

            // Edge snap regions
            _snapLeft = new Rectangle(area.Left, area.Top, thickness, area.Height);
            _snapRight = new Rectangle(area.Right - thickness, area.Top, thickness, area.Height);
            _snapTop = new Rectangle(area.Left, area.Top, area.Width, thickness);
            _snapBottom = new Rectangle(area.Left, area.Bottom - thickness, area.Width, thickness);

            // Corner snap regions (quarter screen)
            int halfWidth = area.Width / 2;
            int halfHeight = area.Height / 2;

            _snapTopLeft = new Rectangle(area.Left, area.Top, halfWidth, halfHeight);
            _snapTopRight = new Rectangle(area.Left + halfWidth, area.Top, halfWidth, halfHeight);
            _snapBottomLeft = new Rectangle(area.Left, area.Top + halfHeight, halfWidth, halfHeight);
            _snapBottomRight = new Rectangle(area.Left + halfWidth, area.Top + halfHeight, halfWidth, halfHeight);
        }

        /// <summary>
        /// Paints snap hint overlays.
        /// </summary>
        /// <param name="g">Graphics context</param>
        public void PaintOverlay(Graphics g)
        {
            if (_disposed || !_snapHintsVisible || !_showSnapHints)
                return;

            try
            {
                using var brush = new SolidBrush(_snapHintColor);
                var form = _host.AsForm;
                var formLocation = form.Location;
                var screen = Screen.FromPoint(formLocation);
                var workingArea = screen.WorkingArea;

                // Convert screen coordinates to form client coordinates
                var clientLeft = workingArea.Left - formLocation.X;
                var clientTop = workingArea.Top - formLocation.Y;
                var clientRight = workingArea.Right - formLocation.X;
                var clientBottom = workingArea.Bottom - formLocation.Y;

                // Check proximity and draw appropriate hints
                bool nearLeft = formLocation.X <= workingArea.Left + _snapThreshold;
                bool nearRight = formLocation.X + form.Width >= workingArea.Right - _snapThreshold;
                bool nearTop = formLocation.Y <= workingArea.Top + _snapThreshold;
                bool nearBottom = formLocation.Y + form.Height >= workingArea.Bottom - _snapThreshold;

                // Draw snap hint overlays
                if (nearLeft && !nearTop && !nearBottom)
                {
                    // Left half
                    var rect = new Rectangle(0, 0, (workingArea.Width / 2) - formLocation.X + workingArea.Left, form.Height);
                    if (rect.Width > 0) g.FillRectangle(brush, rect);
                }

                if (nearRight && !nearTop && !nearBottom)
                {
                    // Right half
                    var rect = new Rectangle((workingArea.Width / 2) - formLocation.X + workingArea.Left, 0, form.Width, form.Height);
                    if (rect.X < form.Width) g.FillRectangle(brush, rect);
                }

                if (nearTop && !nearLeft && !nearRight)
                {
                    // Top half
                    var rect = new Rectangle(0, 0, form.Width, (workingArea.Height / 2) - formLocation.Y + workingArea.Top);
                    if (rect.Height > 0) g.FillRectangle(brush, rect);
                }

                if (nearTop && nearLeft)
                {
                    // Top-left quarter
                    var rect = new Rectangle(0, 0, (workingArea.Width / 2) - formLocation.X + workingArea.Left, (workingArea.Height / 2) - formLocation.Y + workingArea.Top);
                    if (rect.Width > 0 && rect.Height > 0) g.FillRectangle(brush, rect);
                }

                if (nearTop && nearRight)
                {
                    // Top-right quarter
                    var rect = new Rectangle((workingArea.Width / 2) - formLocation.X + workingArea.Left, 0, form.Width, (workingArea.Height / 2) - formLocation.Y + workingArea.Top);
                    if (rect.X < form.Width && rect.Height > 0) g.FillRectangle(brush, rect);
                }

                // Add more corner combinations as needed...
            }
            catch (Exception)
            {
                // Ignore painting errors
            }
        }

        /// <summary>
        /// Gets the suggested snap position for the current form location.
        /// </summary>
        /// <param name="formLocation">Current form location</param>
        /// <returns>Suggested snap location, or null if no snap suggested</returns>
        public Rectangle? GetSuggestedSnapBounds(Point formLocation)
        {
            if (_disposed || !_showSnapHints)
                return null;

            try
            {
                var form = _host.AsForm;
                var screen = Screen.FromPoint(formLocation);
                var workingArea = screen.WorkingArea;

                bool nearLeft = formLocation.X <= workingArea.Left + _snapThreshold;
                bool nearRight = formLocation.X + form.Width >= workingArea.Right - _snapThreshold;
                bool nearTop = formLocation.Y <= workingArea.Top + _snapThreshold;
                bool nearBottom = formLocation.Y + form.Height >= workingArea.Bottom - _snapThreshold;

                // Return suggested bounds based on proximity
                if (nearTop && nearLeft)
                    return new Rectangle(workingArea.Left, workingArea.Top, workingArea.Width / 2, workingArea.Height / 2);
                
                if (nearTop && nearRight)
                    return new Rectangle(workingArea.Left + workingArea.Width / 2, workingArea.Top, workingArea.Width / 2, workingArea.Height / 2);
                
                if (nearLeft && !nearTop && !nearBottom)
                    return new Rectangle(workingArea.Left, workingArea.Top, workingArea.Width / 2, workingArea.Height);
                
                if (nearRight && !nearTop && !nearBottom)
                    return new Rectangle(workingArea.Left + workingArea.Width / 2, workingArea.Top, workingArea.Width / 2, workingArea.Height);
                
                if (nearTop && !nearLeft && !nearRight)
                    return new Rectangle(workingArea.Left, workingArea.Top, workingArea.Width, workingArea.Height / 2);
            }
            catch (Exception)
            {
                // Return null on any error
            }

            return null;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                HideSnapHints();
                _disposed = true;
            }
        }
    }
}