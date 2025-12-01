using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Smart positioning and collision detection for tooltips
    /// Ensures tooltips never go off-screen and intelligently reposition
    /// Based on Popper.js and Material-UI positioning algorithms
    /// </summary>
    public static class ToolTipPositioningHelpers
    {
        /// <summary>
        /// Minimum distance from screen edges (in pixels)
        /// </summary>
        private const int ScreenEdgePadding = 8;

        /// <summary>
        /// Calculate optimal placement for tooltip based on available screen space
        /// Automatically finds the best placement to avoid collisions
        /// </summary>
        public static ToolTipPlacement CalculateOptimalPlacement(
            Rectangle targetRect,
            Size tooltipSize,
            ToolTipPlacement preferredPlacement,
            int offset = 8)
        {
            if (preferredPlacement != ToolTipPlacement.Auto)
            {
                // Check if preferred placement fits
                var testBounds = CalculateBoundsForPlacement(targetRect, tooltipSize, preferredPlacement, offset);
                var screenBounds = GetScreenBounds(targetRect.Location);
                
                if (IsFullyVisible(testBounds, screenBounds))
                {
                    return preferredPlacement;
                }
            }

            // Try all placements and find the best one
            var placements = new[]
            {
                ToolTipPlacement.Bottom,
                ToolTipPlacement.Top,
                ToolTipPlacement.Right,
                ToolTipPlacement.Left,
                ToolTipPlacement.BottomStart,
                ToolTipPlacement.BottomEnd,
                ToolTipPlacement.TopStart,
                ToolTipPlacement.TopEnd,
                ToolTipPlacement.RightStart,
                ToolTipPlacement.RightEnd,
                ToolTipPlacement.LeftStart,
                ToolTipPlacement.LeftEnd
            };

            ToolTipPlacement bestPlacement = ToolTipPlacement.Bottom;
            int bestScore = int.MinValue;

            foreach (var placement in placements)
            {
                var bounds = CalculateBoundsForPlacement(targetRect, tooltipSize, placement, offset);
                var screenBounds = GetScreenBounds(targetRect.Location);
                var score = CalculatePlacementScore(bounds, screenBounds, targetRect);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPlacement = placement;
                }
            }

            return bestPlacement;
        }

        /// <summary>
        /// Calculate tooltip bounds for a specific placement
        /// </summary>
        public static Rectangle CalculateBoundsForPlacement(
            Rectangle targetRect,
            Size tooltipSize,
            ToolTipPlacement placement,
            int offset = 8)
        {
            int x = 0, y = 0;

            switch (placement)
            {
                case ToolTipPlacement.Top:
                    x = targetRect.Left + (targetRect.Width - tooltipSize.Width) / 2;
                    y = targetRect.Top - tooltipSize.Height - offset;
                    break;

                case ToolTipPlacement.TopStart:
                    x = targetRect.Left;
                    y = targetRect.Top - tooltipSize.Height - offset;
                    break;

                case ToolTipPlacement.TopEnd:
                    x = targetRect.Right - tooltipSize.Width;
                    y = targetRect.Top - tooltipSize.Height - offset;
                    break;

                case ToolTipPlacement.Bottom:
                    x = targetRect.Left + (targetRect.Width - tooltipSize.Width) / 2;
                    y = targetRect.Bottom + offset;
                    break;

                case ToolTipPlacement.BottomStart:
                    x = targetRect.Left;
                    y = targetRect.Bottom + offset;
                    break;

                case ToolTipPlacement.BottomEnd:
                    x = targetRect.Right - tooltipSize.Width;
                    y = targetRect.Bottom + offset;
                    break;

                case ToolTipPlacement.Left:
                    x = targetRect.Left - tooltipSize.Width - offset;
                    y = targetRect.Top + (targetRect.Height - tooltipSize.Height) / 2;
                    break;

                case ToolTipPlacement.LeftStart:
                    x = targetRect.Left - tooltipSize.Width - offset;
                    y = targetRect.Top;
                    break;

                case ToolTipPlacement.LeftEnd:
                    x = targetRect.Left - tooltipSize.Width - offset;
                    y = targetRect.Bottom - tooltipSize.Height;
                    break;

                case ToolTipPlacement.Right:
                    x = targetRect.Right + offset;
                    y = targetRect.Top + (targetRect.Height - tooltipSize.Height) / 2;
                    break;

                case ToolTipPlacement.RightStart:
                    x = targetRect.Right + offset;
                    y = targetRect.Top;
                    break;

                case ToolTipPlacement.RightEnd:
                    x = targetRect.Right + offset;
                    y = targetRect.Bottom - tooltipSize.Height;
                    break;

                default: // Auto or unknown
                    x = targetRect.Left + (targetRect.Width - tooltipSize.Width) / 2;
                    y = targetRect.Bottom + offset;
                    break;
            }

            return new Rectangle(x, y, tooltipSize.Width, tooltipSize.Height);
        }

        /// <summary>
        /// Adjust position to ensure tooltip stays within screen bounds
        /// </summary>
        public static Point AdjustForScreenEdges(
            Rectangle tooltipBounds,
            Point targetPosition)
        {
            var screenBounds = GetScreenBounds(targetPosition);
            var adjustedBounds = tooltipBounds;

            // Adjust horizontally
            if (adjustedBounds.Left < screenBounds.Left + ScreenEdgePadding)
            {
                adjustedBounds.X = screenBounds.Left + ScreenEdgePadding;
            }
            else if (adjustedBounds.Right > screenBounds.Right - ScreenEdgePadding)
            {
                adjustedBounds.X = screenBounds.Right - adjustedBounds.Width - ScreenEdgePadding;
            }

            // Adjust vertically
            if (adjustedBounds.Top < screenBounds.Top + ScreenEdgePadding)
            {
                adjustedBounds.Y = screenBounds.Top + ScreenEdgePadding;
            }
            else if (adjustedBounds.Bottom > screenBounds.Bottom - ScreenEdgePadding)
            {
                adjustedBounds.Y = screenBounds.Bottom - adjustedBounds.Height - ScreenEdgePadding;
            }

            return adjustedBounds.Location;
        }

        /// <summary>
        /// Detect if tooltip would collide with screen edges
        /// </summary>
        public static bool DetectCollisions(
            Rectangle tooltipBounds,
            Rectangle screenBounds)
        {
            return !IsFullyVisible(tooltipBounds, screenBounds);
        }

        /// <summary>
        /// Check if tooltip is fully visible within screen bounds
        /// </summary>
        public static bool IsFullyVisible(
            Rectangle tooltipBounds,
            Rectangle screenBounds)
        {
            return tooltipBounds.Left >= screenBounds.Left + ScreenEdgePadding &&
                   tooltipBounds.Right <= screenBounds.Right - ScreenEdgePadding &&
                   tooltipBounds.Top >= screenBounds.Top + ScreenEdgePadding &&
                   tooltipBounds.Bottom <= screenBounds.Bottom - ScreenEdgePadding;
        }

        /// <summary>
        /// Find the best placement that fits on screen
        /// Returns the placement and adjusted position
        /// </summary>
        public static (ToolTipPlacement placement, Point position) FindBestPlacement(
            Rectangle targetRect,
            Size tooltipSize,
            ToolTipPlacement preferredPlacement = ToolTipPlacement.Auto,
            int offset = 8)
        {
            // Calculate optimal placement
            var placement = CalculateOptimalPlacement(targetRect, tooltipSize, preferredPlacement, offset);
            
            // Calculate bounds for this placement
            var bounds = CalculateBoundsForPlacement(targetRect, tooltipSize, placement, offset);
            
            // Adjust for screen edges
            var adjustedPosition = AdjustForScreenEdges(bounds, targetRect.Location);
            
            return (placement, adjustedPosition);
        }

        /// <summary>
        /// Get screen bounds for a given point (handles multi-monitor setups)
        /// </summary>
        public static Rectangle GetScreenBounds(Point point)
        {
            try
            {
                var screen = Screen.FromPoint(point);
                return screen.WorkingArea;
            }
            catch
            {
                // Fallback to primary screen
                return Screen.PrimaryScreen.WorkingArea;
            }
        }

        /// <summary>
        /// Calculate a score for a placement based on visibility and distance from target
        /// Higher score = better placement
        /// </summary>
        private static int CalculatePlacementScore(
            Rectangle tooltipBounds,
            Rectangle screenBounds,
            Rectangle targetRect)
        {
            int score = 0;

            // Check if fully visible
            if (IsFullyVisible(tooltipBounds, screenBounds))
            {
                score += 1000; // Big bonus for fully visible
            }
            else
            {
                // Calculate how much is visible
                var visibleArea = Rectangle.Intersect(tooltipBounds, screenBounds);
                if (!visibleArea.IsEmpty)
                {
                    var visibleRatio = (double)(visibleArea.Width * visibleArea.Height) / (tooltipBounds.Width * tooltipBounds.Height);
                    score += (int)(visibleRatio * 500); // Partial visibility score
                }
            }

            // Prefer placements closer to target center
            var targetCenter = new Point(
                targetRect.Left + targetRect.Width / 2,
                targetRect.Top + targetRect.Height / 2);
            var tooltipCenter = new Point(
                tooltipBounds.Left + tooltipBounds.Width / 2,
                tooltipBounds.Top + tooltipBounds.Height / 2);

            var distance = Math.Sqrt(
                Math.Pow(targetCenter.X - tooltipCenter.X, 2) +
                Math.Pow(targetCenter.Y - tooltipCenter.Y, 2));

            // Closer is better (subtract distance, but not too much)
            score -= (int)(distance / 10);

            // Prefer bottom placement (most common)
            if (tooltipBounds.Top > targetRect.Bottom)
            {
                score += 50;
            }

            return score;
        }

        /// <summary>
        /// Calculate responsive size for tooltip based on content and screen size
        /// </summary>
        public static Size CalculateResponsiveSize(
            Size contentSize,
            Size maxSize,
            Size minSize,
            Rectangle screenBounds)
        {
            // Start with content size
            var size = contentSize;

            // Apply max size constraint (80% of screen width, or specified max)
            var maxWidth = Math.Min(
                maxSize.Width > 0 ? maxSize.Width : int.MaxValue,
                (int)(screenBounds.Width * 0.8));
            var maxHeight = Math.Min(
                maxSize.Height > 0 ? maxSize.Height : int.MaxValue,
                (int)(screenBounds.Height * 0.8));

            // Apply min size constraint
            var minWidth = minSize.Width > 0 ? minSize.Width : 120;
            var minHeight = minSize.Height > 0 ? minSize.Height : 40;

            // Clamp to constraints
            size.Width = Math.Max(minWidth, Math.Min(size.Width, maxWidth));
            size.Height = Math.Max(minHeight, Math.Min(size.Height, maxHeight));

            return size;
        }

        /// <summary>
        /// Calculate optimal position for tooltip with arrow adjustment
        /// Adjusts position to keep arrow pointing at target when repositioned
        /// </summary>
        public static Point CalculatePositionWithArrow(
            Rectangle targetRect,
            Size tooltipSize,
            ToolTipPlacement placement,
            int arrowSize,
            int offset = 8)
        {
            var bounds = CalculateBoundsForPlacement(targetRect, tooltipSize, placement, offset);
            var screenBounds = GetScreenBounds(targetRect.Location);
            
            // Adjust for screen edges
            var adjustedBounds = bounds;
            
            // If we had to adjust horizontally, we might need to adjust arrow position
            // For now, just ensure tooltip stays on screen
            if (adjustedBounds.Left < screenBounds.Left + ScreenEdgePadding)
            {
                adjustedBounds.X = screenBounds.Left + ScreenEdgePadding;
            }
            else if (adjustedBounds.Right > screenBounds.Right - ScreenEdgePadding)
            {
                adjustedBounds.X = screenBounds.Right - adjustedBounds.Width - ScreenEdgePadding;
            }

            if (adjustedBounds.Top < screenBounds.Top + ScreenEdgePadding)
            {
                adjustedBounds.Y = screenBounds.Top + ScreenEdgePadding;
            }
            else if (adjustedBounds.Bottom > screenBounds.Bottom - ScreenEdgePadding)
            {
                adjustedBounds.Y = screenBounds.Bottom - adjustedBounds.Height - ScreenEdgePadding;
            }

            return adjustedBounds.Location;
        }
    }
}

