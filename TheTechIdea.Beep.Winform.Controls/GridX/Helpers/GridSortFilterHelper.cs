using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridSortFilterHelper
    {
        private readonly BeepGridPro _grid;
        public GridSortFilterHelper(BeepGridPro grid) { _grid = grid; }

        public void Sort(string columnName, SortDirection direction)
        {
            var col = _grid.Data.Columns.FirstOrDefault(c => c.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            if (col == null) return;
            col.SortDirection = direction;

            Func<BeepRowConfig, object?> key = r => r.RowData?.GetType().GetProperty(col.ColumnName)?.GetValue(r.RowData);
            var ordered = direction switch
            {
                SortDirection.Ascending => _grid.Data.Rows.OrderBy(key).ToList(),
                SortDirection.Descending => _grid.Data.Rows.OrderByDescending(key).ToList(),
                _ => _grid.Data.Rows.ToList()
            };

            _grid.Data.Rows.Clear();
            int i = 0;
            foreach (var r in ordered)
            {
                r.DisplayIndex = i++;
                _grid.Data.Rows.Add(r);
            }
        }

        public void Filter(string columnName, string contains)
        {
            contains = contains?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(contains))
            {
                _grid.Data.RefreshRows();
                return;
            }

            var col = _grid.Data.Columns.FirstOrDefault(c => c.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            if (col == null) return;

            var all = _grid.Data.Rows.ToList();
            var filtered = all.Where(r => (r.RowData?.GetType().GetProperty(col.ColumnName)?.GetValue(r.RowData)?.ToString() ?? string.Empty)
                                        .Contains(contains, StringComparison.OrdinalIgnoreCase)).ToList();

            _grid.Data.Rows.Clear();
            for (int i = 0; i < filtered.Count; i++)
            {
                filtered[i].DisplayIndex = i;
                _grid.Data.Rows.Add(filtered[i]);
            }
        }
    }
}
