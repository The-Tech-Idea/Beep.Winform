// LayoutTreeBuilder.cs
// Captures the current group topology of a BeepDocumentHost as an ILayoutNode tree.
// The tree can be serialised to JSON and later restored via LayoutTreeApplier.
// ─────────────────────────────────────────────────────────────────────────────
using System.Collections.Generic;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout
{
    /// <summary>
    /// Builds an <see cref="ILayoutNode"/> snapshot from the live state of a
    /// <see cref="BeepDocumentHost"/>.
    /// </summary>
    public static class LayoutTreeBuilder
    {
        /// <summary>
        /// Captures the current group topology of <paramref name="host"/> and
        /// returns the root of an <see cref="ILayoutNode"/> tree.
        /// </summary>
        /// <remarks>
        /// With the Sprint-12 layout engine (2-group max), the result is either
        /// a single <see cref="GroupLayoutNode"/> or a one-level
        /// <see cref="SplitLayoutNode"/> with two <see cref="GroupLayoutNode"/> leaves.
        /// When the engine is extended to support deeper trees, this method will
        /// produce arbitrarily nested <see cref="SplitLayoutNode"/>s.
        /// </remarks>
        public static ILayoutNode BuildFromHost(BeepDocumentHost host)
        {
            var groups = host.Groups;   // IReadOnlyList<BeepDocumentGroup>

            if (groups.Count <= 1)
                return BuildGroupNode(host, groups[0]);

            if (groups.Count == 2)
            {
                var orientation = host.SplitHorizontal
                    ? Orientation.Horizontal
                    : Orientation.Vertical;

                return new SplitLayoutNode(
                    BuildGroupNode(host, groups[0]),
                    BuildGroupNode(host, groups[1]),
                    orientation,
                    host.SplitRatio);
            }

            // > 2 groups — build a degenerate right-leaning chain until the
            // layout engine supports arbitrary depth.
            ILayoutNode rightChain = BuildGroupNode(host, groups[groups.Count - 1]);
            for (int i = groups.Count - 2; i >= 1; i--)
            {
                rightChain = new SplitLayoutNode(
                    BuildGroupNode(host, groups[i]),
                    rightChain,
                    Orientation.Horizontal,
                    1.0f / (groups.Count - i));
            }

            return new SplitLayoutNode(
                BuildGroupNode(host, groups[0]),
                rightChain,
                host.SplitHorizontal ? Orientation.Horizontal : Orientation.Vertical,
                host.SplitRatio);
        }

        // ─────────────────────────────────────────────────────────────────────

        private static GroupLayoutNode BuildGroupNode(BeepDocumentHost host,
                                                      BeepDocumentGroup group)
        {
            // Determine which document is active in this group by checking the
            // host's active document and whether it belongs to this group.
            string? selectedId = null;
            var activeId = host.ActiveDocumentId;
            if (!string.IsNullOrEmpty(activeId)
                && group.DocumentIds.Contains(activeId))
            {
                selectedId = activeId;
            }
            else if (group.DocumentIds.Count > 0)
            {
                // Fall back to the last tab in the group (most recently activated heuristic)
                selectedId = group.DocumentIds[group.DocumentIds.Count - 1];
            }

            return new GroupLayoutNode(
                nodeId:             group.GroupId,
                groupId:            group.GroupId,
                documentIds:        new List<string>(group.DocumentIds),
                selectedDocumentId: selectedId);
        }
    }
}
