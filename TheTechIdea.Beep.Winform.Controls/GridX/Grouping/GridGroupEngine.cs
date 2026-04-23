using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Grouping
{
    /// <summary>
    /// Core engine for computing, sorting, and managing row groups in <see cref="BeepGridPro"/>.
    /// </summary>
    public sealed class GridGroupEngine
    {
        private readonly BeepGridPro _grid;
        private readonly List<GroupDescriptor> _descriptors = new();
        private readonly List<GridGroup> _groups = new();
        private IGridGrouper _grouper = DefaultGridGrouper.Instance;

        public GridGroupEngine(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        /// <summary>
        /// Currently active group descriptors (in nesting order).
        /// </summary>
        public IReadOnlyList<GroupDescriptor> Descriptors => _descriptors;

        /// <summary>
        /// Computed groups from the last <see cref="ApplyGrouping()"/> call.
        /// </summary>
        public IReadOnlyList<GridGroup> Groups => _groups;

        /// <summary>
        /// Whether grouping is currently active (at least one descriptor is registered).
        /// </summary>
        public bool IsGrouped => _descriptors.Count > 0;

        /// <summary>
        /// The grouper implementation used to extract keys and labels. Default: <see cref="DefaultGridGrouper"/>.
        /// </summary>
        public IGridGrouper Grouper
        {
            get => _grouper;
            set => _grouper = value ?? DefaultGridGrouper.Instance;
        }

        /// <summary>
        /// Add a group descriptor and recompute groups.
        /// </summary>
        public void AddDescriptor(GroupDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            _descriptors.Add(descriptor);
            ApplyGrouping();
        }

        /// <summary>
        /// Remove all group descriptors and restore original row visibility.
        /// </summary>
        public void ClearDescriptors()
        {
            _descriptors.Clear();
            _groups.Clear();
            RestoreAllRowsVisible();
        }

        /// <summary>
        /// Apply a complete set of descriptors, replacing any existing ones.
        /// </summary>
        public void SetDescriptors(IEnumerable<GroupDescriptor> descriptors)
        {
            _descriptors.Clear();
            _descriptors.AddRange(descriptors ?? throw new ArgumentNullException(nameof(descriptors)));
            ApplyGrouping();
        }

        /// <summary>
        /// Toggle the collapsed state of a group by its key.
        /// </summary>
        public bool ToggleCollapse(string groupKey)
        {
            var group = _groups.FirstOrDefault(g => g.Key == groupKey);
            if (group == null) return false;

            group.IsCollapsed = !group.IsCollapsed;
            SyncRowVisibility();
            return true;
        }

        /// <summary>
        /// Expand all groups.
        /// </summary>
        public void ExpandAll()
        {
            foreach (var g in _groups) g.IsCollapsed = false;
            SyncRowVisibility();
        }

        /// <summary>
        /// Collapse all groups.
        /// </summary>
        public void CollapseAll()
        {
            foreach (var g in _groups) g.IsCollapsed = true;
            SyncRowVisibility();
        }

        /// <summary>
        /// Recompute groups from the current descriptors and row data.
        /// </summary>
        public void ApplyGrouping()
        {
            _groups.Clear();

            if (_descriptors.Count == 0 || _grid.Data.Rows.Count == 0)
            {
                RestoreAllRowsVisible();
                return;
            }

            // Build nested groups from currently visible rows (respects active filter)
            var visibleRowIndices = Enumerable.Range(0, _grid.Data.Rows.Count)
                .Where(i => _grid.Data.Rows[i].IsVisible)
                .ToList();
            BuildGroups(0, visibleRowIndices, 0);

            // Set FirstRowIndex for every group and sort by it (parents before children at same row)
            foreach (var group in _groups)
            {
                group.FirstRowIndex = group.RowIndices.Count > 0 ? group.RowIndices.Min() : -1;
            }
            _groups.Sort((a, b) =>
            {
                int cmp = a.FirstRowIndex.CompareTo(b.FirstRowIndex);
                return cmp != 0 ? cmp : a.Level.CompareTo(b.Level);
            });

            // Sort groups and rows inside each group
            SortGroupsAndRows();

            // Apply any active non-group column sorts within groups
            ApplyActiveColumnSortsWithinGroups();

            // Compute summary rows for every group
            foreach (var group in _groups)
            {
                GridGroupAggregateEngine.ComputeForGroup(_grid, group);
            }

            // Sync visibility
            SyncRowVisibility();
        }

        private void BuildGroups(int descriptorIndex, List<int> rowIndices, int level)
        {
            if (descriptorIndex >= _descriptors.Count)
                return;

            var descriptor = _descriptors[descriptorIndex];
            var column = _grid.Data.Columns.FirstOrDefault(c => c.ColumnName == descriptor.ColumnName);
            if (column == null)
                return;

            // Bucket rows by group key
            var buckets = new Dictionary<string, List<int>>();
            foreach (var ri in rowIndices)
            {
                var row = _grid.Data.Rows[ri];
                var cell = row.Cells.FirstOrDefault(c => c.ColumnName == descriptor.ColumnName);
                var key = _grouper.GetGroupKey(cell?.CellValue, descriptor);

                if (!buckets.TryGetValue(key, out var list))
                {
                    list = new List<int>();
                    buckets[key] = list;
                }
                list.Add(ri);
            }

            // Create group objects
            foreach (var kv in buckets)
            {
                var group = new GridGroup
                {
                    Key = kv.Key,
                    Label = _grouper.GetGroupLabel(kv.Key, descriptor),
                    RowIndices = kv.Value,
                    IsCollapsed = descriptor.CollapsedByDefault,
                    Level = level
                };

                // Recurse for nested descriptors
                if (descriptorIndex + 1 < _descriptors.Count)
                {
                    BuildGroups(descriptorIndex + 1, group.RowIndices, level + 1);
                }

                _groups.Add(group);
            }
        }

        private void SortGroupsAndRows()
        {
            foreach (var descriptor in _descriptors)
            {
                // Sort rows inside each group according to descriptor.SortDirection
                var colIndex = _grid.Data.Columns
                    .Select((c, i) => new { c, i })
                    .FirstOrDefault(x => x.c.ColumnName == descriptor.ColumnName)?.i ?? -1;
                if (colIndex < 0) continue;

                foreach (var group in _groups.Where(g => g.Level == _descriptors.IndexOf(descriptor)))
                {
                    group.RowIndices.Sort((a, b) =>
                    {
                        var va = _grid.Data.Rows[a].Cells[colIndex].CellValue;
                        var vb = _grid.Data.Rows[b].Cells[colIndex].CellValue;
                        var cmp = CompareValues(va, vb);
                        return descriptor.SortDirection == GroupSortDirection.Ascending ? cmp : -cmp;
                    });
                }
            }
        }

        private void SyncRowVisibility()
        {
            bool isFiltered = _grid.IsFiltered;
            var filteredSet = _grid.FilteredRowIndices != null ? new System.Collections.Generic.HashSet<int>(_grid.FilteredRowIndices) : null;

            // Hide all rows first
            foreach (var row in _grid.Data.Rows)
                row.IsVisible = false;

            // Show rows in non-collapsed groups that also pass the active filter
            foreach (var group in _groups)
            {
                if (!group.IsCollapsed)
                {
                    foreach (var ri in group.RowIndices)
                    {
                        if (ri >= 0 && ri < _grid.Data.Rows.Count)
                        {
                            bool passesFilter = !isFiltered || (filteredSet != null && filteredSet.Contains(ri));
                            _grid.Data.Rows[ri].IsVisible = passesFilter;
                        }
                    }
                }
            }

            _grid.Layout.Recalculate();
            _grid.ScrollBars?.UpdateBars();
            _grid.SafeInvalidate();
        }

        private void RestoreAllRowsVisible()
        {
            bool isFiltered = _grid.IsFiltered;
            var filteredSet = _grid.FilteredRowIndices != null ? new System.Collections.Generic.HashSet<int>(_grid.FilteredRowIndices) : null;

            for (int i = 0; i < _grid.Data.Rows.Count; i++)
            {
                _grid.Data.Rows[i].IsVisible = !isFiltered || (filteredSet != null && filteredSet.Contains(i));
            }

            _grid.Layout.Recalculate();
            _grid.ScrollBars?.UpdateBars();
            _grid.SafeInvalidate();
        }

        /// <summary>
        /// Sorts rows inside every group by an arbitrary column/direction.
        /// Preserves group structure; only reorders row indices within each group.
        /// </summary>
        public void SortWithinGroups(string columnName, SortDirection direction)
        {
            var colIndex = _grid.Data.Columns
                .Select((c, i) => new { c, i })
                .FirstOrDefault(x => x.c.ColumnName == columnName)?.i ?? -1;
            if (colIndex < 0) return;

            foreach (var group in _groups)
            {
                group.RowIndices.Sort((a, b) =>
                {
                    var va = _grid.Data.Rows[a].Cells[colIndex].CellValue;
                    var vb = _grid.Data.Rows[b].Cells[colIndex].CellValue;
                    var cmp = CompareValues(va, vb);
                    return direction == SortDirection.Ascending ? cmp : -cmp;
                });

                // Update FirstRowIndex after reorder
                if (group.RowIndices.Count > 0)
                    group.FirstRowIndex = group.RowIndices.Min();
            }

            // Re-sort groups by FirstRowIndex so headers paint in correct order
            _groups.Sort((a, b) =>
            {
                int cmp = a.FirstRowIndex.CompareTo(b.FirstRowIndex);
                return cmp != 0 ? cmp : a.Level.CompareTo(b.Level);
            });

            SyncRowVisibility();
        }

        /// <summary>
        /// Sorts the top-level groups themselves by the group key of the first descriptor.
        /// Call this when a group-column sort is triggered from the sort pipeline.
        /// </summary>
        public void SortGroupsByColumn(string columnName, GroupSortDirection direction)
        {
            var descriptorIndex = _descriptors.FindIndex(d => d.ColumnName == columnName);
            if (descriptorIndex < 0) return;

            var descriptor = _descriptors[descriptorIndex];
            descriptor.SortDirection = direction;

            // Recompute so groups reflect the new sort direction
            ApplyGrouping();
        }

        /// <summary>
        /// Applies any active column sorts (from BeepColumnConfig.SortDirection) that are
        /// NOT group descriptors as intra-group row sorts.
        /// </summary>
        private void ApplyActiveColumnSortsWithinGroups()
        {
            var groupColumns = new HashSet<string>(_descriptors.Select(d => d.ColumnName), StringComparer.OrdinalIgnoreCase);

            foreach (var column in _grid.Data.Columns)
            {
                if (groupColumns.Contains(column.ColumnName)) continue;
                if (column.SortDirection == SortDirection.Ascending || column.SortDirection == SortDirection.Descending)
                {
                    SortWithinGroups(column.ColumnName, column.SortDirection);
                }
            }
        }

        private static int CompareValues(object? a, object? b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            if (a is IComparable ca && b is IComparable cb && a.GetType() == b.GetType())
                return ca.CompareTo(cb);

            return string.Compare(a.ToString(), b.ToString(), StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Height of a single group header band.
        /// </summary>
        public int GetHeaderHeight()
        {
            return Math.Max(22, _grid.RowHeight);
        }

        /// <summary>
        /// Total number of group headers that should be drawn before the given row index.
        /// </summary>
        public int GetHeaderCountBeforeRow(int rowIndex)
        {
            if (!IsGrouped || rowIndex <= 0) return 0;
            int count = 0;
            foreach (var group in _groups)
            {
                if (group.FirstRowIndex >= 0 && group.FirstRowIndex < rowIndex)
                    count++;
                else if (group.FirstRowIndex >= rowIndex)
                    break; // groups are sorted by FirstRowIndex
            }
            return count;
        }

        /// <summary>
        /// Total content height including all visible rows, group headers, and summary rows.
        /// </summary>
        public int GetTotalContentHeight()
        {
            int rowHeightSum = 0;
            foreach (var row in _grid.Data.Rows)
            {
                if (row.IsVisible)
                    rowHeightSum += row.Height > 0 ? row.Height : _grid.RowHeight;
            }
            int headerHeightSum = IsGrouped ? _groups.Count * GetHeaderHeight() : 0;
            int summaryHeightSum = IsGrouped
                ? _groups.Where(g => !g.IsCollapsed && g.SummaryRow != null).Sum(g => g.SummaryRow!.Height)
                : 0;
            return rowHeightSum + headerHeightSum + summaryHeightSum;
        }

        /// <summary>
        /// Total pixel height of all visible rows, group headers, and summary rows before the given row index.
        /// </summary>
        public int GetTotalHeightBeforeRow(int rowIndex)
        {
            if (rowIndex <= 0) return 0;
            int height = 0;
            for (int i = 0; i < rowIndex && i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                if (row.IsVisible)
                    height += row.Height > 0 ? row.Height : _grid.RowHeight;
            }
            height += GetHeaderCountBeforeRow(rowIndex) * GetHeaderHeight();
            height += GetSummaryRowHeightBeforeRow(rowIndex);
            return height;
        }

        /// <summary>
        /// Total height of all summary rows that appear before the given row index.
        /// A summary row appears after the last row of its group, so it counts if the
        /// group's last row index is strictly less than <paramref name="rowIndex"/>.
        /// </summary>
        public int GetSummaryRowHeightBeforeRow(int rowIndex)
        {
            if (!IsGrouped || rowIndex <= 0) return 0;
            int height = 0;
            foreach (var group in _groups)
            {
                if (group.IsCollapsed || group.SummaryRow == null) continue;
                if (group.RowIndices.Count == 0) continue;
                int lastRow = group.RowIndices.Max();
                if (lastRow < rowIndex)
                    height += group.SummaryRow.Height;
                else if (group.FirstRowIndex >= rowIndex)
                    break; // groups sorted by FirstRowIndex
            }
            return height;
        }
    }
}
