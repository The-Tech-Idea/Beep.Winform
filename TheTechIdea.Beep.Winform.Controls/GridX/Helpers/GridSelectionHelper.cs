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
            RowIndex = row; ColumnIndex = col;

            // Only update active cell flags for highlighting; do NOT change row.IsSelected here.
            for (int r = 0; r < _grid.Data.Rows.Count; r++)
            {
                var rr = _grid.Data.Rows[r];
                for (int c = 0; c < rr.Cells.Count; c++)
                {
                    rr.Cells[c].IsSelected = (r == row && c == col);
                }
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
    }
}
