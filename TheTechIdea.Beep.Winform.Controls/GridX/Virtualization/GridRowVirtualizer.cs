using System;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Virtualization
{
    /// <summary>
    /// Manages on-demand row materialization for large datasets in <see cref="BeepGridPro"/>.
    /// Maintains a sliding window of materialized rows synchronized with the current scroll position.
    /// When active, <see cref="BeepGridPro.Data.Rows"/> contains only the visible window.
    /// </summary>
    public sealed class GridRowVirtualizer
    {
        private readonly BeepGridPro _grid;
        private IVirtualDataSource? _dataSource;
        private readonly Dictionary<long, Models.BeepRowConfig> _materializedRows = new();
        private long _windowStart = 0;
        private int _windowSize = 0;
        private int _overscan = 5;

        public GridRowVirtualizer(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        /// <summary>
        /// The virtual data source currently attached.
        /// </summary>
        public IVirtualDataSource? DataSource
        {
            get => _dataSource;
            set
            {
                if (_dataSource != null)
                    _dataSource.TotalRowCountChanged -= OnTotalRowCountChanged;

                _dataSource = value;
                _materializedRows.Clear();
                _windowStart = 0;
                _windowSize = 0;

                if (_dataSource != null)
                {
                    _dataSource.TotalRowCountChanged += OnTotalRowCountChanged;
                    SyncGridRowCount();
                }
            }
        }

        /// <summary>
        /// Absolute index of the first row in the current visible window.
        /// </summary>
        public long WindowStart => _windowStart;

        /// <summary>
        /// Number of extra rows to materialize above and below the visible viewport.
        /// Higher values reduce flicker during fast scrolling at the cost of memory.
        /// Default: 5.
        /// </summary>
        public int Overscan
        {
            get => _overscan;
            set => _overscan = Math.Max(0, value);
        }

        /// <summary>
        /// Whether virtualization is active (has a data source attached).
        /// </summary>
        public bool IsActive => _dataSource != null;

        /// <summary>
        /// Total row count from the virtual data source, or -1 if not attached.
        /// </summary>
        public long TotalRowCount => _dataSource?.TotalRowCount ?? -1;

        /// <summary>
        /// Update the materialized window based on the current scroll offset and viewport height.
        /// Call this from the grid's scroll handler.
        /// </summary>
        public void UpdateWindow(int scrollOffset, int viewportHeight, int rowHeight)
        {
            if (_dataSource == null || viewportHeight <= 0 || rowHeight <= 0) return;

            int visibleCount = (int)Math.Ceiling(viewportHeight / (double)rowHeight) + 1;
            long newStart = Math.Max(0, scrollOffset / rowHeight - _overscan);
            int newSize = visibleCount + _overscan * 2;

            if (newStart == _windowStart && newSize == _windowSize)
                return; // No change

            _windowStart = newStart;
            _windowSize = newSize;

            // Materialize the new window
            MaterializeWindow(newStart, newSize);

            // Preload adjacent window for smooth scrolling
            _dataSource.PreloadWindow(newStart, visibleCount, _overscan);
        }

        /// <summary>
        /// Get a materialized row by its absolute index.
        /// Returns null if the row is outside the current window.
        /// </summary>
        public Models.BeepRowConfig? GetRow(long index)
        {
            _materializedRows.TryGetValue(index, out var row);
            return row;
        }

        /// <summary>
        /// Force rematerialization of the current window.
        /// Call after filter/sort changes that invalidate existing rows.
        /// </summary>
        public void Refresh()
        {
            _materializedRows.Clear();
            if (_dataSource != null)
            {
                SyncGridRowCount();
                MaterializeWindow(_windowStart, _windowSize);
            }
        }

        /// <summary>
        /// Convert virtual row data into BeepRowConfig objects and populate the grid.
        /// </summary>
        private void MaterializeWindow(long start, int count)
        {
            if (_dataSource == null) return;

            var virtualRows = _dataSource.GetRows(start, count);
            var columns = _grid.Data.Columns.ToList();

            foreach (var vr in virtualRows)
            {
                if (_materializedRows.ContainsKey(vr.Index))
                    continue;

                var row = new Models.BeepRowConfig
                {
                    RowData = vr.OriginalItem
                };

                for (int ci = 0; ci < columns.Count; ci++)
                {
                    var col = columns[ci];
                    object? val = null;

                    if (col.IsSelectionCheckBox)
                        val = false;
                    else if (col.IsRowNumColumn)
                        val = (int)vr.Index + 1;
                    else if (col.IsRowID)
                        val = (int)vr.Index;
                    else if (vr.Values.TryGetValue(col.ColumnName, out var v))
                        val = v;

                    row.Cells.Add(new Models.BeepCellConfig
                    {
                        RowIndex = (int)(vr.Index - _windowStart),
                        ColumnIndex = ci,
                        DisplayIndex = ci,
                        ColumnName = col.ColumnName,
                        CellValue = val!,
                        Width = col.Width,
                        Height = _grid.RowHeight,
                        IsReadOnly = col.ReadOnly,
                        IsEditable = !col.ReadOnly
                    });
                }
                _materializedRows[vr.Index] = row;
            }

            // Publish materialized rows to the grid's data helper
            PublishToGrid();
        }

        private void PublishToGrid()
        {
            _grid.Data.Rows.Clear();
            foreach (var kv in _materializedRows.OrderBy(x => x.Key))
            {
                long absIndex = kv.Key;
                var row = kv.Value;
                int relIndex = (int)(absIndex - _windowStart);
                row.RowIndex = relIndex;
                row.DisplayIndex = relIndex;
                row.Height = _grid.RowHeight;
                row.IsVisible = true;

                _grid.Data.Rows.Add(row);
            }
            _grid.Data.UpdatePageInfo();
        }

        private void SyncGridRowCount()
        {
            if (_dataSource == null) return;
            // Trigger layout recalculation so scrollbars are sized for the total virtual row count
            _grid.Layout?.Recalculate();
            _grid.ScrollBars?.UpdateBars();
        }

        private void OnTotalRowCountChanged(object? sender, EventArgs e)
        {
            SyncGridRowCount();
            _grid.SafeInvalidate();
        }
    }
}
