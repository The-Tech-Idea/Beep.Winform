// LayoutTreeValidator.cs
// Validates the structural and semantic integrity of an ILayoutNode tree.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout
{
    // ─────────────────────────────────────────────────────────────────────────
    // ValidationReport
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Describes the result of a <see cref="LayoutTreeValidator.Validate"/> call.
    /// </summary>
    public sealed class ValidationReport
    {
        /// <summary><c>true</c> when no errors were found.</summary>
        public bool IsValid => Errors.Count == 0;

        /// <summary>Human-readable descriptions of every detected problem.</summary>
        public List<string> Errors { get; } = new List<string>();

        /// <summary>Non-fatal advisories (e.g. empty groups, deep trees).</summary>
        public List<string> Warnings { get; } = new List<string>();

        internal void AddError(string message)   => Errors.Add(message);
        internal void AddWarning(string message) => Warnings.Add(message);

        /// <inheritdoc/>
        public override string ToString()
            => IsValid
                ? $"Valid ({Warnings.Count} warning(s))"
                : $"Invalid — {Errors.Count} error(s), {Warnings.Count} warning(s)";
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Validator
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Stateless validator that inspects an <see cref="ILayoutNode"/> tree for
    /// structural and semantic errors.
    /// </summary>
    public static class LayoutTreeValidator
    {
        /// <summary>
        /// Maximum tree depth before a warning is emitted.  The validator does
        /// not treat exceeding this as an error.
        /// </summary>
        public const int MaxRecommendedDepth = 8;

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Validates <paramref name="root"/> and returns a <see cref="ValidationReport"/>
        /// describing every error and warning found.
        /// </summary>
        /// <param name="root">Root of the layout tree.  Must not be null.</param>
        public static ValidationReport Validate(ILayoutNode root)
        {
            ArgumentNullException.ThrowIfNull(root);

            var report = new ValidationReport();
            var seenIds      = new HashSet<string>(StringComparer.Ordinal);
            var seenDocIds   = new HashSet<string>(StringComparer.Ordinal);

            ValidateNode(root, report, seenIds, seenDocIds, depth: 1);

            if (root.Depth() > MaxRecommendedDepth)
                report.AddWarning(
                    $"Tree depth {root.Depth()} exceeds recommended maximum of {MaxRecommendedDepth}.");

            return report;
        }

        // ── Recursive worker ─────────────────────────────────────────────────

        private static void ValidateNode(
            ILayoutNode        node,
            ValidationReport   report,
            HashSet<string>    seenNodeIds,
            HashSet<string>    seenDocIds,
            int                depth)
        {
            if (node == null)
            {
                report.AddError($"Null child node encountered at depth {depth}.");
                return;
            }

            // Unique node IDs
            if (string.IsNullOrWhiteSpace(node.NodeId))
            {
                report.AddError($"Node at depth {depth} has a null or empty NodeId.");
            }
            else if (!seenNodeIds.Add(node.NodeId))
            {
                report.AddError($"Duplicate NodeId '{node.NodeId}' at depth {depth}.");
            }

            if (node is SplitLayoutNode split)
                ValidateSplit(split, report, seenNodeIds, seenDocIds, depth);
            else if (node is GroupLayoutNode group)
                ValidateGroup(group, report, seenDocIds, depth);
            else
                report.AddError(
                    $"Unknown node type '{node.GetType().Name}' at depth {depth}.");
        }

        private static void ValidateSplit(
            SplitLayoutNode  split,
            ValidationReport report,
            HashSet<string>  seenNodeIds,
            HashSet<string>  seenDocIds,
            int              depth)
        {
            if (split.First == null)
                report.AddError(
                    $"SplitLayoutNode '{split.NodeId}' (depth {depth}) has a null First child.");

            if (split.Second == null)
                report.AddError(
                    $"SplitLayoutNode '{split.NodeId}' (depth {depth}) has a null Second child.");

            if (split.Ratio < 0.1f || split.Ratio > 0.9f)
                report.AddError(
                    $"SplitLayoutNode '{split.NodeId}' Ratio {split.Ratio:F2} is outside [0.10, 0.90].");

            // Detect trivial same-reference child (circular reference guard)
            if (split.First != null && ReferenceEquals(split.First, split.Second))
                report.AddError(
                    $"SplitLayoutNode '{split.NodeId}' First and Second reference the same node.");

            if (split.First  != null)
                ValidateNode(split.First,  report, seenNodeIds, seenDocIds, depth + 1);

            if (split.Second != null)
                ValidateNode(split.Second, report, seenNodeIds, seenDocIds, depth + 1);
        }

        private static void ValidateGroup(
            GroupLayoutNode  group,
            ValidationReport report,
            HashSet<string>  seenDocIds,
            int              depth)
        {
            if (string.IsNullOrWhiteSpace(group.GroupId))
                report.AddError(
                    $"GroupLayoutNode '{group.NodeId}' (depth {depth}) has a null/empty GroupId.");

            if (group.IsEmpty)
            {
                report.AddWarning(
                    $"GroupLayoutNode '{group.NodeId}' (depth {depth}) is empty.");
                return;
            }

            var localDocIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var docId in group.DocumentIds)
            {
                if (string.IsNullOrWhiteSpace(docId))
                {
                    report.AddError(
                        $"GroupLayoutNode '{group.NodeId}' contains a null/empty document ID.");
                    continue;
                }

                // Intra-group duplicate
                if (!localDocIds.Add(docId))
                {
                    report.AddError(
                        $"GroupLayoutNode '{group.NodeId}' contains duplicate document ID '{docId}'.");
                    continue;
                }

                // Cross-group duplicate
                if (!seenDocIds.Add(docId))
                {
                    report.AddError(
                        $"Document ID '{docId}' appears in multiple groups (second occurrence in '{group.NodeId}').");
                }
            }

            // SelectedDocumentId must be in the group (if set)
            if (!string.IsNullOrEmpty(group.SelectedDocumentId)
                && !localDocIds.Contains(group.SelectedDocumentId))
            {
                report.AddError(
                    $"GroupLayoutNode '{group.NodeId}' SelectedDocumentId '{group.SelectedDocumentId}' " +
                    $"is not in DocumentIds.");
            }
        }
    }
}
