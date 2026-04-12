// LayoutTreeRepairer.cs
// Automatically fixes common problems in an ILayoutNode tree and returns a report.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout
{
    // ─────────────────────────────────────────────────────────────────────────
    // RepairReport
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Describes what the <see cref="LayoutTreeRepairer"/> changed in a tree.
    /// </summary>
    public sealed class RepairReport
    {
        /// <summary><c>true</c> when at least one repair was applied.</summary>
        public bool WasRepaired => Actions.Count > 0;

        /// <summary>Human-readable descriptions of every repair applied.</summary>
        public List<string> Actions { get; } = new List<string>();

        internal void Add(string action) => Actions.Add(action);

        /// <inheritdoc/>
        public override string ToString()
            => WasRepaired
                ? $"Repaired — {Actions.Count} action(s) applied"
                : "No repairs needed";
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Repairer
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Applies a set of non-destructive repairs to an <see cref="ILayoutNode"/>
    /// tree in-place (mutates the existing nodes where possible) and returns the
    /// (possibly new) root together with a <see cref="RepairReport"/>.
    /// </summary>
    /// <remarks>
    /// Repairs applied, in order:
    /// <list type="number">
    ///   <item>Clamp out-of-range <see cref="SplitLayoutNode.Ratio"/> values to [0.10, 0.90].</item>
    ///   <item>Replace null children with an empty <see cref="GroupLayoutNode"/>.</item>
    ///   <item>Collapse degenerate splits (where one child is empty) into the non-empty sibling.</item>
    ///   <item>Remove duplicate document IDs (keep first occurrence; cross-group aware).</item>
    ///   <item>Clear <see cref="GroupLayoutNode.SelectedDocumentId"/> when it is not in <c>DocumentIds</c>.</item>
    ///   <item>Assign a fresh NodeId to any node that has a null/empty one.</item>
    /// </list>
    /// </remarks>
    public static class LayoutTreeRepairer
    {
        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Repairs <paramref name="root"/> in-place and returns the (possibly
        /// replaced) root together with a <see cref="RepairReport"/>.
        /// </summary>
        /// <param name="root">Root of the layout tree to repair.  Must not be null.</param>
        /// <returns>
        /// Tuple of the repaired root (may differ when the root itself is replaced)
        /// and the report describing every change made.
        /// </returns>
        public static (ILayoutNode Root, RepairReport Report) Repair(ILayoutNode root)
        {
            ArgumentNullException.ThrowIfNull(root);

            var report   = new RepairReport();
            var seenDocs = new HashSet<string>(StringComparer.Ordinal);

            var repaired = RepairNode(root, report, seenDocs);
            return (repaired, report);
        }

        // ── Recursive worker ─────────────────────────────────────────────────

        private static ILayoutNode RepairNode(
            ILayoutNode     node,
            RepairReport    report,
            HashSet<string> seenDocs)
        {
            // Assign NodeId if missing
            if (string.IsNullOrWhiteSpace(node.NodeId))
            {
                // NodeId is init-only on the sealed types — we replace the whole node
                report.Add($"Node of type {node.NodeType} had no NodeId; replaced with new node.");
                node = node is GroupLayoutNode g
                    ? new GroupLayoutNode(
                        Guid.NewGuid().ToString(), g.GroupId,
                        new List<string>(g.DocumentIds), g.SelectedDocumentId)
                    : (ILayoutNode)new SplitLayoutNode(
                        ((SplitLayoutNode)node).First,
                        ((SplitLayoutNode)node).Second,
                        ((SplitLayoutNode)node).Orientation,
                        ((SplitLayoutNode)node).Ratio);
            }

            if (node is SplitLayoutNode split)
                return RepairSplit(split, report, seenDocs);

            if (node is GroupLayoutNode group)
            {
                RepairGroup(group, report, seenDocs);
                return group;
            }

            return node;
        }

        private static ILayoutNode RepairSplit(
            SplitLayoutNode split,
            RepairReport    report,
            HashSet<string> seenDocs)
        {
            // 1. Clamp ratio
            if (split.Ratio < 0.1f || split.Ratio > 0.9f)
            {
                float clamped = Math.Max(0.1f, Math.Min(0.9f, split.Ratio));
                report.Add(
                    $"SplitLayoutNode '{split.NodeId}': Ratio {split.Ratio:F3} clamped to {clamped:F3}.");
                split.Ratio = clamped;
            }

            // 2. Replace null children
            if (split.First == null)
            {
                report.Add(
                    $"SplitLayoutNode '{split.NodeId}': null First child replaced with empty group.");
                split.First = new GroupLayoutNode();
            }
            if (split.Second == null)
            {
                report.Add(
                    $"SplitLayoutNode '{split.NodeId}': null Second child replaced with empty group.");
                split.Second = new GroupLayoutNode();
            }

            // 3. Recurse
            split.First  = RepairNode(split.First,  report, seenDocs);
            split.Second = RepairNode(split.Second, report, seenDocs);

            // 4. Collapse degenerate splits (both children empty → collapse to single empty group)
            bool firstEmpty  = split.First.IsEmptyLeaf();
            bool secondEmpty = split.Second.IsEmptyLeaf();

            if (firstEmpty && secondEmpty)
            {
                report.Add(
                    $"SplitLayoutNode '{split.NodeId}': both children empty; collapsed to single empty group.");
                return new GroupLayoutNode();
            }
            if (firstEmpty)
            {
                report.Add(
                    $"SplitLayoutNode '{split.NodeId}': First child is empty; collapsed to Second child.");
                return split.Second;
            }
            if (secondEmpty)
            {
                report.Add(
                    $"SplitLayoutNode '{split.NodeId}': Second child is empty; collapsed to First child.");
                return split.First;
            }

            return split;
        }

        private static void RepairGroup(
            GroupLayoutNode group,
            RepairReport    report,
            HashSet<string> seenDocs)
        {
            // 1. Assign GroupId if missing
            if (string.IsNullOrWhiteSpace(group.GroupId))
            {
                group.GroupId = group.NodeId;
                report.Add($"GroupLayoutNode '{group.NodeId}': missing GroupId assigned from NodeId.");
            }

            // 2. Remove null/empty and duplicate document IDs
            var toRemove = new List<string>();
            var localSeen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var id in group.DocumentIds)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    toRemove.Add(id);
                    report.Add($"GroupLayoutNode '{group.NodeId}': removed null/empty document ID.");
                    continue;
                }
                if (!localSeen.Add(id))
                {
                    toRemove.Add(id);
                    report.Add(
                        $"GroupLayoutNode '{group.NodeId}': removed intra-group duplicate '{id}'.");
                    continue;
                }
                if (!seenDocs.Add(id))
                {
                    toRemove.Add(id);
                    report.Add(
                        $"GroupLayoutNode '{group.NodeId}': removed cross-group duplicate '{id}'.");
                }
            }

            foreach (var id in toRemove)
                group.DocumentIds.Remove(id);

            // 3. Clear SelectedDocumentId if it is no longer in DocumentIds
            if (!string.IsNullOrEmpty(group.SelectedDocumentId)
                && !group.DocumentIds.Contains(group.SelectedDocumentId))
            {
                report.Add(
                    $"GroupLayoutNode '{group.NodeId}': SelectedDocumentId " +
                    $"'{group.SelectedDocumentId}' not in DocumentIds; cleared.");
                group.SelectedDocumentId = group.DocumentIds.Count > 0
                    ? group.DocumentIds[group.DocumentIds.Count - 1]
                    : null;
            }
        }
    }
}
