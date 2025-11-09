using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Helpers
{
    /// <summary>
    /// Helper class for dock hit testing
    /// Provides point-to-item mapping, bounds checking, and hover detection
    /// </summary>
    public static class DockHitTestHelper
    {
        /// <summary>
        /// Finds the dock item at the specified point
        /// </summary>
        /// <param name="point">The point to test</param>
        /// <param name="itemStates">Collection of dock item states</param>
        /// <returns>The index of the item at the point, or -1 if none</returns>
        public static int HitTest(Point point, List<DockItemState> itemStates)
        {
            if (itemStates == null || itemStates.Count == 0)
                return -1;

            // Check in reverse order so items drawn on top are tested first
            for (int i = itemStates.Count - 1; i >= 0; i--)
            {
                var state = itemStates[i];
                if (state.HitBounds.Contains(point))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Finds the dock item at the specified point, using primary bounds
        /// </summary>
        public static int HitTestBounds(Point point, List<DockItemState> itemStates)
        {
            if (itemStates == null || itemStates.Count == 0)
                return -1;

            for (int i = itemStates.Count - 1; i >= 0; i--)
            {
                var state = itemStates[i];
                if (state.Bounds.Contains(point))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Calculates expanded hit bounds for an item to improve touch targeting
        /// </summary>
        public static Rectangle CalculateHitBounds(Rectangle itemBounds, int expansionSize)
        {
            var hitBounds = itemBounds;
            hitBounds.Inflate(expansionSize, expansionSize);
            return hitBounds;
        }

        /// <summary>
        /// Updates hit bounds for all items
        /// </summary>
        public static void UpdateHitBounds(List<DockItemState> itemStates, int expansionSize)
        {
            if (itemStates == null)
                return;

            foreach (var state in itemStates)
            {
                state.HitBounds = CalculateHitBounds(state.Bounds, expansionSize);
            }
        }

        /// <summary>
        /// Finds items within a specific region
        /// </summary>
        public static List<int> HitTestRegion(Rectangle region, List<DockItemState> itemStates)
        {
            var results = new List<int>();

            if (itemStates == null || itemStates.Count == 0)
                return results;

            for (int i = 0; i < itemStates.Count; i++)
            {
                var state = itemStates[i];
                if (region.IntersectsWith(state.Bounds))
                    results.Add(i);
            }

            return results;
        }

        /// <summary>
        /// Calculates the distance from a point to the center of an item
        /// </summary>
        public static float DistanceToItem(Point point, DockItemState itemState)
        {
            if (itemState == null)
                return float.MaxValue;

            var bounds = itemState.Bounds;
            int centerX = bounds.X + bounds.Width / 2;
            int centerY = bounds.Y + bounds.Height / 2;

            int dx = point.X - centerX;
            int dy = point.Y - centerY;

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Finds the closest item to a point
        /// </summary>
        public static int FindClosestItem(Point point, List<DockItemState> itemStates)
        {
            if (itemStates == null || itemStates.Count == 0)
                return -1;

            int closestIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < itemStates.Count; i++)
            {
                float distance = DistanceToItem(point, itemStates[i]);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        /// <summary>
        /// Checks if a point is within the dock bounds with a margin
        /// </summary>
        public static bool IsWithinDockBounds(Point point, Rectangle dockBounds, int margin)
        {
            var expandedBounds = dockBounds;
            expandedBounds.Inflate(margin, margin);
            return expandedBounds.Contains(point);
        }

        /// <summary>
        /// Calculates hover influence for magnification effect
        /// Returns a value from 0 (no influence) to 1 (maximum influence)
        /// </summary>
        public static float CalculateHoverInfluence(int itemIndex, int hoveredIndex, int influenceRange)
        {
            if (hoveredIndex < 0 || itemIndex < 0)
                return 0f;

            int distance = Math.Abs(itemIndex - hoveredIndex);

            if (distance == 0)
                return 1.0f; // Full influence on hovered item

            if (distance > influenceRange)
                return 0f; // No influence beyond range

            // Linear falloff based on distance
            return 1.0f - ((float)distance / (influenceRange + 1));
        }

        /// <summary>
        /// Determines the drop position when dragging items
        /// </summary>
        public static int CalculateDropIndex(Point point, List<DockItemState> itemStates, DockOrientation orientation)
        {
            if (itemStates == null || itemStates.Count == 0)
                return 0;

            // Find insertion point based on orientation
            for (int i = 0; i < itemStates.Count; i++)
            {
                var bounds = itemStates[i].Bounds;
                
                if (orientation == DockOrientation.Horizontal)
                {
                    // Check if point is before the center of this item
                    int centerX = bounds.X + bounds.Width / 2;
                    if (point.X < centerX)
                        return i;
                }
                else
                {
                    // Check if point is before the center of this item
                    int centerY = bounds.Y + bounds.Height / 2;
                    if (point.Y < centerY)
                        return i;
                }
            }

            // Point is after all items
            return itemStates.Count;
        }

        /// <summary>
        /// Checks if two rectangles overlap
        /// </summary>
        public static bool Overlaps(Rectangle rect1, Rectangle rect2)
        {
            return rect1.IntersectsWith(rect2);
        }

        /// <summary>
        /// Calculates the overlap area between two rectangles
        /// </summary>
        public static Rectangle GetOverlapArea(Rectangle rect1, Rectangle rect2)
        {
            int x1 = Math.Max(rect1.Left, rect2.Left);
            int y1 = Math.Max(rect1.Top, rect2.Top);
            int x2 = Math.Min(rect1.Right, rect2.Right);
            int y2 = Math.Min(rect1.Bottom, rect2.Bottom);

            if (x2 > x1 && y2 > y1)
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);

            return Rectangle.Empty;
        }

        /// <summary>
        /// Checks if a drag operation is starting (moved beyond threshold)
        /// </summary>
        public static bool IsDragStarting(Point startPoint, Point currentPoint, int dragThreshold)
        {
            int dx = Math.Abs(currentPoint.X - startPoint.X);
            int dy = Math.Abs(currentPoint.Y - startPoint.Y);

            return dx > dragThreshold || dy > dragThreshold;
        }

        /// <summary>
        /// Creates a region that includes all dock items
        /// </summary>
        public static Rectangle GetItemsBounds(List<DockItemState> itemStates)
        {
            if (itemStates == null || itemStates.Count == 0)
                return Rectangle.Empty;

            var bounds = itemStates[0].Bounds;

            foreach (var state in itemStates.Skip(1))
            {
                bounds = Rectangle.Union(bounds, state.Bounds);
            }

            return bounds;
        }

        /// <summary>
        /// Checks if the cursor is hovering over the dock area
        /// Used for auto-hide functionality
        /// </summary>
        public static bool IsHoveringDock(Point cursorPosition, Rectangle dockBounds, int hoverMargin)
        {
            var hoverBounds = dockBounds;
            hoverBounds.Inflate(hoverMargin, hoverMargin);
            return hoverBounds.Contains(cursorPosition);
        }

        /// <summary>
        /// Calculates the relative position of a point within an item
        /// Returns values from 0 to 1 for both X and Y
        /// </summary>
        public static PointF GetRelativePosition(Point point, Rectangle itemBounds)
        {
            if (itemBounds.Width == 0 || itemBounds.Height == 0)
                return new PointF(0.5f, 0.5f);

            float relativeX = (float)(point.X - itemBounds.X) / itemBounds.Width;
            float relativeY = (float)(point.Y - itemBounds.Y) / itemBounds.Height;

            return new PointF(
                Math.Max(0, Math.Min(1, relativeX)),
                Math.Max(0, Math.Min(1, relativeY))
            );
        }
    }
}
