using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridSelectionHelper
    {
        private readonly BeepGridPro _grid;
        public int RowIndex { get; private set; } = -1;
        public int ColumnIndex { get; private set; } = -1;
        public bool HasSelection => RowIndex >= 0 && ColumnIndex >= 0;
        public Rectangle SelectedCellRect
        {
            get
            {
                if (!HasSelection) return Rectangle.Empty;
                return _grid.Data.Rows[RowIndex].Cells[ColumnIndex].Rect;
            }
        }

        public GridSelectionHelper(BeepGridPro grid) { _grid = grid; }

        public void SelectCell(int row, int col)
        {
            if (row < 0 || row >= _grid.Data.Rows.Count) { Clear(); return; }
            if (col < 0 || col >= _grid.Data.Columns.Count) { Clear(); return; }
            
            int oldRow = RowIndex;
            int oldCol = ColumnIndex;
            if (oldRow == row && oldCol == col)
            {
                EnsureVisible();
                return;
            }
            
            RowIndex = row; 
            ColumnIndex = col;

            if (IsValidCell(oldRow, oldCol))
            {
                _grid.Data.Rows[oldRow].Cells[oldCol].IsSelected = false;
            }
            if (IsValidCell(row, col))
            {
                _grid.Data.Rows[row].Cells[col].IsSelected = true;
            }

            bool rowChanged = oldRow != row;
            bool repainted = false;

            if (rowChanged)
            {
                repainted |= TryInvalidateRow(oldRow);
                repainted |= TryInvalidateRow(row);
            }
            else
            {
                repainted |= TryInvalidateCell(oldRow, oldCol);
                repainted |= TryInvalidateCell(row, col);
            }

            // Only do full invalidate if regional invalidations completely failed
            // This prevents flickering of controls like combobox search textboxes
            if (!repainted && _grid.Layout?.RowsRect.IsEmpty != false)
            {
                _grid.SafeInvalidate();
            }
            
            // Ensure the selected row is visible
            EnsureVisible();
            
            // Do not raise RowSelectionChanged on highlight-only changes
        }

        public void Clear()
        {
            RowIndex = ColumnIndex = -1;
        }

        public void EnsureVisible()
        {
            if (!HasSelection) return;
            
            // Calculate the pixel offset for the selected row
            int targetY = 0;
            for (int i = 0; i < RowIndex && i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                targetY += row.Height > 0 ? row.Height : _grid.RowHeight;
            }
            
            // Get current viewport
            var rowsRect = _grid.Layout.RowsRect;
            int viewportTop = _grid.Scroll.VerticalOffset;
            int viewportBottom = viewportTop + rowsRect.Height;
            int selectedRowHeight = _grid.Data.Rows[RowIndex].Height > 0 ? _grid.Data.Rows[RowIndex].Height : _grid.RowHeight;
            int targetBottom = targetY + selectedRowHeight;
            
            // Check if row is above viewport - scroll up
            if (targetY < viewportTop)
            {
                _grid.Scroll.SetVerticalOffset(targetY);
                _grid.Scroll.SetVerticalIndex(RowIndex);
            }
            // Check if row is below viewport - scroll down
            else if (targetBottom > viewportBottom)
            {
                int newOffset = targetBottom - rowsRect.Height;
                _grid.Scroll.SetVerticalOffset(newOffset);
                _grid.Scroll.SetVerticalIndex(RowIndex);
            }
            // Row is already visible - no scroll needed
        }

        private bool IsValidCell(int row, int col)
        {
            if (row < 0 || row >= _grid.Data.Rows.Count) return false;
            if (col < 0 || col >= _grid.Data.Columns.Count) return false;
            return col < _grid.Data.Rows[row].Cells.Count;
        }

        private bool TryInvalidateCell(int row, int col)
        {
            if (!IsValidCell(row, col)) return false;
            var rect = _grid.Data.Rows[row].Cells[col].Rect;
            if (rect.IsEmpty) return false;
            _grid.SafeInvalidate(rect);
            return true;
        }

        private bool TryInvalidateRow(int row)
        {
            if (row < 0 || row >= _grid.Data.Rows.Count) return false;
            var rowCells = _grid.Data.Rows[row].Cells;
            if (rowCells.Count == 0) return false;
            var firstRect = rowCells[0].Rect;
            if (firstRect.IsEmpty) return false;

            var rowRect = new Rectangle(_grid.Layout.RowsRect.Left, firstRect.Top, _grid.Layout.RowsRect.Width, firstRect.Height);
            _grid.SafeInvalidate(rowRect);
            return true;
        }
    }
}
