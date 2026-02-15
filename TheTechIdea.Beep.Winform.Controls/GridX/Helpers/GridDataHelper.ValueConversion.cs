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

            cell.CellValue = normalizedValue!;
            cell.IsDirty = true;

            if (cell.RowIndex >= 0 && cell.RowIndex < Rows.Count)
            {
                var row = Rows[cell.RowIndex];
                row.IsDirty = true;

                if (row.RowData != null && !string.IsNullOrEmpty(column.ColumnName))
                {
                    try
                    {
                        if (row.RowData is DataRowView drv)
                        {
                            if (drv.DataView?.Table?.Columns.Contains(column.ColumnName) == true)
                            {
                                drv.Row[column.ColumnName] = normalizedValue ?? DBNull.Value;
                            }
                        }
                        else if (row.RowData is DataRow dr)
                        {
                            if (dr.Table?.Columns.Contains(column.ColumnName) == true)
                            {
                                dr[column.ColumnName] = normalizedValue ?? DBNull.Value;
                            }
                        }
                        else
                        {
                            var prop = row.RowData.GetType().GetProperty(column.ColumnName);
                            if (prop != null && prop.CanWrite)
                            {
                                var convertedValue = ConvertValue(normalizedValue, prop.PropertyType);
                                prop.SetValue(row.RowData, convertedValue);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"Error updating cell value: {ex.Message}");
#endif
                    }
                }
            }
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
                if (targetType.IsEnum)
                {
                    try { return Enum.Parse(targetType, si.Text, true); } catch { }
                    if (si.Value != null)
                    {
                        try { return Enum.Parse(targetType, si.Value!.ToString()!, true); } catch { }
                    }
                    return Activator.CreateInstance(targetType);
                }
                if (IsNumericType(targetType))
                {
                    object candidate = si.Value ?? si.Text;
                    try { return Convert.ChangeType(candidate, Nullable.GetUnderlyingType(targetType) ?? targetType); } catch { }
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
            catch
            {
                return value;
            }
        }
    }
}
