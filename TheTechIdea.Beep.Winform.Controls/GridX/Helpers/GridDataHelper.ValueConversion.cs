using System;
using System.Data;
using System.Reflection;

using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal partial class GridDataHelper
    {
        public void UpdateCellValue(BeepCellConfig cell, object newValue)
        {
            if (cell == null) return;

            var column = Columns[cell.ColumnIndex];
            if (column.ReadOnly || !cell.IsEditable) return;

            var normalizedValue = NormalizeEditorValue(newValue, column, cell);

            if (cell.RowIndex >= 0 && cell.RowIndex < Rows.Count)
            {
                var row = Rows[cell.RowIndex];
                if (!column.IsUnbound && row.RowData != null && !string.IsNullOrEmpty(column.ColumnName))
                {
                    if (Binding != null)
                    {
                        if (!Binding.TrySetValue(row.RowData, column.ColumnName, normalizedValue,
                                out var appliedValue, out var error, row.RowIndex))
                        {
                            _grid?.ReportOperationError("Data.UpdateCellValue",
                                error ?? new InvalidOperationException("Could not update the bound value."));
                            return;
                        }

                        normalizedValue = appliedValue;
                    }
                    else
                    {
                        try
                        {
                            if (row.RowData is DataRowView drv)
                            {
                                if (drv.DataView?.Table?.Columns.Contains(column.ColumnName) == true)
                                    drv.Row[column.ColumnName] = normalizedValue ?? DBNull.Value;
                            }
                            else if (row.RowData is DataRow dr)
                            {
                                if (dr.Table?.Columns.Contains(column.ColumnName) == true)
                                    dr[column.ColumnName] = normalizedValue ?? DBNull.Value;
                            }
                            else
                            {
                                var prop = GetCachedProperty(row.RowData.GetType(), column.ColumnName);
                                if (prop != null && prop.CanWrite)
                                {
                                    var convertedValue = ConvertValue(normalizedValue, prop.PropertyType);
                                    prop.SetValue(row.RowData, convertedValue);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _grid?.ReportOperationError("Data.UpdateCellValue", ex);
                            return;
                        }
                    }
                }

                row.IsDirty = true;
            }

            cell.CellValue = normalizedValue!;
            cell.IsDirty = true;

            if (cell.RowIndex >= 0 && cell.RowIndex < Rows.Count)
                _grid.RefreshViewAfterEdit(Rows[cell.RowIndex].RowData, cell.ColumnIndex);
        }

        private object? NormalizeEditorValue(object rawValue, BeepColumnConfig column, BeepCellConfig cell)
        {
            if (rawValue == null) return null;

            Type? targetType = null;
            if (cell.RowIndex >= 0 && cell.RowIndex < Rows.Count)
            {
                var row = Rows[cell.RowIndex];
                if (row.RowData is DataRowView drv)
                {
                    var table = drv.Row.Table;
                    if (table?.Columns.Contains(column.ColumnName) == true)
                    {
                        targetType = table.Columns[column.ColumnName]!.DataType;
                    }
                }
                else if (row.RowData is DataRow dr)
                {
                    var table = dr.Table;
                    if (table?.Columns.Contains(column.ColumnName) == true)
                    {
                        targetType = table.Columns[column.ColumnName]!.DataType;
                    }
                }
                else if (row.RowData != null && !string.IsNullOrEmpty(column.ColumnName))
                {
                    var prop = row.RowData.GetType().GetProperty(column.ColumnName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    targetType = prop?.PropertyType;
                }
            }

            if (rawValue is SimpleItem si)
            {
                if (targetType == null)
                {
                    return si.Item ?? si.Value ?? si.Text;
                }
                if (targetType == typeof(string))
                    return si.Text ?? si.Value?.ToString() ?? si.Item?.ToString() ?? string.Empty;
                // Each of these is one rung of a deliberate cascade: try to convert, and on
                // failure try the next source or fall back to the type's default. The narrowing
                // matters - catching everything meant an unrelated fault looked like a value that
                // simply would not convert.
                if (targetType.IsEnum)
                {
                    try { return Enum.Parse(targetType, si.Text, true); }
                    catch (Exception ex) when (ex is ArgumentException or FormatException or OverflowException or InvalidCastException)
                    {
                        _grid?.ReportOperationError("Data.ParseEnum", ex);
                    }
                    if (si.Value != null)
                    {
                        try { return Enum.Parse(targetType, si.Value!.ToString()!, true); }
                        catch (Exception ex) when (ex is ArgumentException or FormatException or OverflowException or InvalidCastException)
                        {
                            _grid?.ReportOperationError("Data.ParseEnum", ex);
                        }
                    }
                    return Activator.CreateInstance(targetType);
                }
                if (IsNumericType(targetType))
                {
                    object candidate = si.Value ?? si.Text;
                    try { return Convert.ChangeType(candidate, Nullable.GetUnderlyingType(targetType) ?? targetType); }
                    catch (Exception ex) when (ex is ArgumentException or FormatException or OverflowException or InvalidCastException)
                    {
                        _grid?.ReportOperationError("Data.ParseNumber", ex);
                    }
                    return Activator.CreateInstance(Nullable.GetUnderlyingType(targetType) ?? targetType);
                }
                if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                {
                    if (DateTime.TryParse(si.Value?.ToString() ?? si.Text, out var dt)) return dt;
                    return default(DateTime);
                }
                return si.Item ?? si.Value ?? si.Text;
            }

            if (targetType != null)
            {
                return ConvertValue(rawValue, targetType);
            }
            return rawValue;
        }

        private bool IsNumericType(Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            return t == typeof(byte) || t == typeof(sbyte) || t == typeof(short) || t == typeof(ushort) ||
                   t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong) ||
                   t == typeof(float) || t == typeof(double) || t == typeof(decimal);
        }

        private object? ConvertValue(object? value, Type targetType)
        {
            if (value == null)
            {
                var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
                return underlyingType.IsValueType ? Activator.CreateInstance(underlyingType) : null;
            }

            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;
            try
            {
                return Convert.ChangeType(value, underlying);
            }
            catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
            {
                _grid?.ReportOperationError("Data.ConvertValue", ex);
                return value;
            }
        }
    }
}
