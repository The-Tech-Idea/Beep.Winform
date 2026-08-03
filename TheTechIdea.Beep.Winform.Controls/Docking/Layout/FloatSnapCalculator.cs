using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Layout
{
    /// <summary>
    /// Decides where a floating window settles when it is dragged near an edge.
    /// </summary>
    /// <remarks>
    /// A pure function of (window, owner, displays). Snapping used to consider only the owner form's
    /// edges, so a float dragged to the top of a second monitor stopped nowhere in particular —
    /// the one place a user most expects a window to catch is the edge of the screen it is on.
    /// <para>
    /// Extracted from <c>FloatWindow.OnMove</c> so it can be exercised with a supplied display list
    /// rather than by dragging a real window across real monitors.
    /// </para>
    /// </remarks>
    public static class FloatSnapCalculator
    {
        /// <summary>Distance within which an edge attracts the window.</summary>
        public const int DefaultThreshold = 14;

        /// <summary>Which edge, if any, a coordinate was pulled to. Reported so it can be asserted.</summary>
        [Flags]
        public enum SnapEdges
        {
            None = 0,
            Left = 1,
            Right = 2,
            Top = 4,
            Bottom = 8,
        }

        public readonly struct SnapResult
        {
            public SnapResult(Point location, SnapEdges edges, bool toOwner)
            {
                Location = location;
                Edges = edges;
                ToOwner = toOwner;
            }

            /// <summary>Where the window should sit. Equal to its current location when nothing caught.</summary>
            public Point Location { get; }

            public SnapEdges Edges { get; }

            /// <summary>True when the owner's edges won; false when a display edge did.</summary>
            public bool ToOwner { get; }

            public bool Snapped => Edges != SnapEdges.None;

            public override string ToString()
                => Snapped ? $"{Location} via {Edges} ({(ToOwner ? "owner" : "display")})"
                           : $"{Location} (no snap)";
        }

        /// <summary>
        /// Snaps <paramref name="window"/> to whichever edges are within
        /// <paramref name="threshold"/> — the owner's first, then the display's.
        /// </summary>
        /// <param name="window">Current window bounds, in screen coordinates.</param>
        /// <param name="owner">Owner form bounds, or <see cref="Rectangle.Empty"/> when there is none.</param>
        /// <param name="monitors">Displays to consider; may be empty.</param>
        /// <remarks>
        /// The owner is tried first because a float aligned to the application window is almost
        /// always what was meant — the display edge is the fallback for a window dragged away from
        /// it. Each axis is decided independently, so a window can catch the owner's left edge and
        /// the screen's top edge at once.
        /// </remarks>
        public static SnapResult Snap(Rectangle window, Rectangle owner,
                                      IReadOnlyList<MonitorInfo> monitors,
                                      int threshold = DefaultThreshold)
        {
            int x = window.X;
            int y = window.Y;
            var edges = SnapEdges.None;
            bool toOwner = false;

            if (!owner.IsEmpty)
            {
                var (ox, oy, oe) = SnapTo(window, owner, threshold);
                if (oe != SnapEdges.None)
                {
                    x = ox;
                    y = oy;
                    edges = oe;
                    toOwner = true;
                }
            }

            // Whatever the owner did not catch, the display may. Only the axes still unclaimed are
            // considered, so an owner-snapped X is not overridden by a screen edge.
            var area = WorkingAreaFor(window, monitors);
            if (!area.IsEmpty)
            {
                var probe = new Rectangle(x, y, window.Width, window.Height);
                var (sx, sy, se) = SnapTo(probe, area, threshold);

                if ((edges & (SnapEdges.Left | SnapEdges.Right)) == 0 &&
                    (se & (SnapEdges.Left | SnapEdges.Right)) != 0)
                {
                    x = sx;
                    edges |= se & (SnapEdges.Left | SnapEdges.Right);
                }

                if ((edges & (SnapEdges.Top | SnapEdges.Bottom)) == 0 &&
                    (se & (SnapEdges.Top | SnapEdges.Bottom)) != 0)
                {
                    y = sy;
                    edges |= se & (SnapEdges.Top | SnapEdges.Bottom);
                }
            }

            return new SnapResult(new Point(x, y), edges, toOwner && edges != SnapEdges.None);
        }

        private static (int x, int y, SnapEdges edges) SnapTo(Rectangle window, Rectangle target, int threshold)
        {
            int x = window.X;
            int y = window.Y;
            var edges = SnapEdges.None;

            if (Math.Abs(window.Left - target.Left) <= threshold)
            {
                x = target.Left;
                edges |= SnapEdges.Left;
            }
            else if (Math.Abs(window.Right - target.Right) <= threshold)
            {
                x = target.Right - window.Width;
                edges |= SnapEdges.Right;
            }

            if (Math.Abs(window.Top - target.Top) <= threshold)
            {
                y = target.Top;
                edges |= SnapEdges.Top;
            }
            else if (Math.Abs(window.Bottom - target.Bottom) <= threshold)
            {
                y = target.Bottom - window.Height;
                edges |= SnapEdges.Bottom;
            }

            return (x, y, edges);
        }

        /// <summary>Working area of the display the window mostly sits on.</summary>
        private static Rectangle WorkingAreaFor(Rectangle window, IReadOnlyList<MonitorInfo> monitors)
        {
            if (monitors == null || monitors.Count == 0)
                return Rectangle.Empty;

            Rectangle best = Rectangle.Empty;
            long bestArea = 0;
            foreach (var m in monitors)
            {
                var overlap = Rectangle.Intersect(window, m.WorkingArea);
                long area = (long)Math.Max(0, overlap.Width) * Math.Max(0, overlap.Height);
                if (area > bestArea)
                {
                    bestArea = area;
                    best = m.WorkingArea;
                }
            }

            return bestArea > 0 ? best : monitors[0].WorkingArea;
        }
    }
}
