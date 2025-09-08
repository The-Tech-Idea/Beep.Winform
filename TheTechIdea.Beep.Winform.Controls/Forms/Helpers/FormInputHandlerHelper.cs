using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Helper for handling form input events and drag operations.
    /// Provides centralized mouse handling and drag state management.
    /// </summary>
    internal class FormInputHandlerHelper
    {
        private readonly IBeepModernFormHost _host;
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private Point _formStartLocation;
        private DateTime _lastClickTime = DateTime.MinValue;
        private Point _lastClickLocation;
        private const int DoubleClickThreshold = 500; // milliseconds
        private const int DoubleClickDistance = 5; // pixels

        /// <summary>Gets whether the form is currently being dragged</summary>
        public bool IsDragging => _isDragging;

        /// <summary>Occurs when dragging begins</summary>
        public event EventHandler<DragEventArgs> DragStarted;

        /// <summary>Occurs during dragging</summary>
        public event EventHandler<DragEventArgs> DragMoved;

        /// <summary>Occurs when dragging ends</summary>
        public event EventHandler<DragEventArgs> DragEnded;

        /// <summary>Occurs on double-click in drag area</summary>
        public event EventHandler<MouseEventArgs> DoubleClickInDragArea;

        public FormInputHandlerHelper(IBeepModernFormHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        /// <summary>
        /// Handles mouse down events.
        /// </summary>
        public void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Check for double-click
                var now = DateTime.Now;
                var timeSinceLastClick = (now - _lastClickTime).TotalMilliseconds;
                var distanceFromLastClick = GetDistance(e.Location, _lastClickLocation);

                if (timeSinceLastClick < DoubleClickThreshold && distanceFromLastClick < DoubleClickDistance)
                {
                    // Double-click detected
                    DoubleClickInDragArea?.Invoke(this, e);
                    _lastClickTime = DateTime.MinValue; // Reset to prevent triple-click
                    return;
                }

                _lastClickTime = now;
                _lastClickLocation = e.Location;

                // Begin potential drag operation
                BeginDrag(e.Location);
            }
        }

        /// <summary>
        /// Handles mouse move events.
        /// </summary>
        public void OnMouseMove(MouseEventArgs e)
        {
            if (_isDragging && e.Button == MouseButtons.Left)
            {
                UpdateDrag(e.Location);
            }
        }

        /// <summary>
        /// Handles mouse up events.
        /// </summary>
        public void OnMouseUp(MouseEventArgs e)
        {
            if (_isDragging && e.Button == MouseButtons.Left)
            {
                EndDrag();
            }
        }

        /// <summary>
        /// Begins a drag operation from the specified point.
        /// </summary>
        public void BeginDrag(Point startPoint)
        {
            if (_isDragging)
                return;

            _isDragging = true;
            _dragStartPoint = startPoint;
            _formStartLocation = _host.AsForm.Location;

            DragStarted?.Invoke(this, new DragEventArgs
            {
                StartPoint = startPoint,
                CurrentPoint = startPoint,
                FormStartLocation = _formStartLocation,
                CurrentFormLocation = _host.AsForm.Location
            });
        }

        /// <summary>
        /// Updates the drag operation to the specified point.
        /// </summary>
        public void UpdateDrag(Point currentPoint)
        {
            if (!_isDragging)
                return;

            try
            {
                var deltaX = currentPoint.X - _dragStartPoint.X;
                var deltaY = currentPoint.Y - _dragStartPoint.Y;
                var newLocation = new Point(_formStartLocation.X + deltaX, _formStartLocation.Y + deltaY);

                _host.AsForm.Location = newLocation;

                DragMoved?.Invoke(this, new DragEventArgs
                {
                    StartPoint = _dragStartPoint,
                    CurrentPoint = currentPoint,
                    FormStartLocation = _formStartLocation,
                    CurrentFormLocation = newLocation
                });
            }
            catch (Exception)
            {
                // Ignore drag errors (form might be disposed, etc.)
                EndDrag();
            }
        }

        /// <summary>
        /// Ends the current drag operation.
        /// </summary>
        public void EndDrag()
        {
            if (!_isDragging)
                return;

            _isDragging = false;

            DragEnded?.Invoke(this, new DragEventArgs
            {
                StartPoint = _dragStartPoint,
                CurrentPoint = Point.Empty,
                FormStartLocation = _formStartLocation,
                CurrentFormLocation = _host.AsForm.Location
            });
        }

        /// <summary>
        /// Cancels any ongoing drag operation.
        /// </summary>
        public void CancelDrag()
        {
            if (!_isDragging)
                return;

            // Restore original form location
            try
            {
                _host.AsForm.Location = _formStartLocation;
            }
            catch
            {
                // Ignore restoration errors
            }

            EndDrag();
        }

        /// <summary>
        /// Calculates distance between two points.
        /// </summary>
        private static double GetDistance(Point p1, Point p2)
        {
            var dx = p1.X - p2.X;
            var dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Event arguments for drag operations.
        /// </summary>
        public class DragEventArgs : EventArgs
        {
            public Point StartPoint { get; set; }
            public Point CurrentPoint { get; set; }
            public Point FormStartLocation { get; set; }
            public Point CurrentFormLocation { get; set; }
        }
    }
}