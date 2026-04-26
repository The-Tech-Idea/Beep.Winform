// LayoutTreeApplier.cs
// Walks an ILayoutNode tree and materialises the splits and document
// assignments into a live BeepDocumentHost.
//
// Sprint 19: The layout engine now supports arbitrary nested splits via
// the ILayoutNode tree. The applier can restore trees of any depth.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout
{
    // ─────────────────────────────────────────────────────────────────────────
    // Result
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Result returned by <see cref="LayoutTreeApplier.Apply"/>.
    /// </summary>
    public sealed class LayoutApplyReport
    {
        /// <summary>Number of tab groups successfully materialised.</summary>
        public int GroupsApplied { get; internal set; }

        /// <summary>Document IDs that were in the tree but not found in the host.</summary>
        public List<string> MissingDocumentIds { get; } = new List<string>();

        /// <summary>Non-fatal messages (e.g. tree depth exceeded engine limit).</summary>
        public List<string> Warnings { get; } = new List<string>();

        /// <summary>True when no missing docs and no warnings were generated.</summary>
        public bool Success => MissingDocumentIds.Count == 0 && Warnings.Count == 0;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Applier
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Restores a <see cref="BeepDocumentHost"/> group topology from a saved
    /// <see cref="ILayoutNode"/> tree.
    /// <para>
    /// The host must already contain all documents referenced by the tree
    /// (via <see cref="BeepDocumentHost.AddDocument"/>).  The applier only
    /// moves documents between groups — it does not create or destroy document
    /// content.
    /// </para>
    /// </summary>
    public static class LayoutTreeApplier
    {
        /// <summary>
        /// Applies <paramref name="root"/> to <paramref name="host"/> by merging
        /// to a single group first, then recreating splits as described by the tree.
        /// Supports arbitrary nested split trees.
        /// </summary>
        public static LayoutApplyReport Apply(BeepDocumentHost host, ILayoutNode root)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (root  == null) return new LayoutApplyReport { GroupsApplied = 1 };

            var report = new LayoutApplyReport();

            // 1. Reset to single group
            host.MergeAllGroups();

            // 2. Collect leaf groups from tree (depth-first, left-to-right)
            var leaves = new List<GroupLayoutNode>();
            CollectLeaves(root, leaves);

            if (leaves.Count == 0)
            {
                report.Warnings.Add("Layout tree contains no leaf groups — nothing applied.");
                return report;
            }

            report.GroupsApplied = leaves.Count;

            // 3. Single group — just activate the right document
            if (leaves.Count == 1)
            {
                ActivateSelected(host, leaves[0], report);
                return report;
            }

            // 4. Multi-group — recursively apply the tree
            ApplyTreeToHost(host, root, report, leaves);

            return report;
        }

        /// <summary>
        /// Recursively applies a layout tree to the host by creating splits
        /// and moving documents into the correct groups.
        /// </summary>
        private static void ApplyTreeToHost(BeepDocumentHost host, ILayoutNode node,
            LayoutApplyReport report, List<GroupLayoutNode> leaves)
        {
            if (node is GroupLayoutNode leaf)
            {
                ActivateSelected(host, leaf, report);
                return;
            }

            if (node is not SplitLayoutNode split) return;

            // Collect leaves from left and right subtrees
            var leftLeaves  = new List<GroupLayoutNode>();
            var rightLeaves = new List<GroupLayoutNode>();
            CollectLeaves(split.First, leftLeaves);
            CollectLeaves(split.Second, rightLeaves);

            if (leftLeaves.Count == 0 || rightLeaves.Count == 0) return;

            // Find a driver document from the right subtree
            string? driverDocId = null;
            foreach (var leaf in rightLeaves)
            {
                foreach (var id in leaf.DocumentIds)
                {
                    if (host.GetPanel(id) != null) { driverDocId = id; break; }
                }
                if (driverDocId != null) break;
            }

            if (driverDocId == null)
            {
                report.Warnings.Add("No documents found to drive split — subtree skipped.");
                return;
            }

            // Apply split
            bool horizontal = split.Orientation == Orientation.Horizontal;
            host.SplitRatio = split.Ratio;

            bool splitOk = horizontal
                ? host.SplitDocumentHorizontal(driverDocId)
                : host.SplitDocumentVertical(driverDocId);

            if (!splitOk)
            {
                report.Warnings.Add("Split failed (MaxGroups limit or invalid document).");
                return;
            }

            // Move remaining right-subtree docs into the new group
            var groups = host.Groups;
            if (groups.Count < 2) return;

            string rightGroupId = groups[groups.Count - 1].GroupId;
            foreach (var leaf in rightLeaves)
            {
                foreach (var id in leaf.DocumentIds)
                {
                    if (id == driverDocId) continue;
                    if (host.GetPanel(id) != null)
                        host.MoveDocumentToGroup(id, rightGroupId);
                    else if (!report.MissingDocumentIds.Contains(id))
                        report.MissingDocumentIds.Add(id);
                }
            }

            // Note missing left-subtree docs
            foreach (var leaf in leftLeaves)
            {
                foreach (var id in leaf.DocumentIds)
                    if (host.GetPanel(id) == null && !report.MissingDocumentIds.Contains(id))
                        report.MissingDocumentIds.Add(id);
            }

            // Recursively apply nested splits
            ApplyTreeToHost(host, split.First, report, leaves);
            ApplyTreeToHost(host, split.Second, report, leaves);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static void CollectLeaves(ILayoutNode node, List<GroupLayoutNode> result)
        {
            if (node is GroupLayoutNode g)
            {
                result.Add(g);
            }
            else if (node is SplitLayoutNode s)
            {
                CollectLeaves(s.First,  result);
                CollectLeaves(s.Second, result);
            }
        }

        private static void ActivateSelected(BeepDocumentHost host,
                                              GroupLayoutNode leaf,
                                              LayoutApplyReport report)
        {
            var sel = leaf.SelectedDocumentId;
            if (!string.IsNullOrEmpty(sel) && host.GetPanel(sel) != null)
                host.SetActiveDocument(sel!);
            else if (!string.IsNullOrEmpty(sel) && !report.MissingDocumentIds.Contains(sel!))
                report.MissingDocumentIds.Add(sel!);
        }
    }
}
