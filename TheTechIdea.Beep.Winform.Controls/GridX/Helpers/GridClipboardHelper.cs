using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Options for clipboard copy operations
    /// </summary>
    public enum CopyOptions
    {
        /// <summary>Copy with column headers</summary>
        WithHeaders,
        /// <summary>Copy without column headers</summary>
        WithoutHeaders,
        /// <summary>Copy only visible columns</summary>
        VisibleColumnsOnly,
        /// <summary>Copy all columns including hidden</summary>
        AllColumns
    }

    /// <summary>
    /// Options for clipboard paste operations
    /// </summary>
    public enum PasteOptions
    {
        /// <summary>Paste values and formatting</summary>
        All,
        /// <summary>Paste values only</summary>
        ValuesOnly,
        /// <summary>Paste formatting only</summary>
        FormattingOnly
    }

    /// <summary>
    /// Helper class for handling clipboard operations (Copy/Paste/Cut) in BeepGridPro
    /// Supports Excel-compatible tab-delimited format
    /// </summary>
    internal class GridClipboardHelper
    {
        private readonly BeepGridPro _grid;
        private List<CellClipboardData> _cutData;

        public GridClipboardHelper(BeepGridPro grid)
        {
            _grid = grid;
            _cutData = new List<CellClipboardData>();
        }

        /// <summary>
        /// Copy selected cells/rows to clipboard with options
        /// </summary>
        public void CopyToClipboard(bool includeHeaders = true, bool visibleColumnsOnly = true)
        {
            if (_grid.Data?.Rows == null || _grid.Data.Rows.Count == 0)
                return;

            try
            {
                var selectedData = GetSelectedData();
                if (selectedData.Count == 0)
                    return;

                // Get columns to include
                var columnsToInclude = visibleColumnsOnly
                    ? _grid.Data.Columns.Where(c => c.Visible && !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID).ToList()
                    : _grid.Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID).ToList();

                // Build tab-delimited text
                var sb = new StringBuilder();

                // Add header row if requested
                if (includeHeaders)
                {
                    sb.AppendLine(string.Join("\t", columnsToInclude.Select(c => c.ColumnCaption ?? c.ColumnName)));
                }

                // Add data rows
                foreach (var rowData in selectedData)
                {
                    var values = new List<string>();
                    foreach (var column in columnsToInclude)
                    {
                        int colIndex = _grid.Data.Columns.IndexOf(column);
                        if (colIndex >= 0 && colIndex < rowData.Cells.Count)
                        {
                            string value = rowData.Cells[colIndex]?.ToString() ?? "";
                            // Escape tabs and newlines
                            value = value.Replace("\t", "    ").Replace("\r\n", " ").Replace("\n", " ");
                            values.Add(value);
                        }
                        else
                        {
                            values.Add("");
                        }
                    }
                    sb.AppendLine(string.Join("\t", values));
                }

                // Copy to clipboard
                if (sb.Length > 0)
                {
                    Clipboard.SetText(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying to clipboard: {ex.Message}", "Copy Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cut selected cells/rows to clipboard (copy and mark for deletion)
        /// </summary>
        public void CutToClipboard(bool includeHeaders = true, bool visibleColumnsOnly = true)
        {
            if (_grid.Data?.Rows == null || _grid.Data.Rows.Count == 0)
                return;

            try
            {
                // First copy the data
                CopyToClipboard(includeHeaders, visibleColumnsOnly);

                // Store cut data for later clearing
                _cutData.Clear();
                var selectedRows = _grid.Data.Rows.Where(r => r.IsSelected).ToList();
                
                if (selectedRows.Count == 0 && _grid.Selection.RowIndex >= 0)
                {
                    if (_grid.Selection.RowIndex < _grid.Data.Rows.Count)
                    {
                        selectedRows.Add(_grid.Data.Rows[_grid.Selection.RowIndex]);
                    }
                }

                var columnsToInclude = visibleColumnsOnly
                    ? _grid.Data.Columns.Where(c => c.Visible && !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID).ToList()
                    : _grid.Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID).ToList();

                // Mark cells as cut
                foreach (var row in selectedRows)
                {
                    int rowIndex = _grid.Data.Rows.IndexOf(row);
                    foreach (var column in columnsToInclude)
                    {
                        int colIndex = _grid.Data.Columns.IndexOf(column);
                        if (colIndex >= 0 && colIndex < row.Cells.Count)
                        {
                            _cutData.Add(new CellClipboardData
                            {
                                RowIndex = rowIndex,
                                ColumnIndex = colIndex,
                                OriginalValue = row.Cells[colIndex].CellValue,
                                OriginalForeColor = row.Cells[colIndex].CellForeColor,
                                OriginalBackColor = row.Cells[colIndex].CellBackColor
                            });
                            
                            // Visual indication of cut (lighter color)
                            row.Cells[colIndex].CellForeColor = "#D3D3D3"; // LightGray
                        }
                    }
                }

                _grid.SafeInvalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cutting to clipboard: {ex.Message}", "Cut Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Paste data from clipboard with options
        /// </summary>
        public void PasteFromClipboard(PasteOptions pasteOption = PasteOptions.All)
        {
            if (_grid.Data?.Rows == null || !Clipboard.ContainsText())
                return;

            try
            {
                string clipboardText = Clipboard.GetText();
                if (string.IsNullOrEmpty(clipboardText))
                    return;

                // Clear cut data if pasting after cut
                ClearCutCells();

                // Parse clipboard data
                var lines = clipboardText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                    return;

                // Determine starting position
                int startRowIndex = _grid.Selection.RowIndex >= 0 
                    ? _grid.Selection.RowIndex 
                    : 0;
                
                int startColIndex = _grid.Selection.ColumnIndex >= 0 
                    ? _grid.Selection.ColumnIndex 
                    : 0;

                // Skip header if it looks like header data
                int dataStartLine = 0;
                if (lines.Length > 1 && IsHeaderRow(lines[0]))
                {
                    dataStartLine = 1;
                }

                // Get visible data columns
                var visibleColumns = _grid.Data.Columns
                    .Where(c => c.Visible && !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID)
                    .ToList();

                // Paste data
                int rowsPasted = 0;

                for (int i = dataStartLine; i < lines.Length; i++)
                {
                    int targetRowIndex = startRowIndex + (i - dataStartLine);
                    string[] values = lines[i].Split('\t');

                    // Create new row if needed
                    if (targetRowIndex >= _grid.Data.Rows.Count)
                    {
                        _grid.Navigator.InsertNew();
                        targetRowIndex = _grid.Data.Rows.Count - 1;
                    }

                    if (targetRowIndex < _grid.Data.Rows.Count)
                    {
                        var row = _grid.Data.Rows[targetRowIndex];
                        
                        // Paste values into cells
                        for (int j = 0; j < values.Length && (startColIndex + j) < visibleColumns.Count; j++)
                        {
                            var column = visibleColumns[startColIndex + j];
                            int colIndex = _grid.Data.Columns.IndexOf(column);

                            if (colIndex >= 0 && colIndex < row.Cells.Count)
                            {
                                var cell = row.Cells[colIndex];
                                
                                switch (pasteOption)
                                {
                                    case PasteOptions.All:
                                        // Paste both value and formatting
                                        object convertedValue = ConvertValue(values[j], column.DataType) ?? string.Empty;
                                        cell.CellValue = convertedValue;
                                        // Mark row as modified (no State property, just update value)
                                        break;
                                        
                                    case PasteOptions.ValuesOnly:
                                        // Paste only the value, keep existing formatting
                                        object value = ConvertValue(values[j], column.DataType) ?? string.Empty;
                                        cell.CellValue = value;
                                        break;
                                        
                                    case PasteOptions.FormattingOnly:
                                        // Formatting not supported with plain text clipboard
                                        break;
                                }
                            }
                        }
                        rowsPasted++;
                    }
                }

                // Refresh grid
                _grid.SafeInvalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error pasting from clipboard: {ex.Message}", "Paste Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Clear cells that were cut
        /// </summary>
        private void ClearCutCells()
        {
            if (_cutData.Count == 0)
                return;

            foreach (var cutCell in _cutData)
            {
                if (cutCell.RowIndex >= 0 && cutCell.RowIndex < _grid.Data.Rows.Count)
                {
                    var row = _grid.Data.Rows[cutCell.RowIndex];
                    if (cutCell.ColumnIndex >= 0 && cutCell.ColumnIndex < row.Cells.Count)
                    {
                        var cell = row.Cells[cutCell.ColumnIndex];
                        cell.CellValue = string.Empty; // Clear the value
                        cell.CellForeColor = cutCell.OriginalForeColor ?? string.Empty;
                        cell.CellBackColor = cutCell.OriginalBackColor ?? string.Empty;
                        // Mark row as modified if needed (no State property, use event)
                        _grid.OnCellValueChanged(cell);
                    }
                }
            }

            _cutData.Clear();
        }

        /// <summary>
        /// Get selected row data for copying
        /// </summary>
        private List<CellClipboardRowData> GetSelectedData()
        {
            var selectedData = new List<CellClipboardRowData>();

            // Get selected rows
            var selectedRows = _grid.Data.Rows
                .Where(r => r.IsSelected)
                .ToList();

            // If no rows selected, use current row
            if (selectedRows.Count == 0 && _grid.Selection.RowIndex >= 0)
            {
                if (_grid.Selection.RowIndex < _grid.Data.Rows.Count)
                {
                    selectedRows.Add(_grid.Data.Rows[_grid.Selection.RowIndex]);
                }
            }

            // Extract cell data
            foreach (var row in selectedRows)
            {
                var rowData = new CellClipboardRowData
                {
                    Cells = row.Cells.Select(c => c.CellValue?.ToString() ?? "").ToList()
                };
                selectedData.Add(rowData);
            }

            return selectedData;
        }

        /// <summary>
        /// Check if a line looks like a header row
        /// </summary>
        private bool IsHeaderRow(string line)
        {
            var fields = line.Split('\t');
            if (fields.Length == 0)
                return false;

            var firstField = fields[0].Trim();
            return _grid.Data.Columns.Any(c => 
                string.Equals(c.ColumnName, firstField, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.ColumnCaption, firstField, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Convert string value to appropriate data type
        /// </summary>
        private object? ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                if (targetType == typeof(string))
                    return value;

                if (targetType == typeof(int) || targetType == typeof(int?))
                {
                    if (int.TryParse(value, out int intValue))
                        return intValue;
                }
                else if (targetType == typeof(decimal) || targetType == typeof(decimal?))
                {
                    if (decimal.TryParse(value, out decimal decValue))
                        return decValue;
                }
                else if (targetType == typeof(double) || targetType == typeof(double?))
                {
                    if (double.TryParse(value, out double dblValue))
                        return dblValue;
                }
                else if (targetType == typeof(bool) || targetType == typeof(bool?))
                {
                    if (bool.TryParse(value, out bool boolValue))
                        return boolValue;
                    if (value == "1" || value.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (value == "0" || value.Equals("No", StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                else if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                {
                    if (DateTime.TryParse(value, out DateTime dateValue))
                        return dateValue;
                }

                return value;
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        /// Copy only the selected cell value
        /// </summary>
        public void CopyCellToClipboard()
        {
            if (_grid.Selection.RowIndex < 0 || _grid.Selection.ColumnIndex < 0)
                return;

            if (_grid.Selection.RowIndex >= _grid.Data.Rows.Count)
                return;

            var row = _grid.Data.Rows[_grid.Selection.RowIndex];
            if (_grid.Selection.ColumnIndex >= row.Cells.Count)
                return;

            var cell = row.Cells[_grid.Selection.ColumnIndex];
            string value = cell.CellValue?.ToString() ?? "";

            try
            {
                Clipboard.SetText(value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying cell: {ex.Message}", "Copy Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// Data structure for clipboard row data
    /// </summary>
    internal class CellClipboardRowData
    {
        public List<string> Cells { get; set; } = new List<string>();
    }

    /// <summary>
    /// Data structure for cut cell tracking
    /// </summary>
    internal class CellClipboardData
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public object? OriginalValue { get; set; }
        public string? OriginalForeColor { get; set; }
        public string? OriginalBackColor { get; set; }
    }
}
