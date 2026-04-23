using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Accessibility
{
    /// <summary>
    /// Accessible object representing a single row in <see cref="BeepGridPro"/>.
    /// Children are the cells in this row.
    /// </summary>
    internal sealed class GridRowAccessibleObject : AccessibleObject
    {
        private readonly BeepGridPro _grid;
        private readonly int _rowIndex;
        private readonly AccessibleObject _parent;
        private readonly Dictionary<int, GridCellAccessibleObject> _cellCache = new();

        public GridRowAccessibleObject(BeepGridPro grid, int rowIndex, AccessibleObject parent)
        {
            _grid = grid;
            _rowIndex = rowIndex;
            _parent = parent;
        }

        public override AccessibleRole Role => AccessibleRole.Row;

        public override string? Name => $"Row {_rowIndex + 1}";

        public override AccessibleObject? Parent => _parent;

        public override Rectangle Bounds
        {
            get
            {
                if (_rowIndex < 0 || _rowIndex >= _grid.Data.Rows.Count) return Rectangle.Empty;
                var row = _grid.Data.Rows[_rowIndex];
                var cellRect = row.Cells.FirstOrDefault()?.Rect ?? Rectangle.Empty;
                if (cellRect.IsEmpty)
                {
                    // Fallback: compute from layout
                    var rowsRect = _grid.Layout.RowsRect;
                    int y = _grid.Render.CalculateRowY(_rowIndex, rowsRect.Top);
                    return _grid.RectangleToScreen(new Rectangle(rowsRect.Left, y, rowsRect.Width, row.Height > 0 ? row.Height : _grid.RowHeight));
                }
                return _grid.RectangleToScreen(new Rectangle(
                    _grid.Layout.RowsRect.Left, cellRect.Y,
                    _grid.Layout.RowsRect.Width, cellRect.Height));
            }
        }

        public override int GetChildCount()
        {
            return _grid.Data.Columns.Count(c => c.Visible);
        }

        public override AccessibleObject? GetChild(int index)
        {
            var visibleCols = _grid.Data.Columns
                .Select((c, i) => new { c, i })
                .Where(x => x.c.Visible)
                .OrderBy(x => x.c.DisplayOrder)
                .ToList();

            if (index < 0 || index >= visibleCols.Count) return null;

            int colIndex = visibleCols[index].i;
            if (!_cellCache.TryGetValue(colIndex, out var cellObj))
            {
                cellObj = new GridCellAccessibleObject(_grid, _rowIndex, colIndex, this);
                _cellCache[colIndex] = cellObj;
            }
            return cellObj;
        }

        public override AccessibleObject? Navigate(AccessibleNavigation navDirection)
        {
            switch (navDirection)
            {
                case AccessibleNavigation.FirstChild:
                    return GetChild(0);
                case AccessibleNavigation.LastChild:
                    return GetChild(GetChildCount() - 1);
                case AccessibleNavigation.Next:
                    return _parent.GetChild(_rowIndex + 1);
                case AccessibleNavigation.Previous:
                    return _parent.GetChild(_rowIndex - 1);
                case AccessibleNavigation.Up:
                    return _parent.GetChild(_rowIndex - 1);
                case AccessibleNavigation.Down:
                    return _parent.GetChild(_rowIndex + 1);
                default:
                    return base.Navigate(navDirection);
            }
        }

        public override AccessibleObject? HitTest(int x, int y)
        {
            var pt = _grid.PointToClient(new System.Drawing.Point(x, y));
            var rowsRect = _grid.Layout.RowsRect;

            // Find column at point
            int colIndex = FindColumnAtPoint(pt.X, rowsRect);
            if (colIndex < 0 || colIndex >= _grid.Data.Columns.Count) return this;

            var visibleCols = _grid.Data.Columns
                .Select((c, i) => new { c, i })
                .Where(x => x.c.Visible)
                .OrderBy(x => x.c.DisplayOrder)
                .ToList();

            var match = visibleCols.FirstOrDefault(v => v.i == colIndex);
            if (match == null) return this;

            int childIndex = visibleCols.IndexOf(match);
            return GetChild(childIndex) ?? this;
        }

        private int FindColumnAtPoint(int clientX, Rectangle rowsRect)
        {
            int x = rowsRect.Left;
            foreach (var col in _grid.Data.Columns.Where(c => c.Visible).OrderBy(c => c.DisplayOrder))
            {
                int w = Math.Max(20, col.Width);
                if (clientX >= x && clientX < x + w) return _grid.Data.Columns.IndexOf(col);
                x += w;
            }
            return -1;
        }
    }
}
