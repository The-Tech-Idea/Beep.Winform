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

                // FLIP, then let the caller SHIFT. An explicit request must not silently become an
                // unrelated side: a tooltip asked for Top that is clipped by 3px should slide 3px
                // along its edge, or at worst move to Bottom -- not jump to Right. Scoring all
                // twelve candidates (below) cannot express that, so it is now reserved for Auto.
                var flipped = GetOppositePlacement(preferredPlacement);
                if (flipped != preferredPlacement)
                {
                    var flippedBounds = CalculateBoundsForPlacement(targetRect, tooltipSize, flipped, offset);
                    if (FitsOnPrimaryAxis(flippedBounds, screenBounds, flipped))
                    {
                        return flipped;
                    }
                }

                // Neither side fits on its primary axis; keep what was asked for and let the
                // shift + clamp stage do what it can.
                return preferredPlacement;
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
        /// Resolve a placement and final position for an anchor rectangle.
        /// <para>
        /// This is the single implementation. It runs the middleware in the order Floating UI
        /// established — <c>offset → flip → shift</c> — and returns the arrow offset needed to keep
        /// the arrow pointing at the anchor after any shift.
        /// </para>
        /// <para>
        /// <paramref name="offset"/> must already include the arrow size. Previously the helper
        /// validated a placement using only the gap while <c>CustomToolTip</c> applied gap + arrow,
        /// so a placement could be certified as fitting and then drawn where it did not.
        /// </para>
        /// </summary>
        /// <returns>
        /// The resolved placement, the tooltip's screen position, and the arrow's pixel offset from
        /// the centre of the tooltip edge (positive = toward End).
        /// </returns>
        public static (ToolTipPlacement placement, Point position, int arrowOffset) Resolve(
            Rectangle anchorRect,
            Size tooltipSize,
            ToolTipPlacement preferredPlacement,
            int offset,
            int viewportPadding)
        {
            var screenBounds = GetScreenBounds(anchorRect.IsEmpty
                ? anchorRect.Location
                : new Point(anchorRect.Left + anchorRect.Width / 2, anchorRect.Top + anchorRect.Height / 2));

            var placement = CalculateOptimalPlacement(anchorRect, tooltipSize, preferredPlacement, offset);
            var bounds = CalculateBoundsForPlacement(anchorRect, tooltipSize, placement, offset);

            // SHIFT: slide along the placement's cross axis to stay inside the viewport, without
            // changing which side of the anchor we are on.
            var shifted = Shift(bounds, screenBounds, placement, viewportPadding);

            // ARROW: how far the anchor's centre now sits from the tooltip's centre on the cross
            // axis. Without this the arrow keeps pointing at the tooltip's own middle after a shift.
            int arrowOffset = IsVerticalPlacement(placement)
                ? (anchorRect.Left + anchorRect.Width / 2) - (shifted.Left + shifted.Width / 2)
                : (anchorRect.Top + anchorRect.Height / 2) - (shifted.Top + shifted.Height / 2);

            return (placement, shifted.Location, arrowOffset);
        }

        /// <summary>
        /// Slides a rectangle along the axis parallel to the tooltip's edge so it stays within the
        /// viewport, leaving the chosen side intact. The perpendicular axis is clamped too, as a
        /// last resort for anchors that sit off-screen entirely.
        /// </summary>
        private static Rectangle Shift(Rectangle bounds, Rectangle screenBounds,
            ToolTipPlacement placement, int padding)
        {
            var r = bounds;

            if (IsVerticalPlacement(placement))
            {
                if (r.Left < screenBounds.Left + padding) r.X = screenBounds.Left + padding;
                else if (r.Right > screenBounds.Right - padding) r.X = screenBounds.Right - r.Width - padding;

                if (r.Top < screenBounds.Top + padding) r.Y = screenBounds.Top + padding;
                else if (r.Bottom > screenBounds.Bottom - padding) r.Y = screenBounds.Bottom - r.Height - padding;
            }
            else
            {
                if (r.Top < screenBounds.Top + padding) r.Y = screenBounds.Top + padding;
                else if (r.Bottom > screenBounds.Bottom - padding) r.Y = screenBounds.Bottom - r.Height - padding;

                if (r.Left < screenBounds.Left + padding) r.X = screenBounds.Left + padding;
                else if (r.Right > screenBounds.Right - padding) r.X = screenBounds.Right - r.Width - padding;
            }

            return r;
        }

        /// <summary>True for Top*/Bottom* placements, where the cross axis is horizontal.</summary>
        public static bool IsVerticalPlacement(ToolTipPlacement p) => p switch
        {
            ToolTipPlacement.Top or ToolTipPlacement.TopStart or ToolTipPlacement.TopEnd or
            ToolTipPlacement.Bottom or ToolTipPlacement.BottomStart or ToolTipPlacement.BottomEnd => true,
            _ => false
        };

        /// <summary>The placement on the opposite side of the anchor, keeping the alignment.</summary>
        public static ToolTipPlacement GetOppositePlacement(ToolTipPlacement p) => p switch
        {
            ToolTipPlacement.Top => ToolTipPlacement.Bottom,
            ToolTipPlacement.TopStart => ToolTipPlacement.BottomStart,
            ToolTipPlacement.TopEnd => ToolTipPlacement.BottomEnd,
            ToolTipPlacement.Bottom => ToolTipPlacement.Top,
            ToolTipPlacement.BottomStart => ToolTipPlacement.TopStart,
            ToolTipPlacement.BottomEnd => ToolTipPlacement.TopEnd,
            ToolTipPlacement.Left => ToolTipPlacement.Right,
            ToolTipPlacement.LeftStart => ToolTipPlacement.RightStart,
            ToolTipPlacement.LeftEnd => ToolTipPlacement.RightEnd,
            ToolTipPlacement.Right => ToolTipPlacement.Left,
            ToolTipPlacement.RightStart => ToolTipPlacement.LeftStart,
            ToolTipPlacement.RightEnd => ToolTipPlacement.LeftEnd,
            _ => p
        };

        /// <summary>
        /// Does the rectangle fit on the axis the placement actually cares about? A Top placement
        /// only needs vertical room — horizontal overflow is the shift stage's job.
        /// </summary>
        private static bool FitsOnPrimaryAxis(Rectangle bounds, Rectangle screenBounds, ToolTipPlacement placement)
            => IsVerticalPlacement(placement)
                ? bounds.Top >= screenBounds.Top + ScreenEdgePadding && bounds.Bottom <= screenBounds.Bottom - ScreenEdgePadding
                : bounds.Left >= screenBounds.Left + ScreenEdgePadding && bounds.Right <= screenBounds.Right - ScreenEdgePadding;

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
            var (placement, position, _) = Resolve(
                targetRect, tooltipSize, preferredPlacement, offset, ScreenEdgePadding);
            return (placement, position);
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
        /// Space available for a tooltip on a given side of an anchor, after the gap and the
        /// viewport padding.
        /// <para>
        /// This is Floating UI's <c>size</c> middleware: it reports what actually fits on the side
        /// that was chosen, so the tooltip can clamp to it. Clamping against a fraction of the
        /// whole screen — as <see cref="CalculateResponsiveSize"/> does on its own — says a tooltip
        /// above an anchor near the top of the display may be 80% of the screen tall, when the real
        /// answer is the 60px between the anchor and the top edge.
        /// </para>
        /// </summary>
        public static Size AvailableSpaceFor(
            Rectangle anchorRect,
            ToolTipPlacement placement,
            int offset,
            int viewportPadding)
        {
            var screen = GetScreenBounds(new Point(
                anchorRect.Left + anchorRect.Width / 2,
                anchorRect.Top + anchorRect.Height / 2));

            int width, height;

            if (IsVerticalPlacement(placement))
            {
                bool above = placement is ToolTipPlacement.Top
                    or ToolTipPlacement.TopStart or ToolTipPlacement.TopEnd;

                height = above
                    ? anchorRect.Top - screen.Top - offset - viewportPadding
                    : screen.Bottom - anchorRect.Bottom - offset - viewportPadding;

                // Across the cross axis the tooltip may use the whole viewport, since shift() can
                // slide it along that axis.
                width = screen.Width - viewportPadding * 2;
            }
            else
            {
                bool leftSide = placement is ToolTipPlacement.Left
                    or ToolTipPlacement.LeftStart or ToolTipPlacement.LeftEnd;

                width = leftSide
                    ? anchorRect.Left - screen.Left - offset - viewportPadding
                    : screen.Right - anchorRect.Right - offset - viewportPadding;

                height = screen.Height - viewportPadding * 2;
            }

            return new Size(Math.Max(0, width), Math.Max(0, height));
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

        // Removed with the move to Resolve():
        //   AdjustForScreenEdges      — superseded by Shift(), which slides along the placement's
        //                               own axis instead of clamping both axes blindly.
        //   DetectCollisions          — a one-line negation of IsFullyVisible, never called.
        //   CalculatePositionWithArrow — a partial second copy of the placement maths whose own
        //                               comment admitted it did not actually adjust the arrow.
    }
}
