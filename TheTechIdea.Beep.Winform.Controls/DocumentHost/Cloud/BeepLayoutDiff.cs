// BeepLayoutDiff.cs
// Structural diff between two BeepDocumentHost layout JSON snapshots.
// Detects added/removed/moved documents, split structure changes and ratio changes.
// Uses only System.Text.Json — no extra dependencies.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>Describes a change in split ratio between two layouts.</summary>
    public sealed record LayoutRatioChange(string NodeId, float OldRatio, float NewRatio);

    /// <summary>
    /// Result of comparing two layout JSON snapshots.
    /// </summary>
    public sealed class LayoutDiffResult
    {
        /// <summary>Document IDs present in B but not A.</summary>
        public IReadOnlyList<string> AddedDocIds   { get; init; } = Array.Empty<string>();

        /// <summary>Document IDs present in A but not B.</summary>
        public IReadOnlyList<string> RemovedDocIds { get; init; } = Array.Empty<string>();

        /// <summary>Document IDs that exist in both A and B but in different groups.</summary>
        public IReadOnlyList<string> MovedDocIds   { get; init; } = Array.Empty<string>();

        /// <summary>Split nodes where the ratio changed between A and B.</summary>
        public IReadOnlyList<LayoutRatioChange> RatioChanges { get; init; } =
            Array.Empty<LayoutRatioChange>();

        /// <summary>
        /// <c>true</c> when the tree topology (number / orientation of splits)
        /// changed between A and B.
        /// </summary>
        public bool StructureChanged { get; init; }

        /// <summary><c>true</c> when A and B are identical in all analysed dimensions.</summary>
        public bool IsIdentical =>
            AddedDocIds.Count   == 0 &&
            RemovedDocIds.Count == 0 &&
            MovedDocIds.Count   == 0 &&
            RatioChanges.Count  == 0 &&
            !StructureChanged;

        /// <summary>Returns a human-readable summary of the diff.</summary>
        public string Summary()
        {
            if (IsIdentical) return "Layouts are identical.";
            var parts = new List<string>();
            if (AddedDocIds.Count   > 0) parts.Add($"+{AddedDocIds.Count} doc(s)");
            if (RemovedDocIds.Count > 0) parts.Add($"-{RemovedDocIds.Count} doc(s)");
            if (MovedDocIds.Count   > 0) parts.Add($"{MovedDocIds.Count} moved");
            if (RatioChanges.Count  > 0) parts.Add($"{RatioChanges.Count} ratio change(s)");
            if (StructureChanged)        parts.Add("structure changed");
            return string.Join(", ", parts);
        }
    }

    /// <summary>
    /// Static utility that diffs two layout JSON strings produced by
    /// <see cref="BeepDocumentHost.SaveLayout"/>.
    /// </summary>
    public static class BeepLayoutDiff
    {
        /// <summary>
        /// Compares <paramref name="layoutJsonA"/> (old) with <paramref name="layoutJsonB"/> (new).
        /// </summary>
        public static LayoutDiffResult Compare(string layoutJsonA, string layoutJsonB)
        {
            if (string.IsNullOrWhiteSpace(layoutJsonA))
                return EmptyDiff();
            if (string.IsNullOrWhiteSpace(layoutJsonB))
                return EmptyDiff();

            try
            {
                using var docA = JsonDocument.Parse(layoutJsonA);
                using var docB = JsonDocument.Parse(layoutJsonB);

                var treeA = GetTree(docA.RootElement);
                var treeB = GetTree(docB.RootElement);

                // Collect docs per group in each snapshot
                var groupsA = new Dictionary<string, List<string>>();
                var groupsB = new Dictionary<string, List<string>>();
                var ratiosA = new Dictionary<string, float>();
                var ratiosB = new Dictionary<string, float>();
                int splitCountA = 0, splitCountB = 0;

                if (treeA.HasValue) WalkTree(treeA.Value, groupsA, ratiosA, ref splitCountA);
                if (treeB.HasValue) WalkTree(treeB.Value, groupsB, ratiosB, ref splitCountB);

                // Build flat doc→groupId maps
                var docGroupA = BuildDocGroupMap(groupsA);
                var docGroupB = BuildDocGroupMap(groupsB);

                var allDocsA = new HashSet<string>(docGroupA.Keys);
                var allDocsB = new HashSet<string>(docGroupB.Keys);

                var added   = new List<string>();
                var removed = new List<string>();
                var moved   = new List<string>();

                foreach (var id in allDocsB)
                    if (!allDocsA.Contains(id)) added.Add(id);

                foreach (var id in allDocsA)
                {
                    if (!allDocsB.Contains(id)) removed.Add(id);
                    else if (docGroupA[id] != docGroupB[id]) moved.Add(id);
                }

                // Ratio changes
                var ratioChanges = new List<LayoutRatioChange>();
                foreach (var (nodeId, oldRatio) in ratiosA)
                {
                    if (ratiosB.TryGetValue(nodeId, out float newRatio) &&
                        Math.Abs(newRatio - oldRatio) > 0.005f)
                    {
                        ratioChanges.Add(new LayoutRatioChange(nodeId, oldRatio, newRatio));
                    }
                }

                bool structureChanged = splitCountA != splitCountB;

                return new LayoutDiffResult
                {
                    AddedDocIds     = added,
                    RemovedDocIds   = removed,
                    MovedDocIds     = moved,
                    RatioChanges    = ratioChanges,
                    StructureChanged = structureChanged,
                };
            }
            catch
            {
                return new LayoutDiffResult { StructureChanged = true };
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static JsonElement? GetTree(JsonElement root)
        {
            if (root.TryGetProperty("layoutTree", out var tree)) return tree;
            if (root.TryGetProperty("LayoutTree", out tree))     return tree;
            return null;
        }

        private static void WalkTree(
            JsonElement node,
            Dictionary<string, List<string>> groups,
            Dictionary<string, float>        ratios,
            ref int splitCount)
        {
            if (!node.TryGetProperty("Type", out var typeProp) &&
                !node.TryGetProperty("type", out typeProp)) return;

            var type = typeProp.GetString() ?? "";

            if (type.Equals("tabGroup", StringComparison.OrdinalIgnoreCase) ||
                type.Equals("group",    StringComparison.OrdinalIgnoreCase))
            {
                var groupId = GetString(node, "NodeId") ?? GetString(node, "GroupId") ?? Guid.NewGuid().ToString();
                var docs    = new List<string>();

                if ((node.TryGetProperty("Documents", out var docsEl) ||
                     node.TryGetProperty("documents", out docsEl)) &&
                    docsEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var d in docsEl.EnumerateArray())
                    {
                        var id = GetString(d, "Id") ?? GetString(d, "id");
                        if (id != null) docs.Add(id);
                    }
                }

                groups[groupId] = docs;
            }
            else if (type.Equals("split", StringComparison.OrdinalIgnoreCase))
            {
                splitCount++;
                var nodeId = GetString(node, "NodeId") ?? Guid.NewGuid().ToString();
                if (node.TryGetProperty("Ratio", out var ratioEl) ||
                    node.TryGetProperty("ratio", out ratioEl))
                    ratios[nodeId] = ratioEl.GetSingle();

                // Walk children
                if ((node.TryGetProperty("Children", out var childrenEl) ||
                     node.TryGetProperty("children", out childrenEl)) &&
                    childrenEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var child in childrenEl.EnumerateArray())
                        WalkTree(child, groups, ratios, ref splitCount);
                }
            }
        }

        private static Dictionary<string, string> BuildDocGroupMap(
            Dictionary<string, List<string>> groups)
        {
            var map = new Dictionary<string, string>();
            foreach (var (groupId, docs) in groups)
                foreach (var docId in docs)
                    map[docId] = groupId;
            return map;
        }

        private static string? GetString(JsonElement el, string name) =>
            el.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String
                ? v.GetString()
                : null;

        private static LayoutDiffResult EmptyDiff() => new();
    }
}
