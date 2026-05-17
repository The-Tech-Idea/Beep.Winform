// SubmenuTriangleTracker.cs
// Phase 04B — Commercial-Grade Menubar UX.
//
// Pure geometry: returns true when the cursor's trajectory from
// lastPos to currentPos is "headed toward" an open submenu so the
// caller can defer dismissal of that submenu.
//
// This is the classic Amazon Mega Menu / WPF MenuItem heuristic:
// while the user is diagonally drifting toward the open submenu,
// briefly leaving the parent item should NOT close the submenu.
// Without this guard, every keyboard-allergic user who tries to drag
// their cursor diagonally into a submenu loses it half-way.
//
// Visualisation:
//
//      Parent item                         Submenu
//      ┌──────────┐                       ┌──────────┐
//      │          │ submenuTopLeft  ─►    │          │
//      │  ●  ←─ lastPos                   │          │
//      │   ╲                              │          │
//      │    ╲ ◄── triangle ──►            │          │
//      │     ╲                            │          │
//      │      ●  ← currentPos             │          │
//      │       (inside triangle)          │          │
//      │          │ submenuBottomLeft ─► │          │
//      └──────────┘                       └──────────┘
//
// The triangle is formed by lastPos + submenu's top-left + submenu's
// bottom-left corners. While currentPos stays inside that triangle,
// the cursor is "tracking" toward the submenu.
//
// Stateless by design — owner classes hold any needed state. All
// inputs are in SCREEN coordinates; consistency is the caller's
// responsibility.
//
// References:
//   * "Pull-Down Menus, Faster" (Bret Victor blog, 2005)
//   * WPF MenuItem.IsSubmenuOpen heuristic source
//   * Amazon retail mega-menu implementation analysis
//
// See .plans/Menus-Phase-04-CommercialMenuBarUX.md.
// ─────────────────────────────────────────────────────────────────────────────
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Helpers
{
    /// <summary>
    /// Stateless geometry helper that answers "is the cursor heading
    /// toward this submenu?".
    /// </summary>
    internal static class SubmenuTriangleTracker
    {
        /// <summary>
        /// Returns <c>true</c> when <paramref name="currentPos"/> lies
        /// inside the triangle formed by <paramref name="lastPos"/>,
        /// the submenu's top-left corner, and the submenu's bottom-left
        /// corner — i.e. the cursor is plausibly drifting toward
        /// the submenu. All points are in screen coordinates.
        /// </summary>
        /// <param name="lastPos">The cursor position the last time it was tested (screen coords).</param>
        /// <param name="currentPos">The cursor position now (screen coords).</param>
        /// <param name="submenuScreenBounds">The submenu's bounds (screen coords).</param>
        public static bool IsCursorTrackingTowardSubmenu(
            Point lastPos,
            Point currentPos,
            Rectangle submenuScreenBounds)
        {
            if (submenuScreenBounds.IsEmpty) return false;
            if (submenuScreenBounds.Width <= 0 || submenuScreenBounds.Height <= 0) return false;

            // If the cursor is already inside the submenu, definitely tracking.
            if (submenuScreenBounds.Contains(currentPos)) return true;

            // Pick the two submenu corners on the side closest to the cursor.
            // Default is "submenu to the right" (top-left + bottom-left corners
            // of the submenu form the triangle vertices), which is the standard
            // LTR layout. RTL / left-opening submenus would use the right edge.
            Point cornerA, cornerB;
            if (currentPos.X <= submenuScreenBounds.Left)
            {
                cornerA = new Point(submenuScreenBounds.Left, submenuScreenBounds.Top);
                cornerB = new Point(submenuScreenBounds.Left, submenuScreenBounds.Bottom);
            }
            else if (currentPos.X >= submenuScreenBounds.Right)
            {
                cornerA = new Point(submenuScreenBounds.Right, submenuScreenBounds.Top);
                cornerB = new Point(submenuScreenBounds.Right, submenuScreenBounds.Bottom);
            }
            else
            {
                // Cursor is horizontally inside the submenu's x-range but
                // outside the bounds (above or below). The triangle test
                // degenerates here — let the timer handle dismissal.
                return false;
            }

            return PointInTriangle(currentPos, lastPos, cornerA, cornerB);
        }

        /// <summary>
        /// Standard barycentric point-in-triangle test. Returns true when
        /// <paramref name="p"/> lies on or inside the triangle
        /// (<paramref name="a"/>, <paramref name="b"/>, <paramref name="c"/>).
        ///
        /// Robust against degenerate triangles (collinear vertices) — a
        /// zero-area triangle reports false so the caller falls back to
        /// the timer-based dismissal path.
        /// </summary>
        private static bool PointInTriangle(Point p, Point a, Point b, Point c)
        {
            // Cross products (using long to avoid int overflow on huge coords).
            long d1 = Sign(p, a, b);
            long d2 = Sign(p, b, c);
            long d3 = Sign(p, c, a);

            bool hasNeg = d1 < 0 || d2 < 0 || d3 < 0;
            bool hasPos = d1 > 0 || d2 > 0 || d3 > 0;

            // Point is inside iff it lies on the same side of every edge.
            return !(hasNeg && hasPos);
        }

        private static long Sign(Point p1, Point p2, Point p3)
        {
            // Returns >0 / <0 / 0 depending on which side of the line
            // (p2→p3) the point p1 lies on.
            return (long)(p1.X - p3.X) * (p2.Y - p3.Y)
                 - (long)(p2.X - p3.X) * (p1.Y - p3.Y);
        }
    }
}
