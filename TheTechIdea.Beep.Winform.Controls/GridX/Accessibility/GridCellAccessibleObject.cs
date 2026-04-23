using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Accessibility
{
    /// <summary>
    /// Accessible object representing a single cell in <see cref="BeepGridPro"/>.
    /// </summary>
    internal sealed class GridCellAccessibleObject : AccessibleObject
    {
        private readonly BeepGridPro _grid;
        private readonly int _rowIndex;
        private readonly int _colIndex;
        private readonly AccessibleObject _parent;

        public GridCellAccessibleObject(BeepGridPro grid, int rowIndex, int colIndex, AccessibleObject parent)
        {
            _grid = grid;
            _rowIndex = rowIndex;
            _colIndex = colIndex;
            _parent = parent;
        }

        public override AccessibleRole Role => AccessibleRole.Cell;

        public override string? Name
        {
            get
            {
                if (_colIndex < 0 || _colIndex >= _grid.Data.Columns.Count) return null;
                return _grid.Data.Columns[_colIndex].ColumnCaption;
            }
        }

        public override string? Value
        {
            get
            {
                if (_rowIndex < 0 || _rowIndex >= _grid.Data.Rows.Count) return null;
                var row = _grid.Data.Rows[_rowIndex];
                if (_colIndex < 0 || _colIndex >= row.Cells.Count) return null;
                return row.Cells[_colIndex].CellValue?.ToString() ?? string.Empty;
            }
        }

        public override AccessibleObject? Parent => _parent;

        public override Rectangle Bounds
        {
            get
            {
                if (_rowIndex < 0 || _rowIndex >= _grid.Data.Rows.Count) return Rectangle.Empty;
                var row = _grid.Data.Rows[_rowIndex];
                if (_colIndex < 0 || _colIndex >= row.Cells.Count) return Rectangle.Empty;
                var cellRect = row.Cells[_colIndex].Rect;
                if (cellRect.IsEmpty) return Rectangle.Empty;
                return _grid.RectangleToScreen(cellRect);
            }
        }

        public override AccessibleStates State
        {
            get
            {
                var state = base.State;
                if (_grid.Selection?.RowIndex == _rowIndex && _grid.Selection?.ColumnIndex == _colIndex)
                    state |= AccessibleStates.Selected | AccessibleStates.Focused;
                if (_rowIndex >= 0 && _rowIndex < _grid.Data.Rows.Count && !_grid.Data.Rows[_rowIndex].IsVisible)
                    state |= AccessibleStates.Invisible;
                return state;
            }
        }

        public override AccessibleObject? Navigate(AccessibleNavigation navDirection)
        {
            var visibleCols = _grid.Data.Columns
                .Select((c, i) => new { c, i })
                .Where(x => x.c.Visible)
                .OrderBy(x => x.c.DisplayOrder)
                .ToList();

            var current = visibleCols.FirstOrDefault(v => v.i == _colIndex);
            if (current == null) return base.Navigate(navDirection);

            int currentIdx = visibleCols.IndexOf(current);

            switch (navDirection)
            {
                case AccessibleNavigation.Next:
                    if (currentIdx + 1 < visibleCols.Count)
                        return _parent.GetChild(currentIdx + 1);
                    // Move to first cell of next row
                    var nextRow = _parent.Parent?.GetChild(_rowIndex + 1) as GridRowAccessibleObject;
                    return nextRow?.GetChild(0);

                case AccessibleNavigation.Previous:
                    if (currentIdx > 0)
                        return _parent.GetChild(currentIdx - 1);
                    // Move to last cell of previous row
                    var prevRow = _parent.Parent?.GetChild(_rowIndex - 1) as GridRowAccessibleObject;
                    int prevCount = prevRow?.GetChildCount() ?? 0;
                    return prevRow?.GetChild(prevCount - 1);

                case AccessibleNavigation.Left:
                    if (currentIdx > 0)
                        return _parent.GetChild(currentIdx - 1);
                    return null;

                case AccessibleNavigation.Right:
                    if (currentIdx + 1 < visibleCols.Count)
                        return _parent.GetChild(currentIdx + 1);
                    return null;

                case AccessibleNavigation.Up:
                    {
                        var upRow = _parent.Parent?.GetChild(_rowIndex - 1) as GridRowAccessibleObject;
                        return upRow?.GetChild(currentIdx);
                    }

                case AccessibleNavigation.Down:
                    {
                        var downRow = _parent.Parent?.GetChild(_rowIndex + 1) as GridRowAccessibleObject;
                        return downRow?.GetChild(currentIdx);
                    }

                default:
                    return base.Navigate(navDirection);
            }
        }

        public override void Select(AccessibleSelection flags)
        {
            _grid.SelectCell(_rowIndex, _colIndex);
            _grid.Focus();
        }

        public override void DoDefaultAction()
        {
            _grid.SelectCell(_rowIndex, _colIndex);
            if (_grid.Edit != null && !_grid.ReadOnly)
                _grid.Edit.BeginEdit();
        }
    }
}
