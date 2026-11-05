using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Query
{
    /// <summary>
    /// Serialization-friendly description of the view the grid should show:
    /// filters, grouping, sorting, and paging. No UI types and no delegates,
    /// so a transport adapter can translate it to SQL, OData, or a service call.
    /// </summary>
    public sealed class GridQuery
    {
        public List<GridFilterGroup> Filters { get; set; } = new();
        public List<GroupDescriptor> Groups { get; set; } = new();
        public List<GridSortDescriptor> Sorts { get; set; } = new();
        public int Offset { get; set; }
        public int PageSize { get; set; } = -1;
        public long Revision { get; set; }
    }

    public sealed class GridFilterGroup
    {
        public FilterLogic Logic { get; set; } = FilterLogic.And;
        public List<GridFilterCondition> Conditions { get; set; } = new();
    }

    public sealed class GridFilterCondition
    {
        public string ColumnName { get; set; } = string.Empty;
        public FilterOperator Operator { get; set; } = FilterOperator.Equals;
        public string? Value { get; set; }
        public string? Value2 { get; set; }
    }

    public sealed class GridSortDescriptor
    {
        public string ColumnName { get; set; } = string.Empty;
        public SortDirection Direction { get; set; } = SortDirection.Ascending;
    }

    /// <summary>One row in the current view. Keys are column names.</summary>
    public sealed class GridViewRow
    {
        public long RowId { get; init; }
        public int SourceIndex { get; init; }
        public object? OriginalItem { get; init; }
        public Dictionary<string, object?> Values { get; init; } = new(StringComparer.OrdinalIgnoreCase);
        public List<string> GroupPath { get; init; } = new();
        public int DisplayIndex { get; internal set; }

        internal GridViewRow WithGroupPath(List<string> groupPath) => new()
        {
            RowId = RowId,
            SourceIndex = SourceIndex,
            OriginalItem = OriginalItem,
            Values = Values,
            GroupPath = groupPath,
            DisplayIndex = DisplayIndex
        };
    }

    public sealed class GridQueryResult
    {
        public IReadOnlyList<GridViewRow> Rows { get; init; } = Array.Empty<GridViewRow>();
        public int TotalCount { get; init; }
        public long Revision { get; init; }
    }

    /// <summary>
    /// Optional capability interface: a virtual source that can page, sort,
    /// filter, and group server-side instead of materializing everything.
    /// </summary>
    public interface IGridQuerySource
    {
        long TotalCount { get; }
        Task<GridQueryResult> QueryAsync(GridQuery query, CancellationToken cancellationToken);
        event EventHandler? QueryResultInvalidated;
    }
}
