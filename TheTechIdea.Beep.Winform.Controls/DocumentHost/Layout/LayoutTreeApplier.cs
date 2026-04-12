// LayoutTreeApplier.cs
// Walks an ILayoutNode tree and materialises the splits and document
// assignments into a live BeepDocumentHost.
//
// Scope (Sprint 12): the current layout engine supports up to 2 groups
// (one flat split). Trees deeper than 2 leaves are partially applied;
// a warning is recorded for every group that could not be placed.
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
        // ── Constants ─────────────────────────────────────────────────────────
        /// <summary>
        /// Maximum number of leaf groups the current layout engine can display.
        /// Increase this constant when the engine is upgraded to support deep trees.
        /// </summary>
        public const int MaxSupportedGroups = 4;

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Applies <paramref name="root"/> to <paramref name="host"/> by merging
        /// to a single group first, then recreating splits as described by the tree.
        /// </summary>
        /// <param name="host">The live host to reconfigure.</param>
        /// <param name="root">Root of the saved layout tree.</param>
        /// <returns>A <see cref="LayoutApplyReport"/> describing what happened.</returns>
        public static LayoutApplyReport Apply(BeepDocumentHost host, ILayoutNode root)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (root  == null) return new LayoutApplyReport { GroupsApplied = 1 };

            var report = new LayoutApplyReport();

            // ── 1. Reset to single group ──────────────────────────────────────
            host.MergeAllGroups();   // all docs now in primary, no splits

            // ── 2. Collect leaf groups from tree (depth-first, left-to-right) ─
            var leaves = new List<GroupLayoutNode>();
            CollectLeaves(root, leaves);

            if (leaves.Count == 0)
            {
                report.Warnings.Add("Layout tree contains no leaf groups — nothing applied.");
                return report;
            }

            report.GroupsApplied = Math.Min(leaves.Count, MaxSupportedGroups);

            if (leaves.Count > MaxSupportedGroups)
                report.Warnings.Add(
                    $"Tree has {leaves.Count} leaf groups; the current engine supports " +
                    $"{MaxSupportedGroups}. Groups {MaxSupportedGroups + 1}–{leaves.Count} " +
                    $"were merged into the primary group.");

            // ── 3. Single group — just activate the right document ────────────
            if (leaves.Count == 1)
            {
                ActivateSelected(host, leaves[0], report);
                return report;
            }

            // ── 4. Two (or more) groups — find the root split orientation ──────
            var rootSplit  = root as SplitLayoutNode;
            bool horizontal = rootSplit == null
                || rootSplit.Orientation == Orientation.Horizontal;
            float ratio    = rootSplit?.Ratio ?? 0.5f;

            var firstLeaf  = leaves[0];
            var secondLeaf = leaves[1];

            // ── 5. Pick the document that will drive the split ────────────────
            //       Prefer the first doc from the second leaf so it ends up in
            //       the correct (secondary = right/bottom) group after SplitInternal.
            string? driverDocId = null;
            foreach (var id in secondLeaf.DocumentIds)
            {
                if (host.GetPanel(id) != null) { driverDocId = id; break; }
                else report.MissingDocumentIds.Add(id);
            }

            // Fallback: use the last doc from the first leaf
            if (driverDocId == null)
            {
                for (int i = firstLeaf.DocumentIds.Count - 1; i >= 0; i--)
                {
                    var id = firstLeaf.DocumentIds[i];
                    if (host.GetPanel(id) != null) { driverDocId = id; break; }
                }
            }

            if (driverDocId == null)
            {
                report.Warnings.Add("No documents found to drive the split — tree not applied.");
                return report;
            }

            // ── 6. Apply split ratio then perform the split ───────────────────
            host.SplitRatio = ratio;

            bool splitOk = horizontal
                ? host.SplitDocumentHorizontal(driverDocId)
                : host.SplitDocumentVertical(driverDocId);

            if (!splitOk)
            {
                report.Warnings.Add("Split failed (MaxGroups limit reached or invalid document).");
                return report;
            }

            // ── 7. Move remaining second-leaf docs into the new group ─────────
            var groups      = host.Groups;   // IReadOnlyList<BeepDocumentGroup>
            if (groups.Count < 2)
            {
                report.Warnings.Add("Split was requested but only one group exists after split.");
                return report;
            }

            string secondGroupId = groups[1].GroupId;

            // The driverDoc is already in group[1] (SplitInternal moves it there).
            // Move any remaining second-leaf docs.
            foreach (var id in secondLeaf.DocumentIds)
            {
                if (id == driverDocId) continue;   // already moved

                if (host.GetPanel(id) != null)
                    host.MoveDocumentToGroup(id, secondGroupId);
                else if (!report.MissingDocumentIds.Contains(id))
                    report.MissingDocumentIds.Add(id);
            }

            // ── 8. Note first-leaf docs that don't exist ───────────────────────
            foreach (var id in firstLeaf.DocumentIds)
                if (host.GetPanel(id) == null && !report.MissingDocumentIds.Contains(id))
                    report.MissingDocumentIds.Add(id);

            // ── 9. Activate selected documents ────────────────────────────────
            ActivateSelected(host, secondLeaf, report);
            ActivateSelected(host, firstLeaf,  report);   // first wins as "last set active"

            return report;
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
