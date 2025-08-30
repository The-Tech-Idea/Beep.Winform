using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Helper class for handling all auto-sizing operations in BeepGridPro
    /// Manages both column width and row height auto-sizing with DPI awareness
    /// </summary>
    internal class GridSizingHelper
    {
        private readonly BeepGridPro _grid;

        public GridSizingHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        /// <summary>
        /// Auto-resize columns to fit their content based on the specified mode
        /// Also handles row height auto-sizing when appropriate
        /// </summary>
        public void AutoResizeColumnsToFitContent()
        {
            if (_grid.Data.Columns == null || _grid.Data.Columns.Count == 0)
                return;

            // Apply auto-sizing based on the mode
            switch (_grid.AutoSizeColumnsMode)
            {
                case DataGridViewAutoSizeColumnsMode.AllCells:
                    AutoSizeAllCellsMode();
                    AutoSizeRowsToFitContent(); // Also auto-size row heights
                    break;
                case DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader:
                    AutoSizeAllCellsExceptHeaderMode();
                    AutoSizeRowsToFitContent(); // Also auto-size row heights
                    break;
                case DataGridViewAutoSizeColumnsMode.DisplayedCells:
                    AutoSizeDisplayedCellsMode();
                    if (_grid.AutoSizeRowsToContent) AutoSizeRowsToFitContent();
                    break;
                case DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader:
                    AutoSizeDisplayedCellsExceptHeaderMode();
                    if (_grid.AutoSizeRowsToContent) AutoSizeRowsToFitContent();
                    break;
                case DataGridViewAutoSizeColumnsMode.ColumnHeader:
                    AutoSizeColumnHeaderMode();
                    if (_grid.AutoSizeRowsToContent) AutoSizeRowsToFitContent();
                    break;
                case DataGridViewAutoSizeColumnsMode.Fill:
                    AutoSizeFillMode();
                    if (_grid.AutoSizeRowsToContent) AutoSizeRowsToFitContent();
                    break;
                case DataGridViewAutoSizeColumnsMode.None:
                default:
                    // Only auto-size rows if explicitly enabled
                    if (_grid.AutoSizeRowsToContent) AutoSizeRowsToFitContent();
                    break;
            }
        }

        /// <summary>
        /// Auto-resize row heights to fit their content based on the tallest cell in each row
        /// Uses DPI-aware measurements for accurate sizing
        /// </summary>
        public void AutoSizeRowsToFitContent()
        {
            if (_grid.Data.Rows == null || _grid.Data.Rows.Count == 0)
                return;

            using (Graphics g = _grid.CreateGraphics())
            {
                foreach (var row in _grid.Data.Rows)
                {
                    int maxHeight = _grid.RowHeight; // Start with default row height

                    for (int colIndex = 0; colIndex < row.Cells.Count && colIndex < _grid.Data.Columns.Count; colIndex++)
                    {
                        var cell = row.Cells[colIndex];
                        var column = _grid.Data.Columns[colIndex];

                        // Skip system columns for height calculation
                        if (column.IsSelectionCheckBox || column.IsRowNumColumn || column.IsRowID)
                            continue;

                        string cellText = cell.CellValue?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(cellText))
                        {
                            int requiredHeight = CalculateOptimalTextHeight(g, cellText, _grid.Font, column.Width);
                            maxHeight = Math.Max(maxHeight, requiredHeight);
                        }
                    }

                    // Cap the maximum height to prevent extremely tall rows
                    row.Height = Math.Min(maxHeight, 200); // Max 200px height
                    
                    // Update all cells in the row to have the same height
                    foreach (var cell in row.Cells)
                    {
                        cell.Height = row.Height;
                    }
                }
            }
        }

        /// <summary>
        /// Calculate optimal width for a column based on its content
        /// </summary>
        /// <param name="column">The column to measure</param>
        /// <param name="includeHeader">Whether to include header text in measurement</param>
        /// <param name="allRows">Whether to measure all rows or just visible rows</param>
        /// <returns>Optimal width in pixels</returns>
        public int GetColumnWidth(BeepColumnConfig column, bool includeHeader, bool allRows)
        {
            if (column == null) return 100;

            int maxWidth = 50; // Minimum width
            int padding = 8; // Padding for text

            using (Graphics g = _grid.CreateGraphics())
            {
                // Measure header text if requested
                if (includeHeader)
                {
                    string headerText = column.ColumnCaption ?? column.ColumnName ?? "";
                    if (!string.IsNullOrEmpty(headerText))
                    {
                        using (var headerFont = new Font(_grid.Font.FontFamily, _grid.Font.Size, FontStyle.Bold))
                        {
                            SizeF headerSize = g.MeasureString(headerText, headerFont);
                            maxWidth = Math.Max(maxWidth, (int)headerSize.Width + padding);
                        }
                    }

                    // Add space for sort/filter icons if enabled
                    if (column.ShowSortIcon || column.ShowFilterIcon)
                    {
                        int iconSpace = 0;
                        if (column.ShowSortIcon) iconSpace += 20;
                        if (column.ShowFilterIcon) iconSpace += 20;
                        maxWidth += iconSpace + 4;
                    }
                }

                // Measure cell content
                if (_grid.Data.Rows != null && _grid.Data.Rows.Any())
                {
                    var rowsToMeasure = allRows ? _grid.Data.Rows : _grid.Data.Rows.Take(Math.Min(50, _grid.Data.Rows.Count));
                    int columnIndex = _grid.Data.Columns.IndexOf(column);

                    if (columnIndex >= 0)
                    {
                        foreach (var row in rowsToMeasure)
                        {
                            if (columnIndex < row.Cells.Count)
                            {
                                var cell = row.Cells[columnIndex];
                                string cellText = cell.CellValue?.ToString() ?? "";

                                if (!string.IsNullOrEmpty(cellText))
                                {
                                    SizeF cellSize = g.MeasureString(cellText, _grid.Font);
                                    maxWidth = Math.Max(maxWidth, (int)cellSize.Width + padding);
                                }
                            }
                        }
                    }
                }
            }

            // Cap the maximum width to prevent extremely wide columns
            return Math.Min(maxWidth, 400);
        }

        private void AutoSizeAllCellsMode()
        {
            foreach (var column in _grid.Data.Columns.Where(c => c.Visible))
            {
                int maxWidth = GetColumnWidth(column, true, true);
                column.Width = maxWidth;
            }
        }

        private void AutoSizeAllCellsExceptHeaderMode()
        {
            foreach (var column in _grid.Data.Columns.Where(c => c.Visible))
            {
                int maxWidth = GetColumnWidth(column, false, true);
                column.Width = maxWidth;
            }
        }

        private void AutoSizeDisplayedCellsMode()
        {
            foreach (var column in _grid.Data.Columns.Where(c => c.Visible))
            {
                int maxWidth = GetColumnWidth(column, true, false);
                column.Width = maxWidth;
            }
        }

        private void AutoSizeDisplayedCellsExceptHeaderMode()
        {
            foreach (var column in _grid.Data.Columns.Where(c => c.Visible))
            {
                int maxWidth = GetColumnWidth(column, false, false);
                column.Width = maxWidth;
            }
        }

        private void AutoSizeColumnHeaderMode()
        {
            using (Graphics g = _grid.CreateGraphics())
            {
                using (var headerFont = new Font(_grid.Font.FontFamily, _grid.Font.Size, FontStyle.Bold))
                {
                    foreach (var column in _grid.Data.Columns.Where(c => c.Visible))
                    {
                        string headerText = column.ColumnCaption ?? column.ColumnName ?? "";
                        if (!string.IsNullOrEmpty(headerText))
                        {
                            SizeF headerSize = g.MeasureString(headerText, headerFont);
                            column.Width = Math.Max(50, (int)headerSize.Width + 20); // 20px padding
                        }
                    }
                }
            }
        }

        private void AutoSizeFillMode()
        {
            var visibleColumns = _grid.Data.Columns.Where(c => c.Visible && !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID).ToList();
            if (!visibleColumns.Any()) return;

            // Calculate total width available
            int availableWidth = _grid.Layout.RowsRect.Width;
            
            // Subtract system column widths
            if (_grid.ShowCheckBox)
                availableWidth -= _grid.Layout.CheckBoxColumnWidth;
            
            var rowNumColumn = _grid.Data.Columns.FirstOrDefault(c => c.IsRowNumColumn && c.Visible);
            if (rowNumColumn != null)
                availableWidth -= rowNumColumn.Width;

            // Reserve space for vertical scrollbar if needed
            if (_grid.ScrollBars.IsVerticalScrollBarNeeded)
                availableWidth -= 15; // Approximate scrollbar width

            if (availableWidth <= 0) return;

            // Distribute width equally among data columns
            int columnWidth = Math.Max(50, availableWidth / visibleColumns.Count);
            foreach (var column in visibleColumns)
            {
                column.Width = columnWidth;
            }
        }

        /// <summary>
        /// Calculate optimal text height with minimal padding for better accuracy
        /// Takes DPI scaling into account for accurate measurements
        /// </summary>
        /// <param name="g">Graphics object for measurement</param>
        /// <param name="text">Text to measure</param>
        /// <param name="font">Font to use</param>
        /// <param name="columnWidth">Available width for text</param>
        /// <returns>Optimal height in pixels</returns>
        private int CalculateOptimalTextHeight(Graphics g, string text, Font font, int columnWidth)
        {
            if (string.IsNullOrEmpty(text))
                return _grid.RowHeight;

            // Calculate available text width (subtract padding for cell margins)
            int textPadding = Math.Max(2, _grid.RowAutoSizePadding);
            int availableWidth = Math.Max(50, columnWidth - textPadding);
            
            // Use simple measurement for now - DPI scaling will be handled differently
            Font measureFont = font;
            
            try
            {
                // First check if text fits in single line
                Size singleLineSize = TextRenderer.MeasureText(g, text, measureFont, 
                    new Size(availableWidth, int.MaxValue), 
                    TextFormatFlags.NoPrefix | TextFormatFlags.PreserveGraphicsClipping);
                
                // If text fits in single line, use single line height with configurable padding
                if (singleLineSize.Width <= availableWidth)
                {
                    return singleLineSize.Height + _grid.RowAutoSizePadding;
                }
                
                // For multi-line text, use TextRenderer for more accurate measurement
                Size multiLineSize = TextRenderer.MeasureText(g, text, measureFont,
                    new Size(availableWidth, int.MaxValue),
                    TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix | TextFormatFlags.PreserveGraphicsClipping);
                
                return multiLineSize.Height + _grid.RowAutoSizePadding + 2; // Extra 2px for multi-line readability
            }
            finally
            {
                // No cleanup needed since we're not creating a new font
            }
        }

        /// <summary>
        /// Set a specific column width by name
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="width">New width in pixels</param>
        public void SetColumnWidth(string columnName, int width)
        {
            var column = _grid.GetColumnByName(columnName);
            if (column != null)
            {
                column.Width = Math.Max(20, width); // Ensure minimum width
            }
        }
    }
}