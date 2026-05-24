using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Layout;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Manages splitter drag operations and layout updates.
    /// Tracks drag state, enforces constraints, and applies layout changes.
    /// </summary>
    public class SplitterDragHandler : IDisposable
    {
        private PositioningUtilities.DragState _currentDragState;
        private DockingLayoutController _layoutController;
        private MdiPanelPositioner _positioner;
        private PanelWindowManager _panelManager;
        private WindowChrome _chrome;
        private DockLayoutTree _layoutTree;
        private Size _containerSize;
        private bool _disposed = false;

        /// <summary>
        /// Event raised when a drag starts.
        /// </summary>
        public event EventHandler<DragStartedEventArgs> DragStarted;

        /// <summary>
        /// Event raised when drag is updated.
        /// </summary>
        public event EventHandler<DragUpdatedEventArgs> DragUpdated;

        /// <summary>
        /// Event raised when a drag completes.
        /// </summary>
        public event EventHandler<DragCompletedEventArgs> DragCompleted;

        /// <summary>
        /// Initializes the drag handler.
        /// </summary>
        public SplitterDragHandler(
            DockingLayoutController layoutController,
            MdiPanelPositioner positioner,
            PanelWindowManager panelManager,
            WindowChrome chrome,
            DockLayoutTree layoutTree,
            Size containerSize)
        {
            _layoutController = layoutController ?? throw new ArgumentNullException(nameof(layoutController));
            _positioner = positioner ?? throw new ArgumentNullException(nameof(positioner));
            _panelManager = panelManager ?? throw new ArgumentNullException(nameof(panelManager));
            _chrome = chrome ?? throw new ArgumentNullException(nameof(chrome));
            _layoutTree = layoutTree ?? throw new ArgumentNullException(nameof(layoutTree));
            _containerSize = containerSize;

            _currentDragState = new PositioningUtilities.DragState();
        }

        /// <summary>
        /// Starts a drag operation from the given point.
        /// </summary>
        public void StartDrag(Point startPoint)
        {
            if (_currentDragState.IsDragging)
                return;  // Already dragging

            // Get the layout bounds for hit-testing
            var layout = _layoutController.CalculateLayout();
            var containerBounds = new Rectangle(0, 0, _containerSize.Width, _containerSize.Height);

            // Find the root group (simplified for now)
            var rootGroup = _layoutTree.Root;
            if (rootGroup == null)
            {
                Debug.WriteLine("[SplitterDragHandler] No root group found");
                return;
            }

            // Hit-test splitters
            var splitterResult = HitTestAllSplitters(startPoint, rootGroup, containerBounds);
            if (!splitterResult.HitSplitter)
            {
                Debug.WriteLine($"[SplitterDragHandler] No splitter hit at {startPoint}");
                return;
            }

            // Initialize drag state
            _currentDragState.IsDragging = true;
            _currentDragState.StartPosition = startPoint;
            _currentDragState.CurrentPosition = startPoint;
            _currentDragState.SplitterInfo = splitterResult;

            // Update cursor
            PositioningUtilities.UpdateCursorForSplitter(
                splitterResult.SplitterOrientation,
                true
            );

            Debug.WriteLine(
                $"[SplitterDragHandler] Drag started at {startPoint}, " +
                $"splitter: {splitterResult.SplitterOrientation}"
            );

            DragStarted?.Invoke(this, new DragStartedEventArgs
            {
                StartPoint = startPoint,
                SplitterOrientation = splitterResult.SplitterOrientation,
                SplitterBounds = splitterResult.SplitterBounds
            });
        }

        /// <summary>
        /// Updates drag operation with new mouse position.
        /// </summary>
        public void UpdateDrag(Point currentPoint)
        {
            if (!_currentDragState.IsDragging)
                return;

            _currentDragState.CurrentPosition = currentPoint;

            // Calculate new sizes based on drag
            PositioningUtilities.CalculateDragResult(_currentDragState, _containerSize);

            Debug.WriteLine(
                $"[SplitterDragHandler] Drag updated: " +
                $"left/top={_currentDragState.NewLeftOrTopSize}, " +
                $"right/bottom={_currentDragState.NewRightOrBottomSize}"
            );

            DragUpdated?.Invoke(this, new DragUpdatedEventArgs
            {
                CurrentPoint = currentPoint,
                DragDelta = _currentDragState.GetDragDelta(),
                NewLeftOrTopSize = _currentDragState.NewLeftOrTopSize,
                NewRightOrBottomSize = _currentDragState.NewRightOrBottomSize
            });
        }

        /// <summary>
        /// Completes a drag operation and applies layout changes.
        /// </summary>
        public void EndDrag(Point endPoint)
        {
            if (!_currentDragState.IsDragging)
                return;

            _currentDragState.CurrentPosition = endPoint;

            // Calculate final sizes
            PositioningUtilities.CalculateDragResult(_currentDragState, _containerSize);

            // Compute new split ratio
            int newRatio = PositioningUtilities.CalculateSplitRatio(
                _currentDragState.NewLeftOrTopSize,
                _currentDragState.NewLeftOrTopSize + PositioningUtilities.SPLITTER_WIDTH +
                _currentDragState.NewRightOrBottomSize
            );

            Debug.WriteLine(
                $"[SplitterDragHandler] Drag ended at {endPoint}, " +
                $"new split ratio: {newRatio}%"
            );

            // Apply the layout change
            // This would need to update the group's split ratio and recalculate
            ApplySplitRatioChange(newRatio);

            // Restore cursor
            PositioningUtilities.UpdateCursorForSplitter(
                _currentDragState.SplitterInfo.SplitterOrientation,
                false
            );

            // Raise event before clearing
            DragCompleted?.Invoke(this, new DragCompletedEventArgs
            {
                StartPoint = _currentDragState.StartPosition,
                EndPoint = endPoint,
                DragDelta = _currentDragState.GetDragDelta(),
                NewSplitRatio = newRatio
            });

            _currentDragState.Reset();
        }

        /// <summary>
        /// Applies a split ratio change and recalculates layout.
        /// </summary>
        private void ApplySplitRatioChange(int newRatio)
        {
            if (_currentDragState.SplitterInfo == null)
                return;

            // Find the group associated with this splitter
            var splitterId = _currentDragState.SplitterInfo.SplitterId;

            // Update the split ratio in the layout tree
            // This is simplified - in a full implementation, we'd traverse to find the group
            var allGroups = _layoutTree.GetAllGroups();
            var targetGroup = allGroups.FirstOrDefault(g => g.Id == splitterId);

            if (targetGroup != null)
            {
                targetGroup.SplitRatio = newRatio;
                Debug.WriteLine($"[SplitterDragHandler] Updated split ratio for group '{splitterId}' to {newRatio}%");
            }

            // Recalculate the entire layout with new ratio
            _layoutController.CalculateLayout();
        }

        /// <summary>
        /// Hits-tests all splitters in the group hierarchy.
        /// </summary>
        private PositioningUtilities.SplitterHitTestResult HitTestAllSplitters(
            Point point,
            DockGroup group,
            Rectangle bounds)
        {
            if (group == null || group.Children.Count == 0)
                return new PositioningUtilities.SplitterHitTestResult { HitSplitter = false };

            // Test splitter between left/right or top/bottom
            var orientation = group.SplitOrientation;
            var result = PositioningUtilities.HitTestSplitter(
                point,
                bounds,
                orientation == SplitOrientation.Vertical ? Orientation.Vertical : Orientation.Horizontal,
                (int)group.SplitRatio,
                group.Id
            );

            if (result.HitSplitter)
                return result;

            // Recursively test child groups
            int childIndex = 0;
            foreach (var child in group.Children.OfType<DockGroup>())
            {
                Rectangle childBounds;
                if (orientation == SplitOrientation.Vertical)
                {
                    int splitX = bounds.X + (int)(bounds.Width * (group.SplitRatio / 100.0));
                    if (childIndex == 0)
                    {
                        childBounds = new Rectangle(
                            bounds.X, bounds.Y,
                            splitX - bounds.X, bounds.Height
                        );
                    }
                    else
                    {
                        childBounds = new Rectangle(
                            splitX + PositioningUtilities.SPLITTER_WIDTH, bounds.Y,
                            bounds.Right - (splitX + PositioningUtilities.SPLITTER_WIDTH), bounds.Height
                        );
                    }
                }
                else
                {
                    int splitY = bounds.Y + (int)(bounds.Height * (group.SplitRatio / 100.0));
                    if (childIndex == 0)
                    {
                        childBounds = new Rectangle(
                            bounds.X, bounds.Y,
                            bounds.Width, splitY - bounds.Y
                        );
                    }
                    else
                    {
                        childBounds = new Rectangle(
                            bounds.X, splitY + PositioningUtilities.SPLITTER_HEIGHT,
                            bounds.Width, bounds.Bottom - (splitY + PositioningUtilities.SPLITTER_HEIGHT)
                        );
                    }
                }

                var childResult = HitTestAllSplitters(point, child, childBounds);
                if (childResult.HitSplitter)
                    return childResult;

                childIndex++;
            }

            return new PositioningUtilities.SplitterHitTestResult { HitSplitter = false };
        }

        /// <summary>
        /// Gets current drag state.
        /// </summary>
        public PositioningUtilities.DragState GetCurrentDragState() => _currentDragState;

        /// <summary>
        /// Gets diagnostic information.
        /// </summary>
        public SplitterDragDiagnostics GetDiagnostics()
        {
            return new SplitterDragDiagnostics
            {
                IsDragging = _currentDragState.IsDragging,
                DragStartPoint = _currentDragState.StartPosition,
                DragCurrentPoint = _currentDragState.CurrentPosition,
                DragDelta = _currentDragState.GetDragDelta(),
                CurrentSplitterOrientation = _currentDragState.SplitterInfo?.SplitterOrientation.ToString(),
                NewLeftOrTopSize = _currentDragState.NewLeftOrTopSize,
                NewRightOrBottomSize = _currentDragState.NewRightOrBottomSize
            };
        }

        /// <summary>
        /// Diagnostics result for drag handler.
        /// </summary>
        public class SplitterDragDiagnostics
        {
            public bool IsDragging { get; set; }
            public Point DragStartPoint { get; set; }
            public Point DragCurrentPoint { get; set; }
            public Point DragDelta { get; set; }
            public string CurrentSplitterOrientation { get; set; }
            public int NewLeftOrTopSize { get; set; }
            public int NewRightOrBottomSize { get; set; }
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            if (_currentDragState.IsDragging)
            {
                EndDrag(_currentDragState.CurrentPosition);
            }

            _disposed = true;
            Debug.WriteLine("[SplitterDragHandler] Disposed");
        }
    }

    /// <summary>
    /// Event arguments for drag started.
    /// </summary>
    public class DragStartedEventArgs : EventArgs
    {
        public Point StartPoint { get; set; }
        public Orientation SplitterOrientation { get; set; }
        public Rectangle SplitterBounds { get; set; }
    }

    /// <summary>
    /// Event arguments for drag updated.
    /// </summary>
    public class DragUpdatedEventArgs : EventArgs
    {
        public Point CurrentPoint { get; set; }
        public Point DragDelta { get; set; }
        public int NewLeftOrTopSize { get; set; }
        public int NewRightOrBottomSize { get; set; }
    }

    /// <summary>
    /// Event arguments for drag completed.
    /// </summary>
    public class DragCompletedEventArgs : EventArgs
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public Point DragDelta { get; set; }
        public int NewSplitRatio { get; set; }
    }
}
