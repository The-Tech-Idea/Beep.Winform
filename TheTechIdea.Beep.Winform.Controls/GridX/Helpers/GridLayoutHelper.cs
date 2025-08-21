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
        public int CheckBoxColumnWidth { get; set; } = 30; // Add checkbox column width like BeepSimpleGrid
        public Rectangle SelectAllCheckRect { get; private set; } = Rectangle.Empty;
        public bool IsCalculating { get; private set; }
        public int HoveredHeaderColumnIndex { get; internal set; }

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
                
                // Validate the drawing rectangle
                if (r.Width <= 0 || r.Height <= 0)
                {
                    HeaderRect = Rectangle.Empty;
                    RowsRect = Rectangle.Empty;
                    NavigatorRect = Rectangle.Empty;
                    HeaderCellRects = System.Array.Empty<Rectangle>();
                    return;
                }
                
                int top = r.Top;
                if (ShowColumnHeaders && r.Height > ColumnHeaderHeight)
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

                // Calculate RowsRect accounting for scrollbars exactly like BeepSimpleGrid
                // Initially assume no scrollbars, then adjust if needed
                int rowsHeight = Math.Max(0, r.Height - (top - r.Top) - bottomReserve);
                if (rowsHeight > 0)
                {
                    RowsRect = new Rectangle(r.Left, top, r.Width, rowsHeight);
                }
                else
                {
                    RowsRect = Rectangle.Empty;
                }

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

            // Ensure system columns are present and visible/invisible as needed exactly like BeepSimpleGrid
            var selColumn = _grid.Data.Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_grid.ShowCheckBox && selColumn != null)
            {
                selColumn.Visible = true;
            }
            else if (selColumn != null)
            {
                selColumn.Visible = false;
            }

            // Calculate sticky columns width exactly like BeepSimpleGrid
            var stickyColumns = _grid.Data.Columns.Where(c => c.Visible && c.Sticked).ToList();
            int stickyWidth = stickyColumns.Sum(c => Math.Max(20, c.Width));
            stickyWidth = Math.Min(stickyWidth, RowsRect.Width); // Prevent overflow

            int unpinnedStartX = RowsRect.Left + stickyWidth - _grid.Scroll.HorizontalOffset;

            HeaderCellRects = new Rectangle[_grid.Data.Columns.Count];
            int px = RowsRect.Left;
            int ux = unpinnedStartX;
            
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                var col = _grid.Data.Columns[i];
                if (!col.Visible) 
                {
                    // Set empty rectangle for invisible columns
                    HeaderCellRects[i] = Rectangle.Empty;
                    continue;
                }
                
                int w = Math.Max(20, col.Width);
                int x = col.Sticked ? px : ux;
                
                // Validate the rectangle before creating it
                if (w > 0 && HeaderRect.Height > 0 && x >= 0)
                {
                    // Ensure the rectangle doesn't extend beyond the bounds
                    int maxWidth = Math.Max(1, RowsRect.Right - x);
                    w = Math.Min(w, maxWidth);
                    
                    HeaderCellRects[i] = new Rectangle(x, HeaderRect.Top, w, HeaderRect.Height);
                }
                else
                {
                    HeaderCellRects[i] = Rectangle.Empty;
                }
                
                if (col.Sticked) px += w; else ux += w;
            }

            // Calculate SelectAllCheckRect exactly like BeepSimpleGrid
            if (_grid.ShowCheckBox && ShowColumnHeaders && selColumn != null)
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

            // Calculate visible row range with variable row heights
            int availableHeight = RowsRect.Height;
            int startPixelOffset = _grid.Scroll.VerticalOffset;
            int startRow = _grid.Scroll.FirstVisibleRowIndex;
            
            // FIX: Find the actual visible rows based on pixel offset and available height
            int currentPixelOffset = 0;
            int actualStartRow = 0;
            
            // Find the first visible row based on pixel offset
            for (int i = 0; i < _grid.Data.Rows.Count; i++)
            {
                var row = _grid.Data.Rows[i];
                int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                
                if (currentPixelOffset + rowHeight > startPixelOffset)
                {
                    actualStartRow = i;
                    break;
                }
                currentPixelOffset += rowHeight;
            }
            
            // Calculate how many rows fit in the available height
            int endRow = actualStartRow;
            int usedHeight = 0;
            
            // Account for partial visibility of first row
            if (actualStartRow < _grid.Data.Rows.Count)
            {
                var firstRow = _grid.Data.Rows[actualStartRow];
                int firstRowHeight = firstRow.Height > 0 ? firstRow.Height : _grid.RowHeight;
                int firstRowVisibleHeight = firstRowHeight - (startPixelOffset - currentPixelOffset);
                
                if (firstRowVisibleHeight > 0)
                {
                    usedHeight += firstRowVisibleHeight;
                    endRow = actualStartRow;
                    
                    // Add more rows if they fit
                    for (int i = actualStartRow + 1; i < _grid.Data.Rows.Count && usedHeight < availableHeight; i++)
                    {
                        var row = _grid.Data.Rows[i];
                        int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                        
                        if (usedHeight + rowHeight > availableHeight)
                            break;
                            
                        usedHeight += rowHeight;
                        endRow = i;
                    }
                }
            }

            // Calculate the Y position accounting for partial row visibility
            int startRowPixelOffset = 0;
            for (int i = 0; i < actualStartRow; i++)
            {
                var row = _grid.Data.Rows[i];
                startRowPixelOffset += row.Height > 0 ? row.Height : RowHeight;
            }
            
            int y = RowsRect.Top - (startPixelOffset - startRowPixelOffset);

            int[] xmap = new int[_grid.Data.Columns.Count];
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
                xmap[i] = HeaderCellRects[i].X;

            for (int r = actualStartRow; r <= endRow && y < RowsRect.Bottom; r++)
            {
                if (r >= _grid.Data.Rows.Count) break;
                
                var row = _grid.Data.Rows[r];
                int h = row.Height > 0 ? row.Height : RowHeight; // Use individual row height
                
                // Calculate checkbox rect exactly like BeepSimpleGrid
                if (_grid.ShowCheckBox && selColumn != null && selColumn.Visible)
                {
                    int cbSize = Math.Min(h - 6, CheckBoxColumnWidth - 6);
                    row.RowCheckRect = new Rectangle(RowsRect.Left + 4, y + (h - cbSize) / 2, cbSize, cbSize);
                }
                else
                {
                    row.RowCheckRect = Rectangle.Empty;
                }

                for (int c = 0; c < row.Cells.Count && c < _grid.Data.Columns.Count; c++)
                {
                    var cell = row.Cells[c];
                    var col = _grid.Data.Columns[c];
                    if (!col.Visible) continue;
                    
                    int w = Math.Max(20, col.Width);
                    cell.Rect = new Rectangle(xmap[c], y, w, h);
                }
                y += h; // Use individual row height for positioning
            }
        }
    }
}
