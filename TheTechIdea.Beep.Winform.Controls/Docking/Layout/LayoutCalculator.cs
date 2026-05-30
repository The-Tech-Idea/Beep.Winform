using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Layout
{
    /// <summary>
    /// Helper class for common layout calculations and utilities.
    /// </summary>
    public class LayoutCalculator
    {
        private readonly int _minWidth;
        private readonly int _minHeight;
        private readonly int _splitterWidth;

        public LayoutCalculator(int minWidth, int minHeight, int splitterWidth)
        {
            _minWidth = Math.Max(1, minWidth);
            _minHeight = Math.Max(1, minHeight);
            _splitterWidth = Math.Max(1, splitterWidth);
        }

        /// <summary>
        /// Calculates a new split ratio based on drag delta.
        /// dragDelta: positive = increase first region, negative = decrease first region
        /// totalSize: total available size (width or height)
        /// </summary>
        public float CalculateNewRatio(float currentRatio, int dragDelta, int totalSize)
        {
            if (totalSize <= 0)
                return currentRatio;

            // dragDelta as percentage of total size
            float deltaPct = (float)dragDelta / totalSize;
            float newRatio = currentRatio + deltaPct;

            // Clamp to valid range [0.1, 0.9]
            return Math.Max(0.1f, Math.Min(0.9f, newRatio));
        }

        /// <summary>
        /// Clamps an edge allocation (in pixels) for a proportional split so neither the
        /// allocated region nor the remaining region falls below their minimum sizes.
        /// </summary>
        /// <param name="desired">Desired size from the ratio.</param>
        /// <param name="total">Total available size along the split axis (excluding the splitter).</param>
        /// <param name="minThis">Minimum size for the allocated region.</param>
        /// <param name="minOther">Minimum size for the remaining region.</param>
        public int ClampSplit(int desired, int total, int minThis, int minOther)
        {
            int lo = Math.Max(1, minThis);
            int hi = total - Math.Max(1, minOther);
            if (hi < lo) hi = lo;             // container too small to honor both: prefer this side's min
            return Math.Max(lo, Math.Min(hi, desired));
        }

        /// <summary>
        /// Converts a pixel delta on a splitter into a new ratio (0.1–0.9) for the given axis size.
        /// </summary>
        public float RatioFromDelta(float currentRatio, int dragDelta, int totalSize)
            => CalculateNewRatio(currentRatio, dragDelta, totalSize);

        /// <summary>
        /// Clamps a rectangle to a parent rectangle.
        /// </summary>
        public Rectangle ClampBounds(Rectangle bounds, Rectangle parentBounds)
        {
            var clamped = bounds;

            // Clamp position
            if (clamped.X < parentBounds.X)
                clamped.X = parentBounds.X;
            if (clamped.Y < parentBounds.Y)
                clamped.Y = parentBounds.Y;

            // Clamp size
            if (clamped.Right > parentBounds.Right)
                clamped.Width = parentBounds.Right - clamped.X;
            if (clamped.Bottom > parentBounds.Bottom)
                clamped.Height = parentBounds.Bottom - clamped.Y;

            // Ensure minimum size
            clamped.Width = Math.Max(_minWidth, clamped.Width);
            clamped.Height = Math.Max(_minHeight, clamped.Height);

            return clamped;
        }

        /// <summary>
        /// Checks if a point is within the grab zone of a splitter.
        /// splitterBounds: the calculated splitter bounds
        /// point: point to test (in container coordinates)
        /// grabTolerance: pixels around splitter that are "grabable"
        /// </summary>
        public bool IsPointOnSplitter(Rectangle splitterBounds, Point point, int grabTolerance = 4)
        {
            // Expand splitter bounds by grab tolerance
            var grabZone = splitterBounds;
            grabZone.Inflate(grabTolerance, grabTolerance);

            return grabZone.Contains(point);
        }

        /// <summary>
        /// Calculates splitter bounds for a vertical split (left/right).
        /// Used to determine the splitter region between two horizontally-adjacent groups.
        /// </summary>
        public Rectangle CalculateVerticalSplitterBounds(Rectangle leftGroupBounds, Rectangle rightGroupBounds)
        {
            return new Rectangle(
                leftGroupBounds.Right,
                Math.Min(leftGroupBounds.Y, rightGroupBounds.Y),
                _splitterWidth,
                Math.Max(leftGroupBounds.Height, rightGroupBounds.Height)
            );
        }

        /// <summary>
        /// Calculates splitter bounds for a horizontal split (top/bottom).
        /// Used to determine the splitter region between two vertically-adjacent groups.
        /// </summary>
        public Rectangle CalculateHorizontalSplitterBounds(Rectangle topGroupBounds, Rectangle bottomGroupBounds)
        {
            return new Rectangle(
                Math.Min(topGroupBounds.X, bottomGroupBounds.X),
                topGroupBounds.Bottom,
                Math.Max(topGroupBounds.Width, bottomGroupBounds.Width),
                _splitterWidth
            );
        }

        /// <summary>
        /// Checks if two rectangles overlap (excluding touching edges).
        /// </summary>
        public bool RectanglesOverlap(Rectangle a, Rectangle b)
        {
            return a.IntersectsWith(b) && 
                   a.X < b.Right && b.X < a.Right &&
                   a.Y < b.Bottom && b.Y < a.Bottom;
        }

        /// <summary>
        /// Gets the maximum usable size for a panel group.
        /// Accounts for tab strip height, chrome height, and minimum sizes.
        /// </summary>
        public Size GetMaxPanelGroupSize(Rectangle containerBounds, int tabStripHeight, int chromeHeight)
        {
            int maxWidth = containerBounds.Width - _minWidth;
            int maxHeight = containerBounds.Height - tabStripHeight - chromeHeight;

            return new Size(Math.Max(_minWidth, maxWidth), Math.Max(_minHeight, maxHeight));
        }

        /// <summary>
        /// Validates that a ratio is within acceptable bounds.
        /// </summary>
        public bool IsValidRatio(float ratio)
        {
            return ratio >= 0.1f && ratio <= 0.9f;
        }

        /// <summary>
        /// Clamps a ratio to valid range.
        /// </summary>
        public float ClampRatio(float ratio)
        {
            return Math.Max(0.1f, Math.Min(0.9f, ratio));
        }

        /// <summary>
        /// Calculates how much space is available after accounting for fixed elements.
        /// </summary>
        public int CalculateAvailableSpace(int containerSize, int numElements, int fixedElementSize)
        {
            int totalFixed = fixedElementSize * numElements;
            return Math.Max(0, containerSize - totalFixed);
        }

        /// <summary>
        /// Distributes available space equally among elements.
        /// </summary>
        public List<int> DistributeSpaceEqually(int containerSize, int numElements, int minElementSize)
        {
            var result = new List<int>();

            if (numElements <= 0)
                return result;

            int availableSize = containerSize;
            int elementSize = Math.Max(minElementSize, containerSize / numElements);

            for (int i = 0; i < numElements; i++)
            {
                result.Add(Math.Min(elementSize, availableSize));
                availableSize -= elementSize;
            }

            // Distribute any remainder to last element
            if (result.Count > 0 && availableSize > 0)
            {
                result[result.Count - 1] += availableSize;
            }

            return result;
        }

        /// <summary>
        /// Calculates the distance from a point to a line segment.
        /// Used for hit-testing splitters.
        /// </summary>
        public float DistanceToLine(Point point, Point lineStart, Point lineEnd)
        {
            // Calculate perpendicular distance from point to line
            float dx = lineEnd.X - lineStart.X;
            float dy = lineEnd.Y - lineStart.Y;

            if (dx == 0 && dy == 0)
                return float.MaxValue;

            float t = ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / (dx * dx + dy * dy);
            t = Math.Max(0, Math.Min(1, t));

            float closestX = lineStart.X + t * dx;
            float closestY = lineStart.Y + t * dy;

            float distX = point.X - closestX;
            float distY = point.Y - closestY;

            return (float)Math.Sqrt(distX * distX + distY * distY);
        }

        /// <summary>
        /// Gets diagnostic string for layout bounds.
        /// </summary>
        public string GetBoundsDiagnostic(Rectangle bounds)
        {
            return $"Bounds(X={bounds.X}, Y={bounds.Y}, W={bounds.Width}, H={bounds.Height})";
        }

        /// <summary>
        /// Gets diagnostic string for split ratio.
        /// </summary>
        public string GetRatioDiagnostic(float ratio)
        {
            int pct = (int)(ratio * 100);
            return $"Ratio({pct}% / {100 - pct}%)";
        }
    }
}
