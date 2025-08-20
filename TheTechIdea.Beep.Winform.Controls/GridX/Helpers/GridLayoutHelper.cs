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
        public Rectangle[] HeaderCellRects { get; private set; } = System.Array.Empty<Rectangle>();
        public int NavigatorHeight { get; set; } = 36;
        public Rectangle NavigatorRect { get; private set; } = Rectangle.Empty;
        public int CheckBoxColumnWidth { get; set; } = 28;
        public Rectangle SelectAllCheckRect { get; private set; } = Rectangle.Empty;
        public bool IsCalculating { get; private set; }

        public GridLayoutHelper(BeepGridPro grid) { _grid = grid; }

        public void EnsureCalculated()
        {
            if (HeaderRect == Rectangle.Empty && RowsRect == Rectangle.Empty)
                Recalculate();
        }

        public void Recalculate()
        {
            if (IsCalculating) return;
            IsCalculating = true;
            try
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

                int bottomReserve = 0;
                if (_grid.ShowNavigator && r.Height > NavigatorHeight)
                {
                    bottomReserve = NavigatorHeight;
                    NavigatorRect = new Rectangle(r.Left, r.Bottom - NavigatorHeight, r.Width, NavigatorHeight);
                }
                else
                {
                    NavigatorRect = Rectangle.Empty;
                }

                RowsRect = new Rectangle(r.Left, top, r.Width, System.Math.Max(0, r.Height - (top - r.Top) - bottomReserve));
                LayoutCells();
            }
            finally
            {
                IsCalculating = false;
            }
        }

        private void LayoutCells()
        {
            if (_grid.Data.Columns.Count == 0)
            {
                HeaderCellRects = System.Array.Empty<Rectangle>();
                return;
            }

            int extraLeft = _grid.ShowCheckBox ? CheckBoxColumnWidth : 0;
            int pinnedWidth = _grid.Data.Columns.Where(c => c.Visible && c.Sticked).Sum(c => System.Math.Max(20, c.Width));
            int unpinnedStartX = RowsRect.Left + extraLeft + pinnedWidth - _grid.Scroll.HorizontalOffset;

            HeaderCellRects = new Rectangle[_grid.Data.Columns.Count];
            int px = RowsRect.Left + extraLeft;
            int ux = unpinnedStartX;
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                var col = _grid.Data.Columns[i];
                int w = System.Math.Max(20, col.Width);
                int x = col.Sticked ? px : ux;
                HeaderCellRects[i] = new Rectangle(x, HeaderRect.Top, w, HeaderRect.Height);
                if (col.Sticked) px += w; else ux += w;
            }

            if (_grid.ShowCheckBox && ShowColumnHeaders)
            {
                int size = CheckBoxColumnWidth - 8;
                SelectAllCheckRect = new Rectangle(RowsRect.Left + 4, HeaderRect.Top + (HeaderRect.Height - size) / 2, size, size);
            }
            else
            {
                SelectAllCheckRect = Rectangle.Empty;
            }

            if (_grid.Data.Rows.Count == 0)
                return;

            int visibleRows = System.Math.Max(1, RowsRect.Height / RowHeight);
            int startRow = System.Math.Max(0, _grid.Scroll.FirstVisibleRowIndex);
            int endRow = System.Math.Min(_grid.Data.Rows.Count - 1, startRow + visibleRows - 1);

            int[] xmap = new int[_grid.Data.Columns.Count];
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
                xmap[i] = HeaderCellRects[i].X;

            int y = RowsRect.Top;
            for (int r = startRow; r <= endRow; r++)
            {
                var row = _grid.Data.Rows[r];
                int h = RowHeight;
                int cbSize = System.Math.Min(h - 6, CheckBoxColumnWidth - 6);
                row.RowCheckRect = _grid.ShowCheckBox
                    ? new Rectangle(RowsRect.Left + 4, y + (h - cbSize) / 2, cbSize, cbSize)
                    : Rectangle.Empty;

                for (int c = 0; c < row.Cells.Count; c++)
                {
                    var cell = row.Cells[c];
                    int w = System.Math.Max(20, cell.Width);
                    cell.Rect = new Rectangle(xmap[c], y, w, h);
                }
                y += h;
            }
        }
    }
}
