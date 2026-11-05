using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridSortFilterHelper
    {
        private readonly BeepGridPro _grid;
        private readonly Dictionary<string, string> _containsFilters = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, HashSet<string>> _inFilters = new(StringComparer.OrdinalIgnoreCase);
        private static readonly IComparer<object?> SortValueComparer = new SafeObjectComparer();

        public GridSortFilterHelper(BeepGridPro grid) { _grid = grid; }

        public void Sort(string columnName, SortDirection direction)
        {
            TryCommitPendingEdit();

            var col = FindColumn(columnName);
            if (col == null) return;

            col.SortDirection = direction;
            col.IsSorted = direction != SortDirection.None;
            if (direction == SortDirection.None)
                col.SortOrder = 0;

            // When grouping is active, do not globally reorder rows — sort within groups instead.
            if (_grid.GroupEngine.IsGrouped)
            {
                var groupDescriptor = _grid.GroupEngine.Descriptors
                    .FirstOrDefault(d => string.Equals(d.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));

                if (groupDescriptor != null)
                {
                    // Sorting a group column: update the group descriptor and recompute groups
                    var groupDir = direction == SortDirection.Descending
                        ? GroupSortDirection.Descending
                        : GroupSortDirection.Ascending;
                    _grid.GroupEngine.SortGroupsByColumn(columnName, groupDir);
                }
                else
                {
                    // Sorting a non-group column: sort rows inside each group
                    _grid.GroupEngine.SortWithinGroups(columnName, direction);
                }
                return;
            }

            ApplyLocalSort(col.ColumnName, direction);
        }

        public void Filter(string columnName, string contains)
        {
            TryCommitPendingEdit();

            var col = FindColumn(columnName);
            if (col == null) return;

            contains = contains?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(contains))
            {
                _containsFilters.Remove(col.ColumnName);
                col.Filter = string.Empty;
            }
            else
            {
                _containsFilters[col.ColumnName] = contains;
                col.Filter = contains;
            }

            UpdateFilteredColumnFlags();

            ApplyFilterState();
        }

        public void FilterIn(string columnName, IEnumerable<object>? selectedValues)
        {
            TryCommitPendingEdit();

            var col = FindColumn(columnName);
            if (col == null) return;

            var valueSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (selectedValues != null)
            {
                foreach (var value in selectedValues)
                {
                    valueSet.Add(Convert.ToString(value) ?? string.Empty);
                }
            }

            if (valueSet.Count == 0)
            {
                _inFilters.Remove(col.ColumnName);
                col.Filter = string.Empty;
            }
            else
            {
                _inFilters[col.ColumnName] = valueSet;
                col.Filter = valueSet.Count <= 3
                    ? string.Join(", ", valueSet)
                    : $"{valueSet.Count} selected";
            }

            UpdateFilteredColumnFlags();

            ApplyFilterState();
        }

        /// <summary>
        /// Clears the per-column contains/in filters, and the grid's ActiveFilter with them.
        /// </summary>
        /// <remarks>
        /// See <c>BeepGridPro.ClearFilter</c>: the two filter systems share no state but write the
        /// same <c>row.IsVisible</c>. Clearing here without clearing there left the grid showing
        /// every row while still reporting IsFiltered = true.
        /// </remarks>
        public void ClearFilters()
        {
            TryCommitPendingEdit();

            if (!_grid._isClearingAllFilters && _grid.IsFiltered)
            {
                _grid._isClearingAllFilters = true;
                try { _grid.ClearFilter(); }
                finally { _grid._isClearingAllFilters = false; }
            }

            _containsFilters.Clear();
            _inFilters.Clear();
            foreach (var column in _grid.Data.Columns)
            {
                column.Filter = string.Empty;
            }
            UpdateFilteredColumnFlags();

            SetAllRowsVisible();
            RecalculateAndInvalidate();
        }

        /// <summary>
        /// True when any per-column contains/in filter is in force, even when the
        /// grid's <see cref="BeepGridPro.ActiveFilter"/> is empty.
        /// </summary>
        public bool HasActiveFilters => _containsFilters.Count > 0 || _inFilters.Count > 0;

        /// <summary>
        /// Clears the contains/in filters for a single column and re-applies the
        /// remaining filter state. Used by the active-filter chips row so each
        /// per-column chip's "x" removes just that column's condition.
        /// </summary>
        public void ClearColumnFilter(string columnName)
        {
            var col = FindColumn(columnName);
            if (col == null) return;

            _containsFilters.Remove(col.ColumnName);
            _inFilters.Remove(col.ColumnName);
            col.Filter = string.Empty;

            UpdateFilteredColumnFlags();
            ApplyFilterState();
        }

        /// <summary>
        /// Returns the active per-column filter conditions as
        /// <see cref="FilterCriteria"/> objects, one per column, so the
        /// active-filter chips row can render them identically to
        /// <see cref="BeepGridPro.ActiveFilter"/> criteria.
        /// </summary>
        internal void AppendQueryConditions(Query.GridQuery query)
        {
            var group = new Query.GridFilterGroup { Logic = FilterLogic.And };

            foreach (var pair in _containsFilters)
            {
                if (string.IsNullOrWhiteSpace(pair.Value)) continue;
                group.Conditions.Add(new Query.GridFilterCondition
                {
                    ColumnName = pair.Key,
                    Operator = FilterOperator.Contains,
                    Value = pair.Value
                });
            }

            foreach (var pair in _inFilters)
            {
                if (pair.Value == null || pair.Value.Count == 0) continue;
                group.Conditions.Add(new Query.GridFilterCondition
                {
                    ColumnName = pair.Key,
                    Operator = FilterOperator.In,
                    Value = string.Join("\u001F", pair.Value)
                });
            }

            if (group.Conditions.Count > 0)
                query.Filters.Add(group);
        }

        public IReadOnlyList<FilterCriteria> GetActiveColumnCriteria()
        {
            var criteria = new List<FilterCriteria>();

            foreach (var pair in _containsFilters)
            {
                criteria.Add(new FilterCriteria(pair.Key, FilterOperator.Contains, pair.Value));
            }

            foreach (var pair in _inFilters)
            {
                criteria.Add(new FilterCriteria(pair.Key, FilterOperator.In, string.Join(", ", pair.Value)));
            }

            return criteria;
        }

        private BeepColumnConfig? FindColumn(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return null;
            return _grid.Data.Columns.FirstOrDefault(c => c.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
        }

        private void ApplyFilterState()
        {
            ApplyLocalVisibilityFilter();
        }

        /// <summary>
        /// Re-applies the current per-column filters after the grid's active
        /// search filter was removed. Keeps unrelated column filters intact.
        /// </summary>
        internal void ReapplyCurrentFilters() => ApplyFilterState();

        internal void PruneUnknownColumns()
        {
            var known = new HashSet<string>(
                _grid.Data.Columns.Select(c => c.ColumnName),
                StringComparer.OrdinalIgnoreCase);

            foreach (var key in _containsFilters.Keys.Where(key => !known.Contains(key)).ToList())
                _containsFilters.Remove(key);

            foreach (var key in _inFilters.Keys.Where(key => !known.Contains(key)).ToList())
                _inFilters.Remove(key);
        }

        internal bool HasActiveSort => _grid.Data.Columns.Any(c => c != null &&
            c.IsSorted && c.SortDirection != SortDirection.None);

        internal void ReapplyView()
        {
            if (HasActiveSort)
                ApplyLocalSort(string.Empty, SortDirection.None);

            if (HasActiveFilters || (_grid.ActiveFilter?.Criteria.Count > 0))
                ApplyLocalVisibilityFilter();
            else
                RecalculateAndInvalidate();
        }

        private void ApplyLocalSort(string columnName, SortDirection direction)
        {
            var sortedColumns = _grid.Data.Columns
                .Where(c => c != null && c.IsSorted && c.SortDirection != SortDirection.None)
                .OrderBy(c => c.SortOrder <= 0 ? int.MaxValue : c.SortOrder)
                .ToList();

            // Nothing sorted (the trigger was a "clear sort") — leave the current row order alone.
            if (sortedColumns.Count == 0)
            {
                RecalculateAndInvalidate();
                return;
            }

            var query = new Query.GridQuery();
            foreach (var col in sortedColumns)
            {
                query.Sorts.Add(new Query.GridSortDescriptor
                {
                    ColumnName = col.ColumnName,
                    Direction = col.SortDirection
                });
            }

            var sourceRows = _grid.Data.Rows.ToList();
            var view = Query.GridView.Build(_grid.Data.Rows, _grid.Data.Columns, includeHidden: true);
            view.Apply(query);

            _grid.Data.Rows.Clear();
            for (int i = 0; i < view.Rows.Count; i++)
            {
                var row = sourceRows[view.Rows[i].SourceIndex];
                row.DisplayIndex = i;
                _grid.Data.Rows.Add(row);
            }

            RecalculateAndInvalidate();
        }

        private void ApplyLocalVisibilityFilter()
        {
            bool hasFilters = _containsFilters.Count > 0 || _inFilters.Count > 0 ||
                              (_grid.ActiveFilter?.Criteria.Count > 0);

            if (!hasFilters)
            {
                for (int i = 0; i < _grid.Data.Rows.Count; i++)
                    _grid.Data.Rows[i].IsVisible = true;
            }
            else
            {
                var query = new Query.GridQuery();
                if (_grid.ActiveFilter is { Criteria.Count: > 0 } activeFilter)
                {
                    foreach (var activeGroup in BeepGridPro.BuildQuery(activeFilter).Filters)
                        query.Filters.Add(activeGroup);
                }

                var group = new Query.GridFilterGroup { Logic = FilterLogic.And };

                foreach (var pair in _containsFilters)
                {
                    if (string.IsNullOrWhiteSpace(pair.Value)) continue;
                    group.Conditions.Add(new Query.GridFilterCondition
                    {
                        ColumnName = pair.Key,
                        Operator = FilterOperator.Contains,
                        Value = pair.Value
                    });
                }

                foreach (var pair in _inFilters)
                {
                    if (pair.Value == null || pair.Value.Count == 0) continue;
                    group.Conditions.Add(new Query.GridFilterCondition
                    {
                        ColumnName = pair.Key,
                        Operator = FilterOperator.In,
                        Value = string.Join("\u001F", pair.Value)
                    });
                }

                query.Filters.Add(group);

                var view = Query.GridView.Build(_grid.Data.Rows, _grid.Data.Columns, includeHidden: true);
                view.Apply(query);
                var visibleSources = new HashSet<int>(view.Rows.Select(r => r.SourceIndex));

                for (int i = 0; i < _grid.Data.Rows.Count; i++)
                    _grid.Data.Rows[i].IsVisible = visibleSources.Contains(i);
            }

            _grid.Layout.RowMetrics.Invalidate();
            EnsureSelectionVisible();
            RecalculateAndInvalidate();
        }

        private void SetAllRowsVisible()
        {
            for (int i = 0; i < _grid.Data.Rows.Count; i++)
            {
                _grid.Data.Rows[i].IsVisible = true;
            }
        }

        private void EnsureSelectionVisible()
        {
            if (_grid.Rows.Count == 0)
            {
                _grid.Selection.Clear();
                return;
            }

            if (_grid.Selection.HasSelection &&
                _grid.Selection.RowIndex >= 0 &&
                _grid.Selection.RowIndex < _grid.Rows.Count &&
                _grid.Rows[_grid.Selection.RowIndex].IsVisible)
            {
                return;
            }

            int visibleRow = -1;
            for (int i = 0; i < _grid.Rows.Count; i++)
            {
                if (_grid.Rows[i].IsVisible)
                {
                    visibleRow = i;
                    break;
                }
            }

            if (visibleRow < 0)
            {
                _grid.Selection.Clear();
                return;
            }

            int col = _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0;
            col = Math.Max(0, Math.Min(col, Math.Max(0, _grid.Columns.Count - 1)));
            _grid.SelectCell(visibleRow, col);
            _grid.Selection.EnsureVisible();
        }

        private void RecalculateAndInvalidate()
        {
            _grid.Footer.Invalidate();
            _grid.Layout.Recalculate();
            _grid.ScrollBars?.UpdateBars();
            _grid.SafeInvalidate();
            _grid.RequestAutoSize(AutoSizeTriggerSource.SortFilter);
        }

        private void UpdateFilteredColumnFlags()
        {
            var filteredColumns = new HashSet<string>(_containsFilters.Keys, StringComparer.OrdinalIgnoreCase);
            foreach (var key in _inFilters.Keys)
            {
                filteredColumns.Add(key);
            }

            foreach (var column in _grid.Data.Columns)
            {
                bool isFiltered = filteredColumns.Contains(column.ColumnName);
                column.IsFiltered = isFiltered;
                if (!isFiltered)
                {
                    column.Filter = string.Empty;
                }
            }
        }

        private void TryCommitPendingEdit()
        {
            try
            {
                _grid.Edit.EndEdit(true);
            }
            catch (Exception ex)
            {
                // Absorbed: this sits on a UI path where throwing would take the
                // window down. Reported, because silently doing nothing here looks
                // to the user exactly like success.
                _grid?.ReportOperationError("Sort.CommitPendingEdit", ex);
            }
        }

        private sealed class SafeObjectComparer : IComparer<object?>
        {
            public int Compare(object? x, object? y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                try
                {
                    if (x is IComparable comparable)
                    {
                        return comparable.CompareTo(y);
                    }
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidCastException)
                {
                    GridErrorReporter.FallbackOnce("SortFilter.Comparer", null,
                        "compare mixed-type sort values", ex);
                    // IComparable.CompareTo throws ArgumentException when the other value is a
                    // different type - a mixed-type column. String comparison below still
                    // orders it sensibly.
                }

                string xs = Convert.ToString(x) ?? string.Empty;
                string ys = Convert.ToString(y) ?? string.Empty;
                return string.Compare(xs, ys, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
