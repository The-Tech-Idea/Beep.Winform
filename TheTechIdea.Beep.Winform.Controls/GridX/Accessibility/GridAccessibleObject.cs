using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Accessibility
{
    /// <summary>
    /// Root <see cref="AccessibleObject"/> for <see cref="BeepGridPro"/>.
    /// Exposes the grid as an accessible table with rows and cells.
    /// </summary>
    internal sealed class GridAccessibleObject : System.Windows.Forms.Control.ControlAccessibleObject
    {
        private readonly BeepGridPro _grid;
        private readonly Dictionary<int, GridRowAccessibleObject> _rowCache = new();

        public GridAccessibleObject(BeepGridPro owner) : base(owner)
        {
            _grid = owner;
        }

        public override AccessibleRole Role => AccessibleRole.Table;

        public override string? Name => !string.IsNullOrWhiteSpace(_grid.GridTitle) ? _grid.GridTitle : base.Name;

        public override string? Description => string.IsNullOrWhiteSpace(_grid.AccessibleDescription)
            ? $"{_grid.Data.Rows.Count} rows, {_grid.Data.Columns.Count} columns"
            : _grid.AccessibleDescription;

        public override int GetChildCount()
            => _grid.Data.Rows.Count(row => row.IsVisible);

        public override AccessibleObject? GetChild(int index)
        {
            if (index < 0) return null;

            int visibleIndex = 0;
            for (int rowIndex = 0; rowIndex < _grid.Data.Rows.Count; rowIndex++)
            {
                if (!_grid.Data.Rows[rowIndex].IsVisible) continue;
                if (visibleIndex == index)
                {
                    if (!_rowCache.TryGetValue(rowIndex, out var rowObj))
                    {
                        rowObj = new GridRowAccessibleObject(_grid, rowIndex, this);
                        _rowCache[rowIndex] = rowObj;
                    }
                    return rowObj;
                }
                visibleIndex++;
            }

            return null;
        }

        public override AccessibleObject? GetFocused() => GetSelected();

        public override AccessibleObject? GetSelected()
        {
            int rowIndex = _grid.Selection?.RowIndex ?? -1;
            int columnIndex = _grid.Selection?.ColumnIndex ?? -1;
            if (rowIndex < 0 || columnIndex < 0) return null;
            if (rowIndex >= _grid.Data.Rows.Count || columnIndex >= _grid.Data.Columns.Count) return null;
            if (!_grid.Data.Rows[rowIndex].IsVisible) return null;

            var rowObject = GetRowObject(rowIndex) as GridRowAccessibleObject;
            if (rowObject == null) return null;

            int visibleColumnIndex = 0;
            foreach (var column in _grid.Data.Columns
                         .Select((column, index) => new { column, index })
                         .Where(item => item.column.Visible)
                         .OrderBy(item => item.column.DisplayOrder))
            {
                if (column.index == columnIndex)
                    return rowObject.GetChild(visibleColumnIndex);
                visibleColumnIndex++;
            }

            return null;
        }

        internal AccessibleObject? GetRowObject(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _grid.Data.Rows.Count) return null;
            if (!_rowCache.TryGetValue(rowIndex, out var rowObj))
            {
                rowObj = new GridRowAccessibleObject(_grid, rowIndex, this);
                _rowCache[rowIndex] = rowObj;
            }
            return rowObj;
        }

        internal AccessibleObject? GetAdjacentVisibleRow(int rowIndex, int direction)
        {
            for (int i = rowIndex + direction; i >= 0 && i < _grid.Data.Rows.Count; i += direction)
            {
                if (_grid.Data.Rows[i].IsVisible) return GetRowObject(i);
            }
            return null;
        }

        public override AccessibleObject? Navigate(AccessibleNavigation navDirection)
        {
            switch (navDirection)
            {
                case AccessibleNavigation.FirstChild:
                    return GetChild(0);
                case AccessibleNavigation.LastChild:
                    return GetChild(GetChildCount() - 1);
                default:
                    return base.Navigate(navDirection);
            }
        }

        public override AccessibleObject? HitTest(int x, int y)
        {
            var pt = _grid.PointToClient(new System.Drawing.Point(x, y));
            var rowsRect = _grid.Layout.RowsRect;
            if (!rowsRect.Contains(pt)) return this;

            // Find row at point
            int rowIndex = FindRowAtPoint(pt.Y, rowsRect);
            if (rowIndex < 0 || rowIndex >= _grid.Data.Rows.Count) return this;

            var rowObj = GetChild(rowIndex) as GridRowAccessibleObject;
            return rowObj?.HitTest(x, y) ?? this;
        }

        private int FindRowAtPoint(int clientY, Rectangle rowsRect)
        {
            int y = rowsRect.Top;
            for (int i = 0; i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                if (!row.IsVisible) continue;
                int h = row.Height > 0 ? row.Height : _grid.RowHeight;
                if (clientY >= y && clientY < y + h) return i;
                y += h;
                // Account for group headers and summary rows
                y += (_grid.GroupEngine?.GetHeaderCountBeforeRow(i + 1) ?? 0) * (_grid.GroupEngine?.GetHeaderHeight() ?? 0);
                y += _grid.GroupEngine?.GetSummaryRowHeightBeforeRow(i + 1) ?? 0;
            }
            return -1;
        }
    }
}
