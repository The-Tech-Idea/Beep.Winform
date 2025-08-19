using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridLayoutHelper
    {
        private readonly BeepGridPro _grid;
        public int RowHeight { get; set; } = 25;
        public int ColumnHeaderHeight { get; set; } = 28;
        public bool ShowColumnHeaders { get; set; } = true;
        public Rectangle HeaderRect { get; private set; }
        public Rectangle RowsRect { get; private set; }

        public GridLayoutHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        public void EnsureCalculated()
        {
            if (HeaderRect == Rectangle.Empty && RowsRect == Rectangle.Empty)
                Recalculate();
        }

        public void Recalculate()
        {
            var r = _grid.DrawingRect;
            int top = r.Top;
            if (ShowColumnHeaders)
            {
                HeaderRect = new Rectangle(r.Left, top, r.Width, ColumnHeaderHeight);
                top += ColumnHeaderHeight;
            }
            else
            {
                HeaderRect = Rectangle.Empty;
            }
            RowsRect = new Rectangle(r.Left, top, r.Width, r.Height - (top - r.Top));
            LayoutCells();
        }

        private void LayoutCells()
        {
            if (_grid.Data.Rows.Count == 0 || _grid.Data.Columns.Count == 0)
                return;

            int[] xmap = new int[_grid.Data.Columns.Count];
            int cx = RowsRect.Left;
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                var col = _grid.Data.Columns[i];
                xmap[i] = cx;
                cx += Math.Max(20, col.Width);
            }

            int y = RowsRect.Top;
            for (int r = 0; r < _grid.Data.Rows.Count; r++)
            {
                var row = _grid.Data.Rows[r];
                int h = RowHeight;
                for (int c = 0; c < row.Cells.Count; c++)
                {
                    var cell = row.Cells[c];
                    cell.Rect = new Rectangle(xmap[c], y, Math.Max(20, cell.Width), h);
                }
                y += h;
            }
        }
    }
}
