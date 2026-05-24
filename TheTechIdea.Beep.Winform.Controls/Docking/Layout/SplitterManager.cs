using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Layout
{
    /// <summary>
    /// Manages splitter drag operations and interactive layout adjustments.
    /// Handles drag detection, ratio calculation, live preview, and final commit.
    /// </summary>
    public class SplitterManager
    {
        private readonly DockingLayoutController _layoutController;
        private readonly LayoutCalculator _calculator;
        private readonly IDockingPainter _painter;

        // Drag state
        private string _draggingSplitterId = null;
        private Point _dragStartPoint = Point.Empty;
        private float _dragStartRatio = 0;
        private Rectangle _previewBounds = Rectangle.Empty;
        private bool _isDragging = false;

        // Configuration
        private const int SPLITTER_GRAB_TOLERANCE = 4;
        private const int DRAG_THRESHOLD_PIXELS = 2;  // Pixels to move before drag is recognized

        public SplitterManager(DockingLayoutController layoutController, IDockingPainter painter)
        {
            _layoutController = layoutController ?? throw new ArgumentNullException(nameof(layoutController));
            _painter = painter ?? throw new ArgumentNullException(nameof(painter));
            _calculator = new LayoutCalculator(50, 50, 4);
        }

        /// <summary>
        /// Gets whether a drag operation is currently in progress.
        /// </summary>
        public bool IsDragging => _isDragging;

        /// <summary>
        /// Gets the current drag preview bounds (for rendering visual feedback).
        /// </summary>
        public Rectangle PreviewBounds => _previewBounds;

        /// <summary>
        /// Attempts to start a splitter drag operation at the given point.
        /// Returns true if successfully started (point was on a splitter).
        /// </summary>
        public bool BeginDrag(Point point)
        {
            if (_isDragging)
                return false;

            // Find splitter at point
            var splitterId = FindSplitterAtPoint(point);
            if (string.IsNullOrEmpty(splitterId))
                return false;

            _draggingSplitterId = splitterId;
            _dragStartPoint = point;
            _isDragging = false;  // Wait for threshold before actual drag

            return true;
        }

        /// <summary>
        /// Updates drag in progress. Returns true if layout preview changed.
        /// </summary>
        public bool UpdateDrag(Point currentPoint)
        {
            if (string.IsNullOrEmpty(_draggingSplitterId))
                return false;

            // Check if drag has moved enough to trigger actual drag
            if (!_isDragging)
            {
                int dx = Math.Abs(currentPoint.X - _dragStartPoint.X);
                int dy = Math.Abs(currentPoint.Y - _dragStartPoint.Y);

                if (dx + dy < DRAG_THRESHOLD_PIXELS)
                    return false;

                _isDragging = true;
                _dragStartRatio = GetSplitterRatio(_draggingSplitterId);
            }

            // Calculate preview bounds based on current drag position
            UpdatePreviewBounds(currentPoint);
            return true;
        }

        /// <summary>
        /// Commits the current drag operation, applying the new layout.
        /// </summary>
        public void EndDrag()
        {
            if (!_isDragging || string.IsNullOrEmpty(_draggingSplitterId))
            {
                CancelDrag();
                return;
            }

            // Apply layout change
            CommitDragLayout();

            CancelDrag();
        }

        /// <summary>
        /// Cancels the current drag operation without applying changes.
        /// </summary>
        public void CancelDrag()
        {
            _draggingSplitterId = null;
            _dragStartPoint = Point.Empty;
            _dragStartRatio = 0;
            _previewBounds = Rectangle.Empty;
            _isDragging = false;
        }

        /// <summary>
        /// Draws the drag preview (semi-transparent guide rectangle).
        /// </summary>
        public void DrawDragPreview(Graphics graphics)
        {
            if (!_isDragging || _previewBounds.IsEmpty)
                return;

            // Semi-transparent blue guide
            using (var guideBrush = new SolidBrush(Color.FromArgb(64, 0, 122, 255)))
            {
                graphics.FillRectangle(guideBrush, _previewBounds);
            }

            // Border
            using (var guidePen = new Pen(Color.FromArgb(0, 122, 255), 2f))
            {
                graphics.DrawRectangle(guidePen, _previewBounds);
            }
        }

        /// <summary>
        /// Gets diagnostic information about current splitter state.
        /// </summary>
        public SplitterDiagnostics GetDiagnostics()
        {
            return new SplitterDiagnostics
            {
                IsDragging = _isDragging,
                DraggingSplitterId = _draggingSplitterId,
                DragStartPoint = _dragStartPoint,
                CurrentRatio = _isDragging ? GetSplitterRatio(_draggingSplitterId) : _dragStartRatio,
                PreviewBounds = _previewBounds
            };
        }

        #region Private Methods

        /// <summary>
        /// Finds splitter at the given point.
        /// </summary>
        private string FindSplitterAtPoint(Point point)
        {
            // TODO: Implement by checking all group splitters
            // For now, return null (will be populated in integration with layout controller)
            return null;
        }

        /// <summary>
        /// Gets the current split ratio for a splitter/group.
        /// </summary>
        private float GetSplitterRatio(string splitterId)
        {
            // TODO: Lookup ratio from layout tree
            return 0.5f;
        }

        /// <summary>
        /// Updates preview bounds based on current drag position.
        /// </summary>
        private void UpdatePreviewBounds(Point currentPoint)
        {
            if (string.IsNullOrEmpty(_draggingSplitterId))
                return;

            int dragDelta = GetDragDelta(currentPoint);

            // Calculate new ratio based on drag
            // TODO: Implement based on splitter orientation

            // Update preview bounds accordingly
            // _previewBounds = CalculatePreviewBounds(dragDelta);
        }

        /// <summary>
        /// Gets the drag delta from start point to current point.
        /// </summary>
        private int GetDragDelta(Point currentPoint)
        {
            // For vertical splitter, use X delta; for horizontal, use Y delta
            // TODO: Determine orientation and return appropriate delta
            return 0;
        }

        /// <summary>
        /// Commits the drag layout changes.
        /// </summary>
        private void CommitDragLayout()
        {
            if (string.IsNullOrEmpty(_draggingSplitterId))
                return;

            int dragDelta = GetDragDelta(Point.Empty);  // TODO: Use current mouse point
            bool isVertical = true;  // TODO: Determine from splitter type

            _layoutController.DragSplitter(_draggingSplitterId, dragDelta, isVertical);
        }

        #endregion
    }

    /// <summary>
    /// Diagnostic information about splitter drag state.
    /// </summary>
    public class SplitterDiagnostics
    {
        public bool IsDragging { get; set; }
        public string DraggingSplitterId { get; set; }
        public Point DragStartPoint { get; set; }
        public float CurrentRatio { get; set; }
        public Rectangle PreviewBounds { get; set; }
    }
}
