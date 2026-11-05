using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Query
{
    /// <summary>
    /// The current view of the grid: one ordered list of rows produced by
    /// <see cref="GridQueryEngine"/>. Renderers and exporters read this
    /// instead of re-deriving state from <c>IsVisible</c> and row order.
    /// </summary>
    public sealed class GridView
    {
        private readonly List<GridViewRow> _sourceRows;

        private GridView(IReadOnlyList<BeepColumnConfig> columns, List<GridViewRow> sourceRows,
            GridQuery? query = null)
        {
            Columns = columns;
            _sourceRows = sourceRows;
            Query = query ?? new GridQuery();
            Result = GridQueryEngine.Execute(_sourceRows, Query);
        }

        public IReadOnlyList<BeepColumnConfig> Columns { get; }
        public GridQuery Query { get; private set; }
        public GridQueryResult Result { get; private set; }
        public IReadOnlyList<GridViewRow> Rows => Result.Rows;
        public int TotalCount => Result.TotalCount;

        public void Apply(GridQuery query)
        {
            Query = query ?? new GridQuery();
            Result = GridQueryEngine.Execute(_sourceRows, Query);
        }

        internal static GridView Build(
            IEnumerable<BeepRowConfig> rows,
            IEnumerable<BeepColumnConfig> columns,
            bool includeHidden,
            GridQuery? query = null)
        {
            var columnList = columns?.Where(c => c != null).ToList() ?? new List<BeepColumnConfig>();
            var rowIdColumn = columnList.FirstOrDefault(c => c.IsRowID);
            var source = new List<GridViewRow>();

            int index = 0;
            foreach (var row in rows ?? Enumerable.Empty<BeepRowConfig>())
            {
                if (row == null) continue;
                if (!includeHidden && !row.IsVisible) continue;

                var values = new Dictionary<string, object?>(row.Cells.Count, StringComparer.OrdinalIgnoreCase);
                foreach (var cell in row.Cells)
                    values[cell.ColumnName] = cell.CellValue;

                long rowId = index;
                if (rowIdColumn != null &&
                    values.TryGetValue(rowIdColumn.ColumnName, out var idValue) &&
                    idValue is IConvertible convertible)
                {
                    try { rowId = convertible.ToInt64(System.Globalization.CultureInfo.InvariantCulture); }
                    catch (FormatException ex)
                    {
                        GridErrorReporter.FallbackOnce("GridView.RowId.Format", null, "read RowID cell", ex);
                        rowId = index;
                    }
                    catch (InvalidCastException ex)
                    {
                        GridErrorReporter.FallbackOnce("GridView.RowId.Cast", null, "read RowID cell", ex);
                        rowId = index;
                    }
                }

                source.Add(new GridViewRow
                {
                    RowId = rowId,
                    SourceIndex = index,
                    OriginalItem = row.RowData,
                    Values = values
                });
                index++;
            }

            return new GridView(columnList, source, query);
        }
    }
}
