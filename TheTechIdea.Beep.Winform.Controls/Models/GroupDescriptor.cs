using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.GridX.Grouping;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    /// <summary>
    /// Describes how rows should be grouped in <see cref="GridX.BeepGridPro"/>.
    /// </summary>
    public sealed class GroupDescriptor
    {
        /// <summary>
        /// Column name to group by.
        /// </summary>
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// Display title for the group header. If empty, uses <see cref="ColumnName"/>.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Sort direction within the group. Default: Ascending.
        /// </summary>
        public GroupSortDirection SortDirection { get; set; } = GroupSortDirection.Ascending;

        /// <summary>
        /// Whether the group is initially collapsed. Default: false.
        /// </summary>
        public bool CollapsedByDefault { get; set; } = false;

        /// <summary>
        /// Optional format string for the group key value (e.g., "yyyy-MM" for monthly date grouping).
        /// </summary>
        public string? ValueFormat { get; set; }

        /// <summary>
        /// Human-readable title for the group header.
        /// </summary>
        public string DisplayTitle => !string.IsNullOrWhiteSpace(Title) ? Title : ColumnName;
    }

    /// <summary>
    /// Sort direction applied to rows within a group.
    /// </summary>
    public enum GroupSortDirection
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// Represents a single group at runtime, tracking its rows and collapsed state.
    /// </summary>
    public sealed class GridGroup
    {
        /// <summary>
        /// Unique key for this group (typically the raw group value).
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable label shown in the group header.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Zero-based indices of rows that belong to this group (into the original row collection).
        /// </summary>
        public List<int> RowIndices { get; set; } = new List<int>();

        /// <summary>
        /// Whether the group is currently collapsed (children hidden).
        /// </summary>
        public bool IsCollapsed { get; set; } = false;

        /// <summary>
        /// Optional aggregate values computed for this group.
        /// </summary>
        public Dictionary<string, object?> Aggregates { get; set; } = new Dictionary<string, object?>();

        /// <summary>
        /// Nesting level (0 = top-level group, 1 = nested inside another group, etc.).
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// Index of the first data row that belongs to this group (into the original row collection).
        /// Used by the layout engine to compute header positions.
        /// </summary>
        public int FirstRowIndex { get; set; } = -1;

        /// <summary>
        /// Optional summary row shown after the group's rows when the group is expanded.
        /// Computed by <see cref="GridGroupAggregateEngine.ComputeForGroup"/>.
        /// </summary>
        public GridGroupSummaryRow? SummaryRow { get; set; }
    }
}
