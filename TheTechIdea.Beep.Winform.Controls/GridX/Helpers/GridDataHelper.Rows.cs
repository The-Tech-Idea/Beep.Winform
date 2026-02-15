using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal partial class GridDataHelper
    {
        public void RefreshRows()
        {
            UnsubscribeRowChangeHandlers();
            Rows.Clear();
            var (enumerable, schemaTable) = GetEffectiveEnumerableWithSchema();
            var items = enumerable.Cast<object?>().ToList();

            int maxRows = System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime
                          ? Math.Min(5, items.Count) : items.Count;

            for (int i = 0; i < maxRows; i++)
            {
                var rowConfig = new BeepRowConfig { RowIndex = i, DisplayIndex = i, Height = _grid.RowHeight, RowData = items[i]! };

                if (items[i] is INotifyPropertyChanged inpc)
                {
                    if (_rowChangeHandlers.TryGetValue(inpc, out var existingHandler))
                    {
                        inpc.PropertyChanged -= existingHandler;
                    }

                    PropertyChangedEventHandler handler = (sender, e) => OnDataObjectPropertyChanged(sender, e, rowConfig);
                    _rowChangeHandlers[inpc] = handler;
                    inpc.PropertyChanged += handler;
                }

                int columnIndex = 0;

                foreach (var col in Columns)
                {
                    object? value = null;

                    if (col.IsSelectionCheckBox)
                    {
                        value = false;
                    }
                    else if (col.IsRowNumColumn)
                    {
                        value = i + 1;
                    }
                    else if (col.IsRowID)
                    {
                        value = i;
                    }
                    else if (items[i] is DataRowView drv)
                    {
                        if (drv.DataView?.Table?.Columns.Contains(col.ColumnName) == true)
                        {
                            value = drv.Row[col.ColumnName];
                        }
                    }
                    else if (items[i] is DataRow dr)
                    {
                        if (dr.Table?.Columns.Contains(col.ColumnName) == true)
                        {
                            value = dr[col.ColumnName];
                        }
                    }
                    else if (items[i] != null)
                    {
                        value = items[i]!.GetType().GetProperty(col.ColumnName)?.GetValue(items[i]);
                    }

                    var cell = new BeepCellConfig
                    {
                        RowIndex = i,
                        ColumnIndex = columnIndex,
                        DisplayIndex = columnIndex,
                        ColumnName = col.ColumnName,
                        CellValue = value!,
                        Width = col.Width,
                        Height = _grid.RowHeight,
                        IsReadOnly = col.ReadOnly,
                        IsEditable = !col.ReadOnly
                    };

                    rowConfig.Cells.Add(cell);
                    columnIndex++;
                }

                Rows.Add(rowConfig);
            }

            UpdatePageInfo();
        }

        private void UnsubscribeRowChangeHandlers()
        {
            if (_rowChangeHandlers.Count == 0) return;

            foreach (var kv in _rowChangeHandlers)
            {
                kv.Key.PropertyChanged -= kv.Value;
            }
            _rowChangeHandlers.Clear();
        }

        private void OnDataObjectPropertyChanged(object? sender, PropertyChangedEventArgs e, BeepRowConfig row)
        {
            if (sender == null || e.PropertyName == null || row == null) return;

            var column = Columns.FirstOrDefault(c => string.Equals(c.ColumnName, e.PropertyName, StringComparison.OrdinalIgnoreCase));
            if (column == null) return;

            var cell = row.Cells.FirstOrDefault(c => c.ColumnIndex == column.Index);
            if (cell == null) return;

            try
            {
                object? newValue = null;

                if (sender is DataRowView drv)
                {
                    if (drv.DataView?.Table?.Columns.Contains(column.ColumnName) == true)
                        newValue = drv[column.ColumnName];
                }
                else if (sender is DataRow dr)
                {
                    if (dr.Table?.Columns.Contains(column.ColumnName) == true)
                        newValue = dr[column.ColumnName];
                }
                else
                {
                    var prop = sender.GetType().GetProperty(column.ColumnName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    if (prop != null && prop.CanRead)
                        newValue = prop.GetValue(sender);
                }

                if (cell.CellValue != newValue && (newValue == null || !newValue.Equals(cell.CellValue)))
                {
                    cell.CellValue = newValue!;
                    cell.IsDirty = true;
                    row.IsDirty = true;

                    int rowIndex = row.RowIndex;
                    if (rowIndex < 0 || rowIndex >= Rows.Count || !ReferenceEquals(Rows[rowIndex], row))
                    {
                        rowIndex = Rows.IndexOf(row);
                    }

                    if (rowIndex >= 0 && rowIndex < Rows.Count)
                    {
                        _grid.InvalidateRow(rowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Error handling property change: {ex.Message}");
#endif
            }
        }
    }
}
