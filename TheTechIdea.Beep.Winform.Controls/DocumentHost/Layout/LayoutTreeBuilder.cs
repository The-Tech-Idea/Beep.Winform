// LayoutTreeBuilder.cs
// Captures the current group topology of a BeepDocumentHost as an ILayoutNode tree.
// The tree can be serialised to JSON and later restored via LayoutTreeApplier.
// Supports arbitrary nested splits (Sprint 19).
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
        /// Supports arbitrary nested splits.
        /// </summary>
        public static ILayoutNode BuildFromHost(BeepDocumentHost host)
        {
            var groups = host.Groups;

            if (groups.Count <= 1)
                return BuildGroupNode(host, groups[0]);

            // Build a left-leaning binary tree from the group list:
            //   Split(Group0, Split(Group1, Split(Group2, Group3)))
            ILayoutNode BuildSubtree(int startIdx)
            {
                if (startIdx >= groups.Count)
                    throw new System.InvalidOperationException("Not enough groups.");

                if (startIdx == groups.Count - 1)
                    return BuildGroupNode(host, groups[startIdx]);

                if (startIdx == groups.Count - 2)
                {
                    return new SplitLayoutNode(
                        BuildGroupNode(host, groups[startIdx]),
                        BuildGroupNode(host, groups[startIdx + 1]),
                        host.SplitHorizontal ? Orientation.Horizontal : Orientation.Vertical,
                        host.SplitRatio);
                }

                var current = BuildGroupNode(host, groups[startIdx]);
                var rest    = BuildSubtree(startIdx + 1);
                return new SplitLayoutNode(current, rest,
                    host.SplitHorizontal ? Orientation.Horizontal : Orientation.Vertical,
                    host.SplitRatio);
            }

            return BuildSubtree(0);
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
