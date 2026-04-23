using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.GridX.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Selection
{
    /// <summary>
    /// Strategy interface for handling selection behavior in BeepGridPro.
    /// Implementations determine what gets selected when the user interacts with cells/rows.
    /// </summary>
    internal interface ISelectionStrategy
    {
        /// <summary>
        /// Called when the user clicks a cell. The strategy decides what to select.
        /// </summary>
        /// <param name="context">Selection context providing access to grid state.</param>
        /// <param name="row">Clicked row index.</param>
        /// <param name="col">Clicked column index.</param>
        /// <param name="ctrlPressed">Whether Ctrl is held (multi-select toggle).</param>
        /// <param name="shiftPressed">Whether Shift is held (range select).</param>
        void HandleCellClick(SelectionContext context, int row, int col, bool ctrlPressed, bool shiftPressed);

        /// <summary>
        /// Called when the user clicks a column header.
        /// </summary>
        void HandleColumnHeaderClick(SelectionContext context, int col, bool ctrlPressed, bool shiftPressed);

        /// <summary>
        /// Called when the user presses an arrow key for keyboard navigation.
        /// </summary>
        void HandleKeyboardNavigation(SelectionContext context, int deltaRow, int deltaCol);

        /// <summary>
        /// Clears all selection state managed by this strategy.
        /// </summary>
        void ClearSelection(SelectionContext context);
    }

    /// <summary>
    /// Context passed to selection strategies. Provides access to grid state
    /// and a lightweight state bag for strategy-specific anchors/ranges.
    /// </summary>
    internal sealed class SelectionContext
    {
        private readonly BeepGridPro _grid;
        private readonly GridSelectionHelper _selection;

        public SelectionContext(BeepGridPro grid, GridSelectionHelper selection)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _selection = selection ?? throw new ArgumentNullException(nameof(selection));
        }

        public BeepGridPro Grid => _grid;
        public GridSelectionHelper Selection => _selection;

        /// <summary>
        /// Lightweight state bag strategies can use to persist anchor/range state.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> StrategyState { get; } = new();

        public int RowCount => _grid.Data?.Rows?.Count ?? 0;
        public int ColumnCount => _grid.Data?.Columns?.Count ?? 0;

        public bool IsValidCell(int row, int col)
        {
            if (row < 0 || row >= RowCount) return false;
            if (col < 0 || col >= ColumnCount) return false;
            return col < _grid.Data.Rows[row].Cells.Count;
        }

        public void SetActiveCell(int row, int col)
        {
            _selection.SelectCell(row, col);
        }

        public void SetRowSelected(int row, bool selected)
        {
            if (row < 0 || row >= RowCount) return;
            var r = _grid.Data.Rows[row];
            if (r.IsSelected != selected)
            {
                r.IsSelected = selected;
                _grid.InvalidateRow(row);
            }
        }

        public void SetCellSelected(int row, int col, bool selected)
        {
            if (!IsValidCell(row, col)) return;
            var cell = _grid.Data.Rows[row].Cells[col];
            if (cell.IsSelected != selected)
            {
                cell.IsSelected = selected;
                if (!cell.Rect.IsEmpty)
                    _grid.SafeInvalidate(cell.Rect);
            }
        }

        public void ClearAllRowSelection()
        {
            for (int i = 0; i < RowCount; i++)
            {
                if (_grid.Data.Rows[i].IsSelected)
                {
                    _grid.Data.Rows[i].IsSelected = false;
                    _grid.InvalidateRow(i);
                }
            }
        }

        public void ClearAllCellSelection()
        {
            for (int r = 0; r < RowCount; r++)
            {
                var row = _grid.Data.Rows[r];
                for (int c = 0; c < row.Cells.Count; c++)
                {
                    if (row.Cells[c].IsSelected)
                    {
                        row.Cells[c].IsSelected = false;
                    }
                }
                _grid.InvalidateRow(r);
            }
        }

        public void RaiseRowSelectionChanged(int row)
        {
            _grid.OnRowSelectionChanged(row);
        }

        public void EnsureVisible()
        {
            _selection.EnsureVisible();
        }
    }
}
