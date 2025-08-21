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
            // Do not raise RowSelectionChanged on highlight-only changes
        }

        public void Clear()
        {
            RowIndex = ColumnIndex = -1;
        }

        public void EnsureVisible()
        {
            if (!HasSelection) return;
        }
    }
}
