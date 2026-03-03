using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Sprint 8 — Smart multi-monitor collision-aware position resolver.
    /// Returns a <see cref="ResolvedPosition"/> that is guaranteed to be
    /// fully visible within the working area of the correct screen.
    /// </summary>
    public static class ToolTipPositionResolver
    {
        private const int EdgePadding = 8;

        // ──────────────────────────────────────────────────────────────────────
        // Main resolve method
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Resolve the best on-screen position for a tooltip.
        /// </summary>
        /// <param name="targetBounds">
        ///   Bounds of the target control in screen coordinates.
        /// </param>
        /// <param name="tooltipSize">Desired tooltip size.</param>
        /// <param name="preferred">Caller-preferred placement (may be Auto).</param>
        /// <param name="offset">Gap between target and tooltip in pixels.</param>
        /// <returns>
        ///   A <see cref="ResolvedPosition"/> with the final screen location,
        ///   the actual placement used, and whether a flip occurred.
        /// </returns>
        public static ResolvedPosition Resolve(
            Rectangle targetBounds,
            Size tooltipSize,
            ToolTipPlacement preferred,
            int offset = 8)
        {
            var screen = Screen.FromRectangle(targetBounds);
            var workArea = screen.WorkingArea;

            // Build the priority cascade
            var cascade = BuildCascade(preferred, targetBounds, workArea);

            foreach (var placement in cascade)
            {
                Point loc = ComputeLocation(targetBounds, tooltipSize, placement, offset);
                Rectangle candidate = new Rectangle(loc, tooltipSize);

                if (IsVisible(candidate, workArea))
                {
                    bool flipped = placement != preferred && preferred != ToolTipPlacement.Auto;
                    Point final = ClampToScreen(candidate, workArea);
                    return new ResolvedPosition(final, placement, flipped, screen.Bounds.Size);
                }
            }

            // Fallback: clamp to screen regardless
            var fallbackPlacement = cascade.Length > 0 ? cascade[0] : ToolTipPlacement.Bottom;
            Point fallbackLoc = ComputeLocation(targetBounds, tooltipSize, fallbackPlacement, offset);
            Point clamped = ClampToScreen(new Rectangle(fallbackLoc, tooltipSize), workArea);
            return new ResolvedPosition(clamped, fallbackPlacement, true, screen.Bounds.Size);
        }

        // ──────────────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────────────

        private static ToolTipPlacement[] BuildCascade(
            ToolTipPlacement preferred, Rectangle targetBounds, Rectangle workArea)
        {
            // If explicit placement requested put it first, then opposites then others
            if (preferred != ToolTipPlacement.Auto)
            {
                return new[]
                {
                    preferred,
                    Opposite(preferred),
                    Perpendicular1(preferred),
                    Perpendicular2(preferred),
                    ToolTipPlacement.Bottom,
                    ToolTipPlacement.Top,
                    ToolTipPlacement.Right,
                    ToolTipPlacement.Left
                };
            }

            // Auto: pick based on available space around target
            bool moreBelow  = workArea.Bottom - targetBounds.Bottom > targetBounds.Top - workArea.Top;
            bool moreRight  = workArea.Right  - targetBounds.Right  > targetBounds.Left - workArea.Left;

            return new[]
            {
                moreBelow  ? ToolTipPlacement.Bottom : ToolTipPlacement.Top,
                moreBelow  ? ToolTipPlacement.Top    : ToolTipPlacement.Bottom,
                moreRight  ? ToolTipPlacement.Right  : ToolTipPlacement.Left,
                moreRight  ? ToolTipPlacement.Left   : ToolTipPlacement.Right,
            };
        }

        private static ToolTipPlacement Opposite(ToolTipPlacement p) => p switch
        {
            ToolTipPlacement.Top       or ToolTipPlacement.TopStart    or ToolTipPlacement.TopEnd    => ToolTipPlacement.Bottom,
            ToolTipPlacement.Bottom    or ToolTipPlacement.BottomStart or ToolTipPlacement.BottomEnd => ToolTipPlacement.Top,
            ToolTipPlacement.Left      or ToolTipPlacement.LeftStart   or ToolTipPlacement.LeftEnd   => ToolTipPlacement.Right,
            ToolTipPlacement.Right     or ToolTipPlacement.RightStart  or ToolTipPlacement.RightEnd  => ToolTipPlacement.Left,
            _ => ToolTipPlacement.Bottom
        };

        private static ToolTipPlacement Perpendicular1(ToolTipPlacement p) => p switch
        {
            ToolTipPlacement.Top    or ToolTipPlacement.Bottom => ToolTipPlacement.Right,
            _                                                   => ToolTipPlacement.Bottom
        };

        private static ToolTipPlacement Perpendicular2(ToolTipPlacement p) => p switch
        {
            ToolTipPlacement.Top    or ToolTipPlacement.Bottom => ToolTipPlacement.Left,
            _                                                   => ToolTipPlacement.Top
        };

        private static Point ComputeLocation(
            Rectangle target, Size size, ToolTipPlacement placement, int offset)
        {
            return placement switch
            {
                ToolTipPlacement.Top        => new Point(target.Left  + (target.Width  - size.Width)  / 2, target.Top    - size.Height - offset),
                ToolTipPlacement.TopStart   => new Point(target.Left,                                       target.Top    - size.Height - offset),
                ToolTipPlacement.TopEnd     => new Point(target.Right - size.Width,                         target.Top    - size.Height - offset),
                ToolTipPlacement.Bottom     => new Point(target.Left  + (target.Width  - size.Width)  / 2, target.Bottom + offset),
                ToolTipPlacement.BottomStart=> new Point(target.Left,                                       target.Bottom + offset),
                ToolTipPlacement.BottomEnd  => new Point(target.Right - size.Width,                         target.Bottom + offset),
                ToolTipPlacement.Left       => new Point(target.Left  - size.Width  - offset,              target.Top    + (target.Height - size.Height) / 2),
                ToolTipPlacement.LeftStart  => new Point(target.Left  - size.Width  - offset,              target.Top),
                ToolTipPlacement.LeftEnd    => new Point(target.Left  - size.Width  - offset,              target.Bottom - size.Height),
                ToolTipPlacement.Right      => new Point(target.Right + offset,                             target.Top    + (target.Height - size.Height) / 2),
                ToolTipPlacement.RightStart => new Point(target.Right + offset,                             target.Top),
                ToolTipPlacement.RightEnd   => new Point(target.Right + offset,                             target.Bottom - size.Height),
                _                           => new Point(target.Left  + (target.Width  - size.Width)  / 2, target.Bottom + offset),
            };
        }

        private static bool IsVisible(Rectangle rect, Rectangle workArea)
            => workArea.Contains(new Rectangle(
                rect.X       + EdgePadding,
                rect.Y       + EdgePadding,
                rect.Width   - EdgePadding * 2,
                rect.Height  - EdgePadding * 2));

        private static Point ClampToScreen(Rectangle rect, Rectangle workArea)
        {
            int x = Math.Max(workArea.Left   + EdgePadding, Math.Min(rect.X, workArea.Right  - rect.Width  - EdgePadding));
            int y = Math.Max(workArea.Top    + EdgePadding, Math.Min(rect.Y, workArea.Bottom - rect.Height - EdgePadding));
            return new Point(x, y);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Result struct
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Result of <see cref="ToolTipPositionResolver.Resolve"/>.
    /// </summary>
    public readonly struct ResolvedPosition
    {
        /// <summary>Final screen-coordinate location for the top-left of the tooltip.</summary>
        public Point Location { get; }

        /// <summary>Actual placement used (may differ from the requested placement if a flip was needed).</summary>
        public ToolTipPlacement ActualPlacement { get; }

        /// <summary>True if the resolver had to flip to a different placement.</summary>
        public bool WasFlipped { get; }

        /// <summary>Physical screen size (useful for DPI calculations by the caller).</summary>
        public Size ScreenSize { get; }

        public ResolvedPosition(Point location, ToolTipPlacement actualPlacement, bool wasFlipped, Size screenSize)
        {
            Location        = location;
            ActualPlacement = actualPlacement;
            WasFlipped      = wasFlipped;
            ScreenSize      = screenSize;
        }
    }
}
