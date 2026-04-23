using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Row visibility and calculation methods for virtualization
    /// </summary>
    internal partial class GridRenderHelper
    {
        /// <summary>
        /// Gets the number of rows that can fit in the current viewport
        /// </summary>
        public int GetVisibleRowCapacity()
        {
            if (_grid?.Layout == null) return 10;
            
            var rowsRect = _grid.Layout.RowsRect;
            if (rowsRect.Height <= 0) return 10;
            
            int rowHeight = _grid.RowHeight;
            if (rowHeight <= 0) rowHeight = 24;
            
            return Math.Max(1, rowsRect.Height / rowHeight);
        }

        /// <summary>
        /// Gets the index of the first fully or partially visible row
        /// </summary>
        public int GetFirstVisibleRowIndex()
        {
            return _grid?.Scroll?.FirstVisibleRowIndex ?? 0;
        }

        /// <summary>
        /// Gets the index of the last visible row
        /// </summary>
        public int GetLastVisibleRowIndex()
        {
            if (_grid?.Data?.Rows == null || _grid.Layout == null) return 0;
            
            int firstVisible = GetFirstVisibleRowIndex();
            int capacity = GetVisibleRowCapacity();
            int lastPossible = Math.Min(firstVisible + capacity - 1, _grid.Data.Rows.Count - 1);
            
            return Math.Max(0, lastPossible);
        }

        /// <summary>
        /// Gets the actual count of visible rows
        /// </summary>
        public int GetActualVisibleRowCount()
        {
            if (_grid?.Data?.Rows == null || _grid.Layout == null) return 0;
            
            var rows = _grid.Data.Rows;
            var rowsRect = _grid.Layout.RowsRect;
            int firstVisible = GetFirstVisibleRowIndex();
            int verticalOffset = _grid?.Scroll?.VerticalOffset ?? 0;
            
            return GetVisibleRowCount(rows, rowsRect.Height, firstVisible, verticalOffset);
        }

        /// <summary>
        /// Calculates how many rows can fit in the available height starting from a specific row
        /// </summary>
        private int GetVisibleRowCount(BindingList<BeepRowConfig> rows, int availableHeight, 
            int startRow, int pixelOffset)
        {
            if (rows == null || rows.Count == 0) return 0;
            
            int visibleCount = 0;
            int usedHeight = 0;
            int headerHeight = _grid.GroupEngine?.GetHeaderHeight() ?? 0;
            
            if (startRow < rows.Count)
            {
                var firstRow = rows[startRow];
                int firstRowHeight = firstRow.Height > 0 ? firstRow.Height : _grid.RowHeight;
                
                int totalOffsetToFirstRow = 0;
                for (int i = 0; i < startRow; i++)
                {
                    var row = rows[i];
                    if (!row.IsVisible) continue;  // skip filtered-out rows
                    totalOffsetToFirstRow += row.Height > 0 ? row.Height : _grid.RowHeight;
                }
                totalOffsetToFirstRow += (_grid.GroupEngine?.GetHeaderCountBeforeRow(startRow) ?? 0) * headerHeight;
                
                int firstRowVisibleHeight = firstRowHeight - (pixelOffset - totalOffsetToFirstRow);
                if (firstRowVisibleHeight > 0)
                {
                    usedHeight += firstRowVisibleHeight;
                    visibleCount++;
                }
                
                for (int i = startRow + 1; i < rows.Count && usedHeight < availableHeight; i++)
                {
                    // Account for group headers starting at this row
                    int newHeaders = (_grid.GroupEngine?.GetHeaderCountBeforeRow(i + 1) ?? 0) - (_grid.GroupEngine?.GetHeaderCountBeforeRow(i) ?? 0);
                    if (newHeaders > 0)
                    {
                        int hh = newHeaders * headerHeight;
                        if (usedHeight + hh > availableHeight)
                            break;
                        usedHeight += hh;
                    }

                    var row = rows[i];
                    if (!row.IsVisible) continue;  // skip filtered-out rows
                    int rowHeight = row.Height > 0 ? row.Height : _grid.RowHeight;
                    
                    if (usedHeight + rowHeight > availableHeight)
                        break;
                        
                    usedHeight += rowHeight;
                    visibleCount++;
                }
            }
            
            return visibleCount;
        }
    }
}
