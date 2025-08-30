using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Reflection;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridDataHelper
    {
        private readonly BeepGridPro _grid;
        public object DataSource { get; private set; }
        public string DataMember { get; private set; }
        public BindingList<BeepRowConfig> Rows { get; } = new();
        public BeepGridColumnConfigCollection Columns { get; } = new();

        public GridDataHelper(BeepGridPro grid)
        {
            _grid = grid;
        }

        public void Bind(object dataSource)
        {
            DataSource = dataSource;
            DataMember = _grid.DataMember; // Update DataMember from grid
            
            // Always ensure system columns are present FIRST
            EnsureSystemColumns();
            
            // Only auto-generate columns if none exist beyond system columns, so user-configured columns are preserved
            if (Columns.Count <= GetSystemColumnCount())
            {
                AutoGenerateColumns();
                return; // AutoGenerateColumns calls RefreshRows and auto-sizing
            }

            // Refresh rows for existing columns (but limit in design mode)
            RefreshRows();

            // Skip auto-sizing in design mode to prevent excessive operations
            if (!System.ComponentModel.LicenseManager.UsageMode.Equals(System.ComponentModel.LicenseUsageMode.Designtime) && 
                _grid.AutoSizeColumnsMode != DataGridViewAutoSizeColumnsMode.None)
            {
                _grid.AutoResizeColumnsToFitContent();
            }
        }

        private int GetSystemColumnCount()
        {
            return Columns.Count(c => c.IsSelectionCheckBox || c.IsRowNumColumn || c.IsRowID);
        }

        /// <summary>
        /// Ensures system columns (checkbox, row number, row ID) are present, exactly like BeepSimpleGrid.EnsureDefaultColumns()
        /// </summary>
        public void EnsureSystemColumns()
        {
            // Check if Sel column exists
            if (!Columns.Any(c => c.ColumnName == "Sel"))
            {
                var selColumn = new BeepColumnConfig
                {
                    ColumnCaption = "☑",
                    ColumnName = "Sel",
                    Width = _grid.Layout.CheckBoxColumnWidth > 0 ? _grid.Layout.CheckBoxColumnWidth : 30,
                    Index = 0,
                    Visible = _grid.ShowCheckBox,
                    Sticked = true,
                    IsUnbound = true,
                    IsSelectionCheckBox = true,
                    PropertyTypeName = typeof(bool).AssemblyQualifiedName,
                    CellEditor = BeepColumnType.CheckBoxBool,
                    GuidID = Guid.NewGuid().ToString(),
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    Resizable = DataGridViewTriState.False,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                };
                selColumn.ColumnType = MapPropertyTypeToDbFieldCategory(selColumn.PropertyTypeName);
                Columns.Insert(0, selColumn);
            }

            // Check if RowNum column exists
            if (!Columns.Any(c => c.ColumnName == "RowNum"))
            {
                int index = Columns.Count(c => c.IsSelectionCheckBox);
                var rowNumColumn = new BeepColumnConfig
                {
                    ColumnCaption = "#",
                    ColumnName = "RowNum",
                    Width = 30,
                    Index = index,
                    Visible = true,
                    Sticked = true,
                    ReadOnly = true,
                    IsRowNumColumn = true,
                    IsUnbound = true,
                    PropertyTypeName = typeof(int).AssemblyQualifiedName,
                    CellEditor = BeepColumnType.Text,
                    GuidID = Guid.NewGuid().ToString(),
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    Resizable = DataGridViewTriState.False,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    AggregationType = AggregationType.Count
                };
                rowNumColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowNumColumn.PropertyTypeName);
                Columns.Insert(index, rowNumColumn);
            }

            // Check if RowID column exists
            if (!Columns.Any(c => c.ColumnName == "RowID"))
            {
                int index = Columns.Count(c => c.IsSelectionCheckBox || c.IsRowNumColumn);
                var rowIdColumn = new BeepColumnConfig
                {
                    ColumnCaption = "RowID",
                    ColumnName = "RowID",
                    Width = 30,
                    Index = index,
                    Visible = false,
                    Sticked = true,
                    ReadOnly = true,
                    IsRowID = true,
                    IsUnbound = true,
                    PropertyTypeName = typeof(int).AssemblyQualifiedName,
                    CellEditor = BeepColumnType.Text,
                    GuidID = Guid.NewGuid().ToString(),
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    Resizable = DataGridViewTriState.False,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                };
                rowIdColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowIdColumn.PropertyTypeName);
                Columns.Insert(index, rowIdColumn);
            }

            // Reindex all columns to ensure proper ordering
            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i].Index = i;
            }
        }

        public void AutoGenerateColumns()
        {
            Columns.Clear();

            EnsureSystemColumns();

            var (enumerable, schemaTable) = GetEffectiveEnumerableWithSchema();

            if (schemaTable != null)
            {
                // Try DataTable schema if available
                foreach (System.Data.DataColumn dCol in schemaTable.Columns)
                {
                    if (IsSystemColumn(dCol.ColumnName)) continue;

                    var bCol = new BeepColumnConfig
                    {
                        ColumnName = dCol.ColumnName,
                        ColumnCaption = dCol.Caption ?? dCol.ColumnName,
                        PropertyTypeName = dCol.DataType.AssemblyQualifiedName,
                        Width = 100,
                        ColumnType = MapDbType(dCol.DataType),
                        CellEditor = MapColumnType(dCol.DataType),
                        Visible = true,
                        Resizable = DataGridViewTriState.True,
                        SortMode = DataGridViewColumnSortMode.Automatic,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    };
                    bCol.Index = Columns.Count;
                    Columns.Add(bCol);
                }
            }
            else
            {
                // Use entity type if available, otherwise try reflection on first item
                Type itemType = _grid.EntityType;
                if (itemType != null)
                {
                    // Use the known entity type for better column generation
                    foreach (var prop in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (IsSystemColumn(prop.Name)) continue;

                        var bCol = new BeepColumnConfig
                        {
                            ColumnName = prop.Name,
                            ColumnCaption = prop.Name,
                            PropertyTypeName = prop.PropertyType.AssemblyQualifiedName,
                            Width = 100,
                            ColumnType = MapDbType(prop.PropertyType),
                            CellEditor = MapColumnType(prop.PropertyType),
                            Visible = true,
                            Resizable = DataGridViewTriState.True,
                            SortMode = DataGridViewColumnSortMode.Automatic,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                        };
                        bCol.Index = Columns.Count;
                        Columns.Add(bCol);
                    }
                }
                else
                {
                    // Fallback: Try reflection on first item
                    var first = enumerable.Cast<object?>().FirstOrDefault();
                    if (first != null)
                    {
                        foreach (var prop in first.GetType().GetProperties())
                        {
                            if (IsSystemColumn(prop.Name)) continue;

                            var bCol = new BeepColumnConfig
                            {
                                ColumnName = prop.Name,
                                ColumnCaption = prop.Name,
                                PropertyTypeName = prop.PropertyType.AssemblyQualifiedName,
                                Width = 100,
                                ColumnType = MapDbType(prop.PropertyType),
                                CellEditor = MapColumnType(prop.PropertyType),
                                Visible = true,
                                Resizable = DataGridViewTriState.True,
                                SortMode = DataGridViewColumnSortMode.Automatic,
                                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                            };
                            bCol.Index = Columns.Count;
                            Columns.Add(bCol);
                        }
                    }
                }
            }

            // Refresh rows to populate them with data
            RefreshRows();

            // Apply auto-sizing if enabled
            if (_grid.AutoSizeColumnsMode != DataGridViewAutoSizeColumnsMode.None)
            {
                _grid.AutoResizeColumnsToFitContent();
            }
        }

        private void AddColumnsFromType(Type itemType)
        {
            int startIndex = Columns.Count(c => c.IsSelectionCheckBox || c.IsRowNumColumn || c.IsRowID);
            int index = startIndex;

            foreach (var prop in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.GetIndexParameters().Length > 0) continue;
                
                // Skip if column already exists
                if (Columns.Any(c => c.ColumnName == prop.Name && !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID))
                    continue;

                Columns.Add(new BeepColumnConfig
                {
                    ColumnName = prop.Name,
                    ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(prop.Name),
                    Width = 100,
                    Index = index++,
                    Visible = true,
                    PropertyTypeName = prop.PropertyType.AssemblyQualifiedName,
                    CellEditor = MapPropertyTypeToCellEditor(prop.PropertyType.AssemblyQualifiedName),
                    ColumnType = MapPropertyTypeToDbFieldCategory(prop.PropertyType.AssemblyQualifiedName)
                });
            }
        }

        private static Type GetEnumerableItemType(IEnumerable enumerable)
        {
            if (enumerable == null) return null;
            var type = enumerable.GetType();
            // Try IEnumerable<T>
            var ienum = type.GetInterfaces()
                            .Concat(new[] { type })
                            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return ienum?.GetGenericArguments().FirstOrDefault();
        }

        public void RefreshRows()
        {
            Rows.Clear();
            var (enumerable, schemaTable) = GetEffectiveEnumerableWithSchema();
            var items = enumerable.Cast<object?>().ToList();

            // In design mode, limit to a small number of sample rows to reduce overhead
            int maxRows = System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime ? 
                          Math.Min(5, items.Count) : items.Count;

            for (int i = 0; i < maxRows; i++)
            {
                var r = new BeepRowConfig { RowIndex = i, DisplayIndex = i, Height = _grid.RowHeight, RowData = items[i] };
                int colIndex = 0;
                
                foreach (var col in Columns)
                {
                    object? val = null;

                    // Handle system columns exactly like BeepSimpleGrid
                    if (col.IsSelectionCheckBox)
                    {
                        val = false; // Default unchecked state
                    }
                    else if (col.IsRowNumColumn)
                    {
                        val = i + 1; // 1-based row number
                    }
                    else if (col.IsRowID)
                    {
                        val = i; // 0-based row ID for internal use
                    }
                    else if (items[i] is System.Data.DataRowView drv)
                    {
                        if (drv.DataView?.Table?.Columns.Contains(col.ColumnName) == true)
                        {
                            val = drv.Row[col.ColumnName];
                        }
                    }
                    else if (items[i] is System.Data.DataRow dr)
                    {
                        if (dr.Table?.Columns.Contains(col.ColumnName) == true)
                        {
                            val = dr[col.ColumnName];
                        }
                    }
                    else if (items[i] != null)
                    {
                        val = items[i].GetType().GetProperty(col.ColumnName)?.GetValue(items[i]);
                    }

                    var cell = new BeepCellConfig
                    {
                        RowIndex = i,
                        ColumnIndex = colIndex,
                        DisplayIndex = colIndex,
                        ColumnName = col.ColumnName,
                        CellValue = val,
                        Width = col.Width,
                        Height = _grid.RowHeight
                    };
                    r.Cells.Add(cell);
                    colIndex++;
                }
                Rows.Add(r);
            }

            // Skip auto-sizing in design mode to prevent excessive operations
            if (!System.ComponentModel.LicenseManager.UsageMode.Equals(System.ComponentModel.LicenseUsageMode.Designtime) && 
                _grid.AutoSizeColumnsMode != DataGridViewAutoSizeColumnsMode.None)
            {
                _grid.AutoResizeColumnsToFitContent();
            }
        }

        // Resolve the data source honoring BindingSource and the BeepGridPro.DataMember
        private object ResolveDataForBinding()
        {
            object data = DataSource;
            if (data == null) return null;

            // Unwrap BindingSource
            if (data is BindingSource bs)
            {
                return bs.List ?? bs.DataSource ?? data;
            }

            string dataMember = DataMember ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(dataMember))
            {
                // Handle ADO.NET containers
                if (data is System.Data.DataSet ds)
                {
                    if (ds.Tables.Contains(dataMember))
                        return ds.Tables[dataMember].DefaultView;
                }
                if (data is System.Data.DataViewManager dvm)
                {
                    var ds2 = dvm.DataSet;
                    if (ds2 != null && ds2.Tables.Contains(dataMember))
                        return ds2.Tables[dataMember].DefaultView;
                }

                // Reflect a property on the object
                var prop = data.GetType().GetProperty(dataMember);
                if (prop != null)
                {
                    var memberVal = prop.GetValue(data);
                    if (memberVal != null)
                    {
                        // If it is a BindingSource, unwrap it
                        if (memberVal is BindingSource mbs)
                            return mbs.List ?? mbs.DataSource ?? memberVal;
                        return memberVal;
                    }
                }
            }

            return data;
        }

        private (IEnumerable enumerable, System.Data.DataTable schemaTable) GetEffectiveEnumerableWithSchema()
        {
            var resolved = ResolveDataForBinding();
            System.Data.DataTable schema = null;

            if (resolved == null)
                return (Array.Empty<object>(), schema);

            if (resolved is BindingSource bs)
            {
                resolved = bs.List ?? bs.DataSource;
            }

            if (resolved is System.Data.DataTable dt)
            {
                schema = dt;
                return (dt.DefaultView, schema);
            }

            if (resolved is System.Data.DataView dv)
            {
                schema = dv.Table;
                return (dv, schema);
            }

            if (resolved is IEnumerable en)
            {
                // Try to determine entity type from the enumerable
                if (_grid.EntityType == null && en.GetType().IsGenericType)
                {
                    var genericArgs = en.GetType().GetGenericArguments();
                    if (genericArgs.Length > 0)
                    {
                        _grid.SetEntityType(genericArgs[0]);
                    }
                }
                return (en, schema);
            }

            // Single object - set entity type if not already set
            if (_grid.EntityType == null && resolved != null)
            {
                _grid.SetEntityType(resolved.GetType());
            }
            return (new object[] { resolved }, schema);
        }

        // Helper methods exactly like BeepSimpleGrid
        private DbFieldCategory MapPropertyTypeToDbFieldCategory(string propertyTypeName)
        {
            if (string.IsNullOrWhiteSpace(propertyTypeName))
                return DbFieldCategory.String;

            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
                return DbFieldCategory.String;

            if (type == typeof(string)) return DbFieldCategory.String;
            if (type == typeof(int) || type == typeof(long)) return DbFieldCategory.Numeric;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return DbFieldCategory.Numeric;
            if (type == typeof(DateTime)) return DbFieldCategory.Date;
            if (type == typeof(bool)) return DbFieldCategory.Boolean;
            if (type == typeof(char)) return DbFieldCategory.Char;
            if (type == typeof(Guid)) return DbFieldCategory.Guid;

            return DbFieldCategory.String;
        }

        private BeepColumnType MapPropertyTypeToCellEditor(string propertyTypeName)
        {
            if (string.IsNullOrWhiteSpace(propertyTypeName))
                return BeepColumnType.Text;

            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
                return BeepColumnType.Text;

            if (type == typeof(string)) return BeepColumnType.Text;
            if (type == typeof(int) || type == typeof(long)) return BeepColumnType.NumericUpDown;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return BeepColumnType.Text;
            if (type == typeof(DateTime)) return BeepColumnType.DateTime;
            if (type == typeof(bool)) return BeepColumnType.CheckBoxBool;
            if (type == typeof(char)) return BeepColumnType.CheckBoxChar;
            if (type == typeof(Guid)) return BeepColumnType.Text;

            return BeepColumnType.Text;
        }

        private bool IsSystemColumn(string columnName)
        {
            return columnName == "Sel" || columnName == "RowNum" || columnName == "RowID";
        }

        private static DbFieldCategory MapDbType(Type dataType)
        {
            if (dataType == typeof(string)) return DbFieldCategory.String;
            if (dataType == typeof(int) || dataType == typeof(long)) return DbFieldCategory.Numeric;
            if (dataType == typeof(float) || dataType == typeof(double) || dataType == typeof(decimal)) return DbFieldCategory.Numeric;
            if (dataType == typeof(DateTime)) return DbFieldCategory.Date;
            if (dataType == typeof(bool)) return DbFieldCategory.Boolean;
            if (dataType == typeof(char)) return DbFieldCategory.Char;
            if (dataType == typeof(Guid)) return DbFieldCategory.Guid;

            return DbFieldCategory.String;
        }

        private static BeepColumnType MapColumnType(Type dataType)
        {
            if (dataType == typeof(string)) return BeepColumnType.Text;
            if (dataType == typeof(int) || dataType == typeof(long)) return BeepColumnType.NumericUpDown;
            if (dataType == typeof(float) || dataType == typeof(double) || dataType == typeof(decimal)) return BeepColumnType.Text;
            if (dataType == typeof(DateTime)) return BeepColumnType.DateTime;
            if (dataType == typeof(bool)) return BeepColumnType.CheckBoxBool;
            if (dataType == typeof(char)) return BeepColumnType.CheckBoxChar;
            if (dataType == typeof(Guid)) return BeepColumnType.Text;

            return BeepColumnType.Text;
        }

        /// <summary>
        /// Updates a cell value and the underlying data source
        /// </summary>
        public void UpdateCellValue(BeepCellConfig cell, object newValue)
        {
            if (cell == null) return;

            var column = Columns[cell.ColumnIndex];
            if (column.ReadOnly || !cell.IsEditable) return;

            // Normalize the value first (handle SimpleItem objects from editors)
            var normalizedValue = NormalizeEditorValue(newValue, column, cell);

            // Update the cell
            cell.CellValue = normalizedValue;
            cell.IsDirty = true;

            // Update the underlying data source
            if (cell.RowIndex >= 0 && cell.RowIndex < Rows.Count)
            {
                var row = Rows[cell.RowIndex];
                row.IsDirty = true;

                // Update the data object if available
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
                        // Log error but don't throw to maintain grid stability
                        System.Diagnostics.Debug.WriteLine($"Error updating cell value: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Normalizes editor values, especially handling SimpleItem objects from ComboBox/ListBox editors
        /// </summary>
        private object NormalizeEditorValue(object rawValue, BeepColumnConfig column, BeepCellConfig cell)
        {
            if (rawValue == null) return null;

            // Get target type from the data source
            Type targetType = null;
            if (cell.RowIndex >= 0 && cell.RowIndex < Rows.Count)
            {
                var row = Rows[cell.RowIndex];
                if (row.RowData is DataRowView drv)
                {
                    if (drv.DataView?.Table?.Columns.Contains(column.ColumnName) == true)
                    {
                        targetType = drv.Row.Table.Columns[column.ColumnName].DataType;
                    }
                }
                else if (row.RowData is DataRow dr)
                {
                    if (dr.Table?.Columns.Contains(column.ColumnName) == true)
                    {
                        targetType = dr.Table.Columns[column.ColumnName].DataType;
                    }
                }
                else if (row.RowData != null && !string.IsNullOrEmpty(column.ColumnName))
                {
                    var prop = row.RowData.GetType().GetProperty(column.ColumnName, 
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    targetType = prop?.PropertyType;
                }
            }

            // Handle SimpleItem objects from ComboBox/ListBox editors
            if (rawValue is SimpleItem si)
            {
                if (targetType == null)
                {
                    return si.Item ?? si.Value ?? si.Text;
                }
                if (targetType == typeof(string))
                    return si.Text ?? si.Value?.ToString() ?? si.Item?.ToString();
                if (targetType.IsEnum)
                {
                    try { return Enum.Parse(targetType, si.Text, true); } catch { }
                    if (si.Value != null)
                    {
                        try { return Enum.Parse(targetType, si.Value.ToString(), true); } catch { }
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

            // For non-SimpleItem values, return as-is or convert if target type is known
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

        private object ConvertValue(object value, Type targetType)
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

        // Data synchronization methods (moved from BeepGridPro for better encapsulation)
        internal void SyncFullDataFromBindingSource()
        {
            _grid._fullData = new List<object>();
            var bindingSource = _grid.Navigator.GetBindingSource();
            if (bindingSource == null) return;

            var enumerator = bindingSource.GetEnumerator();
            int i = 0;
            bool isFirstItem = true;

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item == null) continue;

                // Set _entityType based on the first valid item
                if (isFirstItem && _grid._entityType == null)
                {
                    _grid._entityType = item.GetType();
                    isFirstItem = false;
                }

                _grid._fullData.Add(item);
                i++;
            }

            if (_grid._entityType == null)
            {
                // No valid items found to set _entityType
            }
        }

        internal void InitializeData()
        {
            var bindingSource = _grid.Navigator.GetBindingSource();
            if (bindingSource != null)
            {
                SyncFullDataFromBindingSource(); // Initial sync from BindingSource
            }
            else
            {
                // Fallback
                _grid._fullData = new List<object>();
            }
            _grid._dataOffset = 0;
        }
    }
}
