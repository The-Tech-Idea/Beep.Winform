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
            _grid.Footer.Invalidate();
            _grid.Layout?.RowMetrics.Invalidate();
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
                    else if (Binding != null)
                    {
                        value = Binding.GetValue(items[i], col.ColumnName);
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
                        value = GetCachedProperty(items[i]!.GetType(), col.ColumnName)?.GetValue(items[i]);
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

            UpdateRowNumberColumnWidth();
            UpdatePageInfo();
        }

        private void UpdateRowNumberColumnWidth()
        {
            var column = Columns.FirstOrDefault(c => c.IsRowNumColumn);
            if (column == null) return;

            string sample = new string('9', Math.Max(1, Rows.Count.ToString().Length));
            int measured = System.Windows.Forms.TextRenderer.MeasureText(sample, _grid.Font).Width + 12;
            int min = column.MinWidth > 0 ? column.MinWidth : 30;
            int max = column.MaxWidth > 0 ? column.MaxWidth : 120;
            column.Width = System.Math.Clamp(measured, min, max);
        }

        internal void SyncRowCellsToColumns()
        {
            if (Rows.Count == 0) return;

            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                var row = Rows[rowIndex];
                var existing = new Dictionary<string, BeepCellConfig>(StringComparer.OrdinalIgnoreCase);
                foreach (var cell in row.Cells)
                {
                    if (!string.IsNullOrWhiteSpace(cell.ColumnName) && !existing.ContainsKey(cell.ColumnName))
                        existing[cell.ColumnName] = cell;
                }

                var newCells = new List<BeepCellConfig>(Columns.Count);
                for (int columnIndex = 0; columnIndex < Columns.Count; columnIndex++)
                {
                    var column = Columns[columnIndex];
                    if (!existing.TryGetValue(column.ColumnName, out var cell))
                    {
                        cell = new BeepCellConfig
                        {
                            RowIndex = rowIndex,
                            ColumnIndex = columnIndex,
                            DisplayIndex = columnIndex,
                            ColumnName = column.ColumnName,
                            CellValue = GetCellValueForColumn(row, column, rowIndex),
                            Width = column.Width,
                            Height = row.Height > 0 ? row.Height : _grid.RowHeight,
                            IsReadOnly = column.ReadOnly,
                            IsEditable = !column.ReadOnly,
                            IsVisible = true
                        };
                    }
                    else
                    {
                        cell.RowIndex = rowIndex;
                        cell.ColumnIndex = columnIndex;
                        cell.DisplayIndex = columnIndex;
                        cell.ColumnName = column.ColumnName;
                        cell.IsReadOnly = column.ReadOnly;
                        cell.IsEditable = !column.ReadOnly;
                    }

                    newCells.Add(cell);
                }

                row.Cells.Clear();
                foreach (var cell in newCells)
                    row.Cells.Add(cell);
            }

            _grid.Layout.RowMetrics.Invalidate();
            _grid.Layout.Recalculate();
            _grid.SafeInvalidate();
        }

        private object? GetCellValueForColumn(BeepRowConfig row, BeepColumnConfig column, int rowIndex)
        {
            if (column.IsSelectionCheckBox) return false;
            if (column.IsRowNumColumn) return rowIndex + 1;
            if (column.IsRowID) return rowIndex;
            if (column.IsUnbound) return null;

            if (Binding != null)
            {
                try
                {
                    return Binding.GetValue(row.RowData, column.ColumnName);
                }
                catch (Exception ex)
                {
                    GridErrorReporter.Operation(_grid, "Binding.GetValue", ex);
                    return null;
                }
            }

            return row.RowData == null
                ? null
                : GetCachedProperty(row.RowData.GetType(), column.ColumnName)?.GetValue(row.RowData);
        }

        internal void SubscribeRowToInpc(Models.BeepRowConfig rowConfig)
        {
            if (rowConfig?.RowData is not INotifyPropertyChanged inpc) return;

            if (_rowChangeHandlers.TryGetValue(inpc, out var existingHandler))
                inpc.PropertyChanged -= existingHandler;

            PropertyChangedEventHandler handler = (sender, e) => OnDataObjectPropertyChanged(sender, e, rowConfig);
            _rowChangeHandlers[inpc] = handler;
            inpc.PropertyChanged += handler;
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
                    var prop = GetCachedProperty(sender.GetType(), column.ColumnName);

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
                        _grid.Footer.Invalidate();
                        _grid.InvalidateRow(rowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                GridErrorReporter.Operation(_grid, "Data.PropertyChanged", ex);
            }
        }
    }
}
