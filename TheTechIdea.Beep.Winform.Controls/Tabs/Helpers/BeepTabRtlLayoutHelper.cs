// BeepTabRtlLayoutHelper.cs
// Right-to-left (RTL) mirroring support for BeepTabHeaderHost layout snapshots.
// Mirrors tab-body bounds, close-button bounds, and adornment bounds relative to
// the header strip, while intentionally preserving the logical (visual) tab order.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Mirrors a <see cref="BeepTabHeaderLayoutSnapshot"/> for right-to-left layouts.
    /// Call <see cref="MirrorSnapshot"/> immediately after the snapshot is built
    /// when <c>control.RightToLeft == RightToLeft.Yes</c>.
    /// </summary>
    public static class BeepTabRtlLayoutHelper
    {
        /// <summary>
        /// Mirrors every bounds rectangle in <paramref name="snapshot"/> horizontally
        /// within <paramref name="containerWidth"/> so that the first logical tab
        /// appears on the right edge of the header.
        /// </summary>
        /// <param name="snapshot">The snapshot to mutate in-place.</param>
        /// <param name="containerWidth">
        /// The pixel width of the header strip (typically <c>BeepTabHeaderHost.Width</c>).
        /// </param>
        /// <exception cref="ArgumentNullException"/>
        public static void MirrorSnapshot(BeepTabHeaderLayoutSnapshot snapshot, int containerWidth)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            if (containerWidth <= 0) return;

            foreach (BeepTabHeaderItemLayout item in snapshot.Items)
            {
                item.Bounds             = MirrorH(item.Bounds,             containerWidth);
                item.TextBounds         = MirrorH(item.TextBounds,         containerWidth);
                item.CloseButtonBounds  = MirrorH(item.CloseButtonBounds,  containerWidth);
                item.IconBounds         = MirrorH(item.IconBounds,         containerWidth);
                item.SubTextBounds      = MirrorH(item.SubTextBounds,      containerWidth);
                item.BadgeBounds        = MirrorH(item.BadgeBounds,        containerWidth);
                item.DirtyMarkerBounds  = MirrorH(item.DirtyMarkerBounds,  containerWidth);
                item.BusyIndicatorBounds= MirrorH(item.BusyIndicatorBounds,containerWidth);
            }

            // Also mirror the header strip's own bounds (for overflow button placement).
            snapshot.HeaderBounds = MirrorH(snapshot.HeaderBounds, containerWidth);
        }

        /// <summary>
        /// Converts an LTR client-coordinate <see cref="Point"/> (e.g. from a mouse
        /// event) to its RTL-equivalent so existing hit-test code can be reused without
        /// modification.
        /// </summary>
        /// <param name="point">Original LTR client point.</param>
        /// <param name="containerWidth">Width of the header strip.</param>
        public static Point FlipPoint(Point point, int containerWidth)
        {
            return new Point(containerWidth - point.X - 1, point.Y);
        }

        /// <summary>
        /// Returns <see langword="true"/> when <paramref name="containerWidth"/> and
        /// <paramref name="rightToLeft"/> together require RTL mirroring.
        /// </summary>
        public static bool ShouldMirror(System.Windows.Forms.RightToLeft rightToLeft, int containerWidth)
        {
            return rightToLeft == System.Windows.Forms.RightToLeft.Yes && containerWidth > 0;
        }

        // ── Private helper ────────────────────────────────────────────────────

        /// <summary>
        /// Mirrors a single <see cref="Rectangle"/> horizontally within
        /// <paramref name="containerWidth"/>.
        /// Empty rectangles are returned unchanged.
        /// </summary>
        private static Rectangle MirrorH(Rectangle r, int containerWidth)
        {
            if (r.IsEmpty) return r;
            return new Rectangle(containerWidth - r.Right, r.Y, r.Width, r.Height);
        }
    }
}
