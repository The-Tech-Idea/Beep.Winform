using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Partial class containing data access methods for BeepGridPro.
    /// </summary>
    public partial class BeepGridPro
    {
        #region BeepSimpleGrid Compatibility Helpers
        /// <summary>
        /// Gets a column by its name.
        /// </summary>
        /// <param name="columnName">The name of the column to find.</param>
        /// <returns>The column configuration or null if not found.</returns>
        public BeepColumnConfig? GetColumnByName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return null;
            return Data.Columns.FirstOrDefault(c => string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a column by its caption.
        /// </summary>
        /// <param name="caption">The caption of the column to find.</param>
        /// <returns>The column configuration or null if not found.</returns>
        public BeepColumnConfig? GetColumnByCaption(string caption)
        {
            if (string.IsNullOrWhiteSpace(caption)) return null;
            return Data.Columns.FirstOrDefault(c => string.Equals(caption, c.ColumnCaption, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a column by its index.
        /// </summary>
        /// <param name="index">The index of the column.</param>
        /// <returns>The column configuration or null if index is out of range.</returns>
        public BeepColumnConfig? GetColumnByIndex(int index)
        {
            if (index < 0 || index >= Data.Columns.Count) return null;
            return Data.Columns[index];
        }

        /// <summary>
        /// Gets all columns as a dictionary keyed by column name.
        /// </summary>
        /// <returns>A dictionary of column configurations.</returns>
        public Dictionary<string, BeepColumnConfig> GetDictionaryColumns()
        {
            return Data.Columns.ToDictionary(c => c.ColumnName, c => c, StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region Two-Way Binding
        public bool SetCellValue(int rowIndex, int columnIndex, object? value)
        {
            if (rowIndex < 0 || rowIndex >= Data.Rows.Count ||
                columnIndex < 0 || columnIndex >= Data.Columns.Count)
                return false;

            var column = Data.Columns[columnIndex];
            if (ReadOnly || column.ReadOnly || column.IsSelectionCheckBox ||
                column.IsRowNumColumn || column.IsRowID)
                return false;

            var row = Data.Rows[rowIndex];
            object? appliedValue = value;
            if (!column.IsUnbound)
            {
                if (Data.Binding == null)
                {
                    ReportOperationError("Binding.SetCellValue",
                        new InvalidOperationException("The grid is not bound to a writable data source."));
                    return false;
                }

                if (!Data.Binding.TrySetValue(row.RowData, column.ColumnName, value,
                        out appliedValue, out var error))
                {
                    ReportOperationError("Binding.SetCellValue",
                        error ?? new InvalidOperationException("Could not set the cell value."));
                    return false;
                }
            }

            var cell = row.Cells.FirstOrDefault(c =>
                string.Equals(c.ColumnName, column.ColumnName, StringComparison.OrdinalIgnoreCase));
            if (cell != null)
            {
                cell.CellValue = appliedValue!;
                cell.IsDirty = true;
                OnCellValueChanged(cell);
            }

            row.IsDirty = true;
            Footer.Invalidate();
            if (GroupEngine?.IsGrouped == true)
                GroupEngine.ApplyGrouping();
            RefreshViewAfterEdit(row.RowData, columnIndex);
            InvalidateRow(rowIndex);
            return true;
        }

        internal void RefreshViewAfterEdit(object? item, int columnIndex)
        {
            if (!SortFilter.HasActiveSort && !SortFilter.HasActiveFilters &&
                (_activeFilter?.Criteria.Count ?? 0) == 0)
                return;

            SortFilter.ReapplyView();

            int targetRow = -1;
            for (int i = 0; i < Rows.Count; i++)
            {
                if (ReferenceEquals(Rows[i].RowData, item) || Equals(Rows[i].RowData, item))
                {
                    if (Rows[i].IsVisible) targetRow = i;
                    break;
                }
            }

            if (targetRow < 0)
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    if (Rows[i].IsVisible)
                    {
                        targetRow = i;
                        break;
                    }
                }
            }

            if (targetRow >= 0)
            {
                int column = Math.Max(0, Math.Min(columnIndex,
                    Math.Max(0, Data.Columns.Count - 1)));
                Selection.SelectCell(targetRow, column);
                Scroll?.SetVerticalIndex(targetRow);
            }
            else
            {
                Selection.Clear();
            }

            SafeInvalidate();
            UpdateAccessibleDescription();
        }

        public bool SetCellValue(int rowIndex, string columnName, object? value)
        {
            var column = GetColumnByName(columnName);
            return column != null && SetCellValue(rowIndex, column.Index, value);
        }

        public object? GetCellValue(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= Data.Rows.Count ||
                columnIndex < 0 || columnIndex >= Data.Rows[rowIndex].Cells.Count)
                return null;

            return Data.Rows[rowIndex].Cells[columnIndex].CellValue;
        }
        #endregion

        #region Data Synchronization
        /// <summary>
        /// The current ordered/visible view of the grid. Exporters and hosts
        /// should read this instead of walking <see cref="Data.Rows"/> and
        /// filtering on <c>IsVisible</c> themselves.
        /// </summary>
        [Browsable(false)]
        public Query.GridView View => DataController.BuildView();

        /// <summary>
        /// Syncs changes from the row's RowDataObject back to the cell values and updates the grid display.
        /// Call this after modifying properties on the RowDataObject to reflect changes in the grid.
        /// </summary>
        /// <param name="row">The row whose data object was modified</param>
        public void SyncRowDataToGrid(BeepRowConfig row)
        {
            if (row == null || row.RowDataObject == null) return;

            // Update each cell from the data object
            foreach (var cell in row.Cells)
            {
                if (cell.ColumnIndex >= 0 && cell.ColumnIndex < Data.Columns.Count)
                {
                    var column = Data.Columns[cell.ColumnIndex];
                    
                    // Skip system columns
                    if (column.IsSelectionCheckBox || column.IsRowNumColumn || column.IsRowID)
                        continue;

                    // Get value from data object
                    try
                    {
                        object? newValue = null;

                        if (Data.Binding != null)
                        {
                            newValue = Data.Binding.GetValue(row.RowDataObject, column.ColumnName);
                        }
                        else if (row.RowDataObject is System.Data.DataRowView drv)
                        {
                            if (drv.DataView?.Table?.Columns.Contains(column.ColumnName) == true)
                                newValue = drv[column.ColumnName];
                        }
                        else if (row.RowDataObject is System.Data.DataRow dr)
                        {
                            if (dr.Table?.Columns.Contains(column.ColumnName) == true)
                                newValue = dr[column.ColumnName];
                        }
                        else if (!string.IsNullOrEmpty(column.ColumnName))
                        {
                            var prop = row.RowDataObject.GetType().GetProperty(column.ColumnName,
                                System.Reflection.BindingFlags.Public | 
                                System.Reflection.BindingFlags.Instance | 
                                System.Reflection.BindingFlags.IgnoreCase);
                            
                            if (prop != null && prop.CanRead)
                                newValue = prop.GetValue(row.RowDataObject);
                        }

                        // Update cell if value changed
                        if (newValue != cell.CellValue && 
                            (newValue == null || !newValue.Equals(cell.CellValue)))
                        {
                            cell.CellValue = newValue;
                            cell.IsDirty = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        GridErrorReporter.Operation(this, "Data.SyncRow", ex);
                    }
                }
            }

            // Mark row as dirty and refresh display
            row.IsDirty = true;
            InvalidateRow(row.RowIndex);
        }

        /// <summary>
        /// Syncs changes from row data objects back to grid for multiple rows.
        /// Useful after batch updates to RowDataObjects.
        /// </summary>
        /// <param name="rows">The rows to sync</param>
        public void SyncRowDataToGrid(IEnumerable<BeepRowConfig> rows)
        {
            if (rows == null) return;

            foreach (var row in rows)
            {
                SyncRowDataToGrid(row);
            }
        }
        #endregion
    }
}
