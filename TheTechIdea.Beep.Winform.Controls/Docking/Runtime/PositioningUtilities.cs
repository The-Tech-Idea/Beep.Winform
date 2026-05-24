using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Utility methods for splitter drag operations and mouse tracking.
    /// Handles mouse interactions, cursor changes, and drag state management.
    /// </summary>
    public static class PositioningUtilities
    {
        /// <summary>
        /// Default splitter width in pixels.
        /// </summary>
        public const int SPLITTER_WIDTH = 4;

        /// <summary>
        /// Default splitter height in pixels.
        /// </summary>
        public const int SPLITTER_HEIGHT = 4;

        /// <summary>
        /// Hit test tolerance for splitter detection (pixels beyond splitter bounds).
        /// </summary>
        public const int SPLITTER_HIT_TOLERANCE = 2;

        /// <summary>
        /// Minimum size for a panel in pixels.
        /// </summary>
        public const int MIN_PANEL_SIZE = 50;

        /// <summary>
        /// Maximum size for a panel in pixels.
        /// </summary>
        public const int MAX_PANEL_SIZE = 5000;

        /// <summary>
        /// Represents the result of hit-testing against a splitter.
        /// </summary>
        public class SplitterHitTestResult
        {
            /// <summary>
            /// True if the hit point intersects a splitter.
            /// </summary>
            public bool HitSplitter { get; set; }

            /// <summary>
            /// The bounds of the splitter if hit.
            /// </summary>
            public Rectangle SplitterBounds { get; set; }

            /// <summary>
            /// The orientation of the splitter (vertical or horizontal).
            /// </summary>
            public Orientation SplitterOrientation { get; set; }

            /// <summary>
            /// Identifier for the splitter (e.g., group key or index).
            /// </summary>
            public string SplitterId { get; set; }

            /// <summary>
            /// The left/top panel bounds if split is vertical/horizontal.
            /// </summary>
            public Rectangle LeftOrTopPanel { get; set; }

            /// <summary>
            /// The right/bottom panel bounds if split is vertical/horizontal.
            /// </summary>
            public Rectangle RightOrBottomPanel { get; set; }
        }

        /// <summary>
        /// Represents a drag operation state.
        /// </summary>
        public class DragState
        {
            /// <summary>
            /// True if a drag operation is in progress.
            /// </summary>
            public bool IsDragging { get; set; }

            /// <summary>
            /// The starting position of the drag.
            /// </summary>
            public Point StartPosition { get; set; }

            /// <summary>
            /// The current position during the drag.
            /// </summary>
            public Point CurrentPosition { get; set; }

            /// <summary>
            /// The splitter being dragged.
            /// </summary>
            public SplitterHitTestResult SplitterInfo { get; set; }

            /// <summary>
            /// The new size for the left/top panel.
            /// </summary>
            public int NewLeftOrTopSize { get; set; }

            /// <summary>
            /// The new size for the right/bottom panel.
            /// </summary>
            public int NewRightOrBottomSize { get; set; }

            /// <summary>
            /// Calculates the drag delta from start to current position.
            /// </summary>
            public Point GetDragDelta() => new Point(
                CurrentPosition.X - StartPosition.X,
                CurrentPosition.Y - StartPosition.Y
            );

            /// <summary>
            /// Resets the drag state.
            /// </summary>
            public void Reset()
            {
                IsDragging = false;
                StartPosition = Point.Empty;
                CurrentPosition = Point.Empty;
                SplitterInfo = null;
                NewLeftOrTopSize = 0;
                NewRightOrBottomSize = 0;
            }
        }

        /// <summary>
        /// Detects if a point is over a splitter and returns hit test results.
        /// </summary>
        public static SplitterHitTestResult HitTestSplitter(
            Point point,
            Rectangle bounds,
            Orientation splitOrientation,
            int splitRatio,
            string splitterId)
        {
            var result = new SplitterHitTestResult
            {
                SplitterId = splitterId,
                SplitterOrientation = splitOrientation
            };

            if (splitOrientation == Orientation.Vertical)
            {
                // Vertical splitter - divides horizontally
                int splitX = bounds.X + (int)(bounds.Width * (splitRatio / 100.0));
                int splitterLeft = Math.Max(bounds.X, splitX - SPLITTER_HIT_TOLERANCE);
                int splitterRight = Math.Min(bounds.Right, splitX + SPLITTER_WIDTH + SPLITTER_HIT_TOLERANCE);

                if (point.X >= splitterLeft && point.X <= splitterRight && 
                    point.Y >= bounds.Y && point.Y <= bounds.Bottom)
                {
                    result.HitSplitter = true;
                    result.SplitterBounds = new Rectangle(splitX, bounds.Y, SPLITTER_WIDTH, bounds.Height);
                    result.LeftOrTopPanel = new Rectangle(bounds.X, bounds.Y, splitX - bounds.X, bounds.Height);
                    result.RightOrBottomPanel = new Rectangle(splitX + SPLITTER_WIDTH, bounds.Y,
                        bounds.Right - (splitX + SPLITTER_WIDTH), bounds.Height);
                }
            }
            else
            {
                // Horizontal splitter - divides vertically
                int splitY = bounds.Y + (int)(bounds.Height * (splitRatio / 100.0));
                int splitterTop = Math.Max(bounds.Y, splitY - SPLITTER_HIT_TOLERANCE);
                int splitterBottom = Math.Min(bounds.Bottom, splitY + SPLITTER_HEIGHT + SPLITTER_HIT_TOLERANCE);

                if (point.X >= bounds.X && point.X <= bounds.Right &&
                    point.Y >= splitterTop && point.Y <= splitterBottom)
                {
                    result.HitSplitter = true;
                    result.SplitterBounds = new Rectangle(bounds.X, splitY, bounds.Width, SPLITTER_HEIGHT);
                    result.LeftOrTopPanel = new Rectangle(bounds.X, bounds.Y, bounds.Width, splitY - bounds.Y);
                    result.RightOrBottomPanel = new Rectangle(bounds.X, splitY + SPLITTER_HEIGHT,
                        bounds.Width, bounds.Bottom - (splitY + SPLITTER_HEIGHT));
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the cursor based on the splitter orientation.
        /// </summary>
        public static void UpdateCursorForSplitter(
            Orientation splitOrientation,
            bool isHoveringOverSplitter)
        {
            if (!isHoveringOverSplitter)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            Cursor.Current = splitOrientation == Orientation.Vertical
                ? Cursors.VSplit
                : Cursors.HSplit;
        }

        /// <summary>
        /// Calculates new panel sizes during a splitter drag operation.
        /// </summary>
        public static void CalculateDragResult(
            DragState dragState,
            Size containerSize)
        {
            if (dragState?.SplitterInfo == null)
                return;

            Point delta = dragState.GetDragDelta();
            var splitterInfo = dragState.SplitterInfo;

            if (splitterInfo.SplitterOrientation == Orientation.Vertical)
            {
                // Vertical splitter - drag left/right
                int newLeftSize = Math.Max(MIN_PANEL_SIZE,
                    Math.Min(MAX_PANEL_SIZE, splitterInfo.LeftOrTopPanel.Width + delta.X));

                int newRightSize = Math.Max(MIN_PANEL_SIZE,
                    Math.Min(MAX_PANEL_SIZE, splitterInfo.RightOrBottomPanel.Width - delta.X));

                // Ensure we don't exceed container width
                int totalSize = newLeftSize + SPLITTER_WIDTH + newRightSize;
                if (totalSize > containerSize.Width)
                {
                    newRightSize = containerSize.Width - newLeftSize - SPLITTER_WIDTH;
                    newRightSize = Math.Max(MIN_PANEL_SIZE, newRightSize);
                }

                dragState.NewLeftOrTopSize = newLeftSize;
                dragState.NewRightOrBottomSize = newRightSize;
            }
            else
            {
                // Horizontal splitter - drag up/down
                int newTopSize = Math.Max(MIN_PANEL_SIZE,
                    Math.Min(MAX_PANEL_SIZE, splitterInfo.LeftOrTopPanel.Height + delta.Y));

                int newBottomSize = Math.Max(MIN_PANEL_SIZE,
                    Math.Min(MAX_PANEL_SIZE, splitterInfo.RightOrBottomPanel.Height - delta.Y));

                // Ensure we don't exceed container height
                int totalSize = newTopSize + SPLITTER_HEIGHT + newBottomSize;
                if (totalSize > containerSize.Height)
                {
                    newBottomSize = containerSize.Height - newTopSize - SPLITTER_HEIGHT;
                    newBottomSize = Math.Max(MIN_PANEL_SIZE, newBottomSize);
                }

                dragState.NewLeftOrTopSize = newTopSize;
                dragState.NewRightOrBottomSize = newBottomSize;
            }
        }

        /// <summary>
        /// Validates that a new panel size is within acceptable bounds.
        /// </summary>
        public static bool IsValidPanelSize(int size)
        {
            return size >= MIN_PANEL_SIZE && size <= MAX_PANEL_SIZE;
        }

        /// <summary>
        /// Clamps a value between minimum and maximum panel sizes.
        /// </summary>
        public static int ClampPanelSize(int size)
        {
            return Math.Max(MIN_PANEL_SIZE, Math.Min(MAX_PANEL_SIZE, size));
        }

        /// <summary>
        /// Calculates the split ratio (0-100) from panel sizes.
        /// </summary>
        public static int CalculateSplitRatio(int leftOrTopSize, int totalSize)
        {
            if (totalSize <= 0)
                return 50;

            int ratio = (int)((leftOrTopSize / (double)totalSize) * 100);
            return Math.Max(10, Math.Min(90, ratio));  // Keep ratio between 10% and 90%
        }

        /// <summary>
        /// Adjusts a rectangle to ensure minimum size requirements.
        /// </summary>
        public static Rectangle EnsureMinimumSize(Rectangle rect, Size minimumSize)
        {
            int width = Math.Max(rect.Width, minimumSize.Width);
            int height = Math.Max(rect.Height, minimumSize.Height);
            return new Rectangle(rect.X, rect.Y, width, height);
        }

        /// <summary>
        /// Adjusts a rectangle to fit within container bounds.
        /// </summary>
        public static Rectangle ClampToBounds(Rectangle rect, Rectangle containerBounds)
        {
            int x = Math.Max(containerBounds.X, Math.Min(rect.X, containerBounds.Right - rect.Width));
            int y = Math.Max(containerBounds.Y, Math.Min(rect.Y, containerBounds.Bottom - rect.Height));
            int width = Math.Min(rect.Width, containerBounds.Width);
            int height = Math.Min(rect.Height, containerBounds.Height);

            return new Rectangle(x, y, width, height);
        }
    }
}
