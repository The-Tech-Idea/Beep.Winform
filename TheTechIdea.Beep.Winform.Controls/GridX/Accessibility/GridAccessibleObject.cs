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

        public override string? Description => $"{_grid.Data.Rows.Count} rows, {_grid.Data.Columns.Count} columns";

        public override int GetChildCount()
        {
            return _grid.Data.Rows.Count;
        }

        public override AccessibleObject? GetChild(int index)
        {
            if (index < 0 || index >= _grid.Data.Rows.Count) return null;
            if (!_rowCache.TryGetValue(index, out var rowObj))
            {
                rowObj = new GridRowAccessibleObject(_grid, index, this);
                _rowCache[index] = rowObj;
            }
            return rowObj;
        }

        public override AccessibleObject? Navigate(AccessibleNavigation navDirection)
        {
            switch (navDirection)
            {
                case AccessibleNavigation.FirstChild:
                    return GetChild(0);
                case AccessibleNavigation.LastChild:
                    return GetChild(_grid.Data.Rows.Count - 1);
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
