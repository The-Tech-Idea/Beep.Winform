using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Reflection;
 
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal partial class GridDataHelper
    {
        private readonly BeepGridPro _grid;
        public object DataSource { get; private set; }
        public string DataMember { get; private set; }
        public BindingList<BeepRowConfig> Rows { get; } = new();
        public BeepGridColumnConfigCollection Columns { get; }
        private readonly Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler> _rowChangeHandlers = new();

        public GridDataHelper(BeepGridPro grid)
        {
            _grid = grid;
            Columns = new BeepGridColumnConfigCollection();

            // Subscribe to column property changes to refresh the grid
            Columns.ColumnPropertyChanged += Columns_ColumnPropertyChanged;
            Columns.ListChanged += Columns_ListChanged;
        }

        /// <summary>
        /// Handles when any column property changes (e.g., Visible, Width, etc.)
        /// </summary>
        private void Columns_ColumnPropertyChanged(object sender, ColumnPropertyChangedEventArgs e)
        {
            // Determine what kind of refresh is needed based on the property that changed
            switch (e.PropertyName)
            {
                case nameof(BeepColumnConfig.Visible):
                case nameof(BeepColumnConfig.Width):
                case nameof(BeepColumnConfig.MinWidth):
                case nameof(BeepColumnConfig.MaxWidth):
                case nameof(BeepColumnConfig.Index):
                case nameof(BeepColumnConfig.Sticked):
                    // These require layout recalculation
                    _grid.Layout?.Recalculate();
                    _grid.Invalidate();
                    break;

                case nameof(BeepColumnConfig.ColumnCaption):
                case nameof(BeepColumnConfig.ColumnBackColor):
                case nameof(BeepColumnConfig.ColumnForeColor):
                case nameof(BeepColumnConfig.ColumnHeaderBackColor):
                case nameof(BeepColumnConfig.ColumnHeaderForeColor):
                case nameof(BeepColumnConfig.UseCustomColors):
                case nameof(BeepColumnConfig.CellTextAlignment):
                case nameof(BeepColumnConfig.HeaderTextAlignment):
                    // These only require repaint
                    _grid.Invalidate();
                    break;

                case nameof(BeepColumnConfig.ReadOnly):
                case nameof(BeepColumnConfig.CellEditor):
                case nameof(BeepColumnConfig.AllowSort):
                case nameof(BeepColumnConfig.AllowFilter):
                    // These affect behavior but may need repaint for visual indicators
                    _grid.Invalidate();
                    break;

                default:
                    // For any other property, just invalidate to be safe
                    _grid.Invalidate();
                    break;
            }
        }

        /// <summary>
        /// Handles when columns are added, removed, or the list is reset
        /// </summary>
        private void Columns_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                case ListChangedType.ItemDeleted:
                case ListChangedType.Reset:
                    // Column structure changed - need full layout recalculation
                    _grid.Layout?.Recalculate();
                    _grid.Invalidate();
                    break;

                case ListChangedType.ItemMoved:
                    // Column order changed
                    _grid.Layout?.Recalculate();
                    _grid.Invalidate();
                    break;
            }
        }

        /// <summary>
        /// Clears the DataSource reference and unsubscribes from row change handlers.
        /// </summary>
        public void ClearDataSource()
        {
            UnsubscribeRowChangeHandlers();
            DataSource = null;
            DataMember = null;
        }

        public void Bind(object dataSource, bool triggerAutoSize = true)
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

            // Update page info for paging controls
            UpdatePageInfo();

            // Skip auto-sizing in design mode to prevent excessive operations
            if (triggerAutoSize &&
                !System.ComponentModel.LicenseManager.UsageMode.Equals(System.ComponentModel.LicenseUsageMode.Designtime))
            {
                _grid.RequestAutoSize(AutoSizeTriggerSource.DataBind);
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
                    AllowSort = false,
                    AllowAutoSize = false,
                    MinWidth = 24,
                    MaxWidth = 48,
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
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    AllowSort = true,
                    AllowAutoSize = false,
                    MinWidth = 30,
                    MaxWidth = 120,
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
                    AllowSort = false,
                    AllowAutoSize = false,
                    MinWidth = 30,
                    MaxWidth = 120,
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
                // Determine the item type: use entity type if set and meaningful,
                // otherwise try the generic type argument, otherwise reflect on first item
                Type itemType = _grid.EntityType;

                // typeof(object) is the same as "not set" — has no useful properties
                if (itemType == null || itemType == typeof(object))
                {
                    // Try to detect from generic collection type
                    itemType = GetEnumerableItemType(enumerable);
                }

                // If still not useful, try reflection on the first item
                if (itemType == null || itemType == typeof(object))
                {
                    var first = enumerable.Cast<object?>().FirstOrDefault();
                    if (first != null)
                    {
                        itemType = first.GetType();
                        _grid.SetEntityType(itemType);
                    }
                }

                if (itemType != null && itemType != typeof(object))
                {
                    // Update the grid's entity type for future use
                    _grid.SetEntityType(itemType);

                    // Generate columns from the item type's properties
                    foreach (var prop in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (prop.GetIndexParameters().Length > 0) continue;
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

            // Refresh rows to populate them with data
            RefreshRows();

            _grid.RequestAutoSize(AutoSizeTriggerSource.DataBind);
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
                // Try to determine entity type from the enumerable if not already set
                if ((_grid.EntityType == null || _grid.EntityType == typeof(object)) && en.GetType().IsGenericType)
                {
                    var genericArgs = en.GetType().GetGenericArguments();
                    if (genericArgs.Length > 0 && genericArgs[0] != typeof(object))
                    {
                        _grid.SetEntityType(genericArgs[0]);
                    }
                }
                return (en, schema);
            }

            // Single object - set entity type if not already set
            if ((_grid.EntityType == null || _grid.EntityType == typeof(object)) && resolved != null)
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
                if (isFirstItem && (_grid._entityType == null || _grid._entityType == typeof(object)))
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

        private void UpdatePageInfo()
        {
            int totalRecords = Rows.Count;

            int currentPage = 1;
            int totalPages = 1;

            if (totalRecords > 0)
            {
                // Use render helper pagination math (based on visible row capacity + selection).
                totalPages = Math.Max(1, _grid.Render.GetTotalPages(_grid));
                currentPage = Math.Max(1, Math.Min(totalPages, _grid.Render.GetCurrentPage(_grid)));
            }

            _grid.NavigatorPainter.UpdatePageInfo(currentPage, totalPages, totalRecords);
            _grid.NavigatorPainter.EnablePagingControls(totalRecords > 1);
        }
    }
}

