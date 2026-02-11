using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
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

            if (TryApplySourceSort(col, direction))
            {
                return;
            }

            // Never reorder local rows in UOW mode when source sort cannot be applied:
            // grid row index must stay aligned with UOW cursor/index operations.
            if (_grid.Uow != null)
            {
                _grid.SafeInvalidate();
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

        public void ClearFilters()
        {
            TryCommitPendingEdit();

            _containsFilters.Clear();
            _inFilters.Clear();
            foreach (var column in _grid.Data.Columns)
            {
                column.Filter = string.Empty;
            }
            UpdateFilteredColumnFlags();

            if (TryClearSourceFilter())
            {
                return;
            }

            SetAllRowsVisible();
            RecalculateAndInvalidate();
        }

        private BeepColumnConfig? FindColumn(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return null;
            return _grid.Data.Columns.FirstOrDefault(c => c.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
        }

        private void ApplyFilterState()
        {
            if (TryApplySourceFilter())
            {
                return;
            }

            ApplyLocalVisibilityFilter();
        }

        private bool TryApplySourceSort(BeepColumnConfig column, SortDirection direction)
        {
            if (column.IsSelectionCheckBox || column.IsRowNumColumn || column.IsRowID || column.IsUnbound)
            {
                return false;
            }

            var bs = _grid.Navigator.GetBindingSource();
            if (bs == null) return false;

            var descriptor = ResolvePropertyDescriptor(bs, column.ColumnName);
            if (descriptor != null && bs.List is IBindingList bindingList && bindingList.SupportsSorting)
            {
                try
                {
                    var listDirection = direction == SortDirection.Descending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;

                    bindingList.ApplySort(descriptor, listDirection);
                    bs.ResetBindings(false);
                    RebindFromBindingSource(bs);
                    return true;
                }
                catch
                {
                    // fall through to additional strategies
                }
            }

            if (bs.SupportsSorting)
            {
                try
                {
                    var expression = direction switch
                    {
                        SortDirection.Ascending => $"{EscapeIdentifier(column.ColumnName)} ASC",
                        SortDirection.Descending => $"{EscapeIdentifier(column.ColumnName)} DESC",
                        _ => string.Empty
                    };

                    bs.Sort = string.IsNullOrWhiteSpace(expression) ? null : expression;
                    RebindFromBindingSource(bs);
                    return true;
                }
                catch
                {
                    // fall through to additional strategies
                }
            }

            // In non-UOW mode, try reordering the underlying list when BindingSource.Sort is unavailable.
            if (_grid.Uow == null && TrySortUnderlyingList(bs, column.ColumnName, direction))
            {
                RebindFromBindingSource(bs);
                return true;
            }

            return false;
        }

        private bool TrySortUnderlyingList(BindingSource bs, string columnName, SortDirection direction)
        {
            if (direction != SortDirection.Ascending && direction != SortDirection.Descending)
            {
                return false;
            }

            if (bs.List is not IList list || list.IsReadOnly || list.Count <= 1)
            {
                return false;
            }

            var descriptor = ResolvePropertyDescriptor(bs, columnName);
            if (descriptor == null)
            {
                return false;
            }

            var items = list.Cast<object>().ToList();
            IOrderedEnumerable<object> ordered = direction == SortDirection.Ascending
                ? items.OrderBy(item => descriptor.GetValue(item), SortValueComparer)
                : items.OrderByDescending(item => descriptor.GetValue(item), SortValueComparer);

            var sortedItems = ordered.ToList();
            if (sortedItems.Count != list.Count)
            {
                return false;
            }

            list.Clear();
            foreach (var item in sortedItems)
            {
                list.Add(item);
            }

            bs.ResetBindings(false);
            return true;
        }

        private static PropertyDescriptor? ResolvePropertyDescriptor(BindingSource bs, string columnName)
        {
            try
            {
                var props = bs.GetItemProperties(null);
                var pd = props?.Find(columnName, true);
                if (pd != null) return pd;
            }
            catch
            {
                // Ignore, fallback below.
            }

            if (bs.Count > 0 && bs[0] != null)
            {
                return TypeDescriptor.GetProperties(bs[0]).Find(columnName, true);
            }

            return null;
        }

        private bool TryApplySourceFilter()
        {
            var bs = _grid.Navigator.GetBindingSource();
            if (bs == null)
            {
                return false;
            }

            if (bs.List is IBindingListView bindingListView && bindingListView.SupportsFiltering)
            {
                string bindingListExpression = BuildBindingListViewFilterExpression();

                try
                {
                    bindingListView.Filter = string.IsNullOrWhiteSpace(bindingListExpression) ? null : bindingListExpression;
                    bs.ResetBindings(false);
                    RebindFromBindingSource(bs);
                    return true;
                }
                catch
                {
                    // fall through to DataView-style filter path
                }
            }

            if (!bs.SupportsFiltering)
            {
                return false;
            }

            string dataViewExpression = BuildDataViewFilterExpression();

            try
            {
                bs.Filter = string.IsNullOrWhiteSpace(dataViewExpression) ? null : dataViewExpression;
                RebindFromBindingSource(bs);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryClearSourceFilter()
        {
            var bs = _grid.Navigator.GetBindingSource();
            if (bs == null)
            {
                return false;
            }

            if (bs.List is IBindingListView bindingListView && bindingListView.SupportsFiltering)
            {
                try
                {
                    bindingListView.Filter = null;
                    bs.ResetBindings(false);
                    RebindFromBindingSource(bs);
                    return true;
                }
                catch
                {
                    // fall through to BindingSource.Filter
                }
            }

            if (!bs.SupportsFiltering)
            {
                return false;
            }

            try
            {
                bs.Filter = null;
                RebindFromBindingSource(bs);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string BuildBindingListViewFilterExpression()
        {
            var clauses = new List<string>();

            foreach (var pair in _containsFilters)
            {
                if (string.IsNullOrWhiteSpace(pair.Value)) continue;

                string identifier = pair.Key;
                string value = EscapeBindingListLikeLiteral(pair.Value);
                clauses.Add($"{identifier} LIKE '%{value}%'");
            }

            foreach (var pair in _inFilters)
            {
                if (pair.Value == null || pair.Value.Count == 0) continue;

                string identifier = pair.Key;
                var values = pair.Value
                    .Select(v => $"{identifier} = '{EscapeBindingListStringLiteral(v)}'")
                    .ToList();

                if (values.Count > 0)
                {
                    clauses.Add($"({string.Join(" OR ", values)})");
                }
            }

            return string.Join(" AND ", clauses);
        }

        private string BuildDataViewFilterExpression()
        {
            var clauses = new List<string>();

            foreach (var pair in _containsFilters)
            {
                if (string.IsNullOrWhiteSpace(pair.Value)) continue;

                string identifier = EscapeIdentifier(pair.Key);
                string value = EscapeLikeLiteral(pair.Value);
                clauses.Add($"CONVERT({identifier}, 'System.String') LIKE '%{value}%'");
            }

            foreach (var pair in _inFilters)
            {
                if (pair.Value == null || pair.Value.Count == 0) continue;

                string identifier = EscapeIdentifier(pair.Key);
                var values = pair.Value
                    .Select(v => $"CONVERT({identifier}, 'System.String') = '{EscapeStringLiteral(v)}'")
                    .ToList();

                if (values.Count > 0)
                {
                    clauses.Add($"({string.Join(" OR ", values)})");
                }
            }

            return string.Join(" AND ", clauses);
        }

        private static string EscapeIdentifier(string columnName)
        {
            return "[" + (columnName ?? string.Empty).Replace("]", "]]") + "]";
        }

        private static string EscapeLikeLiteral(string value)
        {
            // DataView RowFilter LIKE escaping for literal wildcard chars.
            return EscapeStringLiteral(value)
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("*", "[*]");
        }

        private static string EscapeStringLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private static string EscapeBindingListStringLiteral(string value)
        {
            // ObservableBindingList filter parser expects raw values inside single quotes.
            // Do not SQL-escape quotes here.
            return value ?? string.Empty;
        }

        private static string EscapeBindingListLikeLiteral(string value)
        {
            // ObservableBindingList LIKE parser handles % wildcard directly.
            return EscapeBindingListStringLiteral(value);
        }

        private void ApplyLocalSort(string columnName, SortDirection direction)
        {
            if (direction != SortDirection.Ascending && direction != SortDirection.Descending)
            {
                return;
            }

            Func<BeepRowConfig, object?> key = row => GetColumnValue(row, columnName);
            var ordered = direction == SortDirection.Ascending
                ? _grid.Data.Rows.OrderBy(key, SortValueComparer).ToList()
                : _grid.Data.Rows.OrderByDescending(key, SortValueComparer).ToList();

            _grid.Data.Rows.Clear();
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].DisplayIndex = i;
                _grid.Data.Rows.Add(ordered[i]);
            }

            RecalculateAndInvalidate();
        }

        private void ApplyLocalVisibilityFilter()
        {
            bool hasFilters = _containsFilters.Count > 0 || _inFilters.Count > 0;

            for (int i = 0; i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                bool visible = !hasFilters || MatchesAllLocalFilters(row);
                row.IsVisible = visible;
            }

            EnsureSelectionVisible();
            RecalculateAndInvalidate();
        }

        private bool MatchesAllLocalFilters(BeepRowConfig row)
        {
            foreach (var pair in _containsFilters)
            {
                var value = Convert.ToString(GetColumnValue(row, pair.Key)) ?? string.Empty;
                if (!value.Contains(pair.Value, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            foreach (var pair in _inFilters)
            {
                var value = Convert.ToString(GetColumnValue(row, pair.Key)) ?? string.Empty;
                if (!pair.Value.Contains(value))
                {
                    return false;
                }
            }

            return true;
        }

        private static object? GetColumnValue(BeepRowConfig row, string columnName)
        {
            var cell = row.Cells.FirstOrDefault(c => string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
            return cell?.CellValue;
        }

        private void RebindFromBindingSource(BindingSource bs)
        {
            TryCommitPendingEdit();

            object? selectedRowData = null;
            int selectedRow = _grid.Selection.RowIndex;
            int selectedCol = _grid.Selection.HasSelection ? _grid.Selection.ColumnIndex : 0;

            if (_grid.Selection.HasSelection &&
                selectedRow >= 0 &&
                selectedRow < _grid.Rows.Count)
            {
                selectedRowData = _grid.Rows[selectedRow].RowData;
            }

            // Keep binding intact when unchanged (important for ObservableBindingList/UOW mode).
            if (!ReferenceEquals(_grid.Data.DataSource, bs))
            {
                _grid.Data.Bind(bs, triggerAutoSize: false);
                _grid.Data.InitializeData();
            }
            else
            {
                _grid.Data.RefreshRows();
            }

            SetAllRowsVisible();

            if (_grid.Rows.Count > 0)
            {
                int newRow = ResolveSelectionRow(selectedRowData, selectedRow);
                int col = Math.Max(0, Math.Min(selectedCol, Math.Max(0, _grid.Columns.Count - 1)));
                _grid.SelectCell(newRow, col);
            }
            else
            {
                _grid.Selection.Clear();
            }

            RecalculateAndInvalidate();
        }

        private int ResolveSelectionRow(object? selectedRowData, int fallbackRow)
        {
            if (selectedRowData != null)
            {
                for (int i = 0; i < _grid.Rows.Count; i++)
                {
                    var rowData = _grid.Rows[i].RowData;
                    if (ReferenceEquals(rowData, selectedRowData) || Equals(rowData, selectedRowData))
                    {
                        return i;
                    }
                }
            }

            if (fallbackRow < 0) return 0;
            return Math.Max(0, Math.Min(fallbackRow, _grid.Rows.Count - 1));
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
            catch
            {
                // Best-effort commit only.
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
                catch
                {
                    // fallback to string compare below
                }

                string xs = Convert.ToString(x) ?? string.Empty;
                string ys = Convert.ToString(y) ?? string.Empty;
                return string.Compare(xs, ys, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
