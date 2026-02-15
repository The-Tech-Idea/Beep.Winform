using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Partial class containing targeted invalidation methods for BeepGridPro.
    /// </summary>
    public partial class BeepGridPro
    {
        #region Targeted Invalidation Methods

        /// <summary>
        /// Safe invalidate that checks for disposal before invalidating.
        /// </summary>
        internal void SafeInvalidate()
        {
            if (!IsDisposed && !Disposing)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Safe invalidate for a specific rectangle that checks for disposal before invalidating.
        /// </summary>
        /// <param name="rect">The rectangle to invalidate.</param>
        internal void SafeInvalidate(Rectangle rect)
        {
            if (!IsDisposed && !Disposing)
            {
                Invalidate(rect);
            }
        }

        /// <summary>
        /// Invalidates only the specified row, avoiding full grid repaint.
        /// </summary>
        /// <param name="rowIndex">The index of the row to invalidate.</param>
        public void InvalidateRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Data.Rows.Count)
                return;

            var row = Data.Rows[rowIndex];
            if (row.Cells.Count > 0)
            {
                // Get the bounding rectangle for the entire row
                var firstCell = row.Cells[0];
                var lastCell = row.Cells[row.Cells.Count - 1];
                
                if (!firstCell.Rect.IsEmpty && !lastCell.Rect.IsEmpty)
                {
                    var rowRect = new Rectangle(
                        Layout.RowsRect.Left,
                        firstCell.Rect.Top,
                        Layout.RowsRect.Width,
                        firstCell.Rect.Height
                    );
                    
                    // Only invalidate the specific row region
                    Invalidate(rowRect);
                }
            }
        }

        /// <summary>
        /// Invalidates only the specified cell, avoiding full grid repaint.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell to invalidate.</param>
        /// <param name="columnIndex">The column index of the cell to invalidate.</param>
        public void InvalidateCell(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= Data.Rows.Count)
                return;
            if (columnIndex < 0 || columnIndex >= Data.Columns.Count)
                return;

            var cell = Data.Rows[rowIndex].Cells[columnIndex];
            if (!cell.Rect.IsEmpty)
            {
                // Only invalidate the specific cell region
                Invalidate(cell.Rect);
            }
        }

        /// <summary>
        /// Invalidates a range of rows for batch updates.
        /// </summary>
        /// <param name="startRowIndex">The starting row index.</param>
        /// <param name="endRowIndex">The ending row index.</param>
        public void InvalidateRows(int startRowIndex, int endRowIndex)
        {
            if (startRowIndex < 0 || startRowIndex >= Data.Rows.Count)
                return;
            if (endRowIndex < 0 || endRowIndex >= Data.Rows.Count)
                return;
            
            int start = Math.Min(startRowIndex, endRowIndex);
            int end = Math.Max(startRowIndex, endRowIndex);
            
            if (Data.Rows[start].Cells.Count > 0 && Data.Rows[end].Cells.Count > 0)
            {
                var firstCell = Data.Rows[start].Cells[0];
                var lastRow = Data.Rows[end];
                var lastCell = lastRow.Cells[0];
                
                if (!firstCell.Rect.IsEmpty && !lastCell.Rect.IsEmpty)
                {
                    int rowHeight = lastCell.Rect.Height > 0 ? lastCell.Rect.Height : RowHeight;
                    var rangeRect = new Rectangle(
                        Layout.RowsRect.Left,
                        firstCell.Rect.Top,
                        Layout.RowsRect.Width,
                        (lastCell.Rect.Top + rowHeight) - firstCell.Rect.Top
                    );
                    
                    Invalidate(rangeRect);
                }
            }
        }

        #endregion
    }
}
