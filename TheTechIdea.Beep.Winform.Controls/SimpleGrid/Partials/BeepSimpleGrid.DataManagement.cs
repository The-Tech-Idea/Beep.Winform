using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing data management functionality for BeepSimpleGrid
    /// Handles DataSource setup, BindingSource operations, and CRUD operations
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region DataSource Setup

        private void DataSetup()
        {
            try
            {
                EntityStructure entity = null;
                object resolvedData = null;

                if (_bindingSource == null)
                {
                    _bindingSource = new BindingSource();
                }

                if (_dataSource == null)
                {
                    finalData = _bindingSource;
                }
                else if (_dataSource is BindingSource bindingSrc)
                {
                    AssignBindingSource(bindingSrc);
                    var ret = SetupBindingSource();
                    resolvedData = ret.Item1;
                    entity = ret.Item2;
                    finalData = _bindingSource;
                }
                else if (_dataSource is Type typeDataSource)
                {
                    _entityType = typeDataSource;
                    _bindingSource.DataSource = _dataSource;
                    entity = EntityHelper.GetEntityStructureFromType(typeDataSource);
                    finalData = _bindingSource;
                }
                else
                {
                    _bindingSource.DataSource = _dataSource;
                    resolvedData = _dataSource;
                    finalData = _bindingSource;

                    if (_entityType == null && _dataSource != null)
                    {
                        Type dataSourceType = _dataSource.GetType();

                        if (dataSourceType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(dataSourceType))
                        {
                            Type[] genericArgs = dataSourceType.GetGenericArguments();
                            if (genericArgs.Length > 0)
                            {
                                _entityType = genericArgs[0];
                            }
                        }
                        else if (dataSourceType.IsArray)
                        {
                            _entityType = dataSourceType.GetElementType();
                        }
                    }
                }

                if (entity == null && _entityType != null)
                {
                    entity = EntityHelper.GetEntityStructureFromType(_entityType);
                }
                else if (entity == null && resolvedData != null)
                {
                    entity = EntityHelper.GetEntityStructureFromListorTable(resolvedData);
                }

                if (entity != null)
                {
                    Entity = entity;
                    if (!_columns.Any() || !columnssetupusingeditordontchange)
                    {
                        CreateColumnsForEntity();
                    }
                }
                else if (_entityType != null)
                {
                    CreateColumnsFromType(_entityType);
                }
                else
                {
                    EnsureDefaultColumns();
                }
            }
            catch (Exception ex)
            {
                EnsureDefaultColumns();
            }
        }

        private Tuple<object, EntityStructure> SetupBindingSource()
        {
            object resolvedData = null;
            EntityStructure entity = null;

            try
            {
                if (_bindingSource == null)
                {
                    return Tuple.Create(resolvedData, entity);
                }

                object dataSourceInstance = _bindingSource.DataSource;
                Type dataSourceType = dataSourceInstance as Type ?? dataSourceInstance?.GetType();
                Type itemType = null;

                if (dataSourceType == null)
                {
                    return Tuple.Create(resolvedData, entity);
                }

                if (!string.IsNullOrEmpty(_bindingSource.DataMember))
                {
                    PropertyInfo memberProp = dataSourceType.GetProperty(
                        _bindingSource.DataMember,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    if (memberProp != null)
                    {
                        Type memberType = memberProp.PropertyType;

                        if (typeof(IEnumerable).IsAssignableFrom(memberType) && memberType != typeof(string))
                        {
                            itemType = GetItemTypeFromDataMember(memberType);
                        }
                        else
                        {
                            itemType = memberType;
                        }

                        if (!(dataSourceInstance is Type))
                        {
                            resolvedData = memberProp.GetValue(dataSourceInstance);
                        }
                    }
                }
                else
                {
                    if (typeof(IEnumerable).IsAssignableFrom(dataSourceType) && dataSourceType != typeof(string))
                    {
                        itemType = GetItemTypeFromDataMember(dataSourceType);

                        if (!(dataSourceInstance is Type))
                        {
                            resolvedData = _bindingSource.List ?? dataSourceInstance;
                        }
                    }
                    else
                    {
                        itemType = dataSourceType;
                        resolvedData = dataSourceInstance;
                    }
                }

                if (itemType != null)
                {
                    _entityType = itemType;
                    entity = EntityHelper.GetEntityStructureFromType(itemType);
                }
                else if (resolvedData != null)
                {
                    entity = EntityHelper.GetEntityStructureFromListorTable(resolvedData);
                    if (entity != null && !string.IsNullOrEmpty(entity.EntityName))
                    {
                        _entityType = Type.GetType(entity.EntityName);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return Tuple.Create(resolvedData, entity);
        }

        private Type GetItemTypeFromDataMember(Type propertyType)
        {
            if (propertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                Type[] genericArgs = propertyType.GetGenericArguments();
                if (genericArgs.Length > 0)
                {
                    return genericArgs[0];
                }
            }

            if (propertyType == typeof(DataTable))
            {
                return typeof(DataRow);
            }

            if (propertyType.IsArray)
            {
                return propertyType.GetElementType();
            }

            return null;
        }

        #endregion

        #region Data Initialization

        private void InitializeData()
        {
            if (_bindingSource != null)
            {
                SyncFullDataFromBindingSource();
            }
            else
            {
                _fullData = finalData is IEnumerable<object> enumerable ? enumerable.ToList() : new List<object>();
                WrapFullData();
            }

            _dataOffset = 0;

            if (originalList == null)
            {
                originalList = new List<object>();
            }
            originalList.Clear();
            originalList.AddRange(_fullData);

            if (_fullData != null && _fullData.Count > 0 && _entityType == null)
            {
                var firstItem = _fullData.First() as DataRowWrapper;
                if (firstItem != null)
                {
                    _entityType = firstItem.OriginalData.GetType();
                }
            }

            InitializeColumnsAndTracking();
            InitializeRows();
            UpdateScrollBars();

            filterColumnComboBox.ListItems.Clear();
            filterColumnComboBox.ListItems.Add(new SimpleItem { Text = "All Columns", Value = null });
            foreach (var col in Columns)
            {
                if (col.Visible && !col.IsSelectionCheckBox & !col.IsRowNumColumn)
                    filterColumnComboBox.ListItems.Add(new SimpleItem { DisplayField = col.ColumnCaption ?? col.ColumnName, Text = col.ColumnName ?? col.ColumnCaption, Value = col.ColumnName });
            }
            if (DataNavigator != null) DataNavigator.DataSource = _fullData;
        }

        private void WrapFullData()
        {
            var wrappedData = new List<object>();
            for (int i = 0; i < _fullData.Count; i++)
            {
                var wrapper = new DataRowWrapper(_fullData[i], i)
                {
                    TrackingUniqueId = Guid.NewGuid(),
                    RowState = DataRowState.Unchanged,
                    DateTimeChange = DateTime.Now
                };
                wrappedData.Add(wrapper);
            }
            _fullData = wrappedData;
        }

        private void SyncFullDataFromBindingSource()
        {
            _fullData = new List<object>();
            var enumerator = _bindingSource.GetEnumerator();
            int i = 0;
            bool isFirstItem = true;

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item == null) continue;

                if (isFirstItem && _entityType == null)
                {
                    _entityType = item.GetType();
                    isFirstItem = false;
                }

                var wrapper = new DataRowWrapper(item, i++)
                {
                    TrackingUniqueId = Guid.NewGuid(),
                    RowState = DataRowState.Unchanged,
                    DateTimeChange = DateTime.Now
                };
                _fullData.Add(wrapper);
            }

            if (_entityType == null)
            {
            }
        }

        private void InitializeColumnsAndTracking()
        {
            if (_fullData == null || !_fullData.Any())
                return;

            if (_columns == null)
                _columns = new List<BeepColumnConfig>();

            if (_fullData?.Any() == true)
            {
                var firstItem = _fullData.First() as DataRowWrapper;
                if (firstItem?.OriginalData != null)
                {
                    _entityType = firstItem.OriginalData.GetType();
                    var properties = _entityType.GetProperties();

                    int currentIndex = 0;

                    if (!_columns.Any(c => c.ColumnName == "Sel"))
                    {
                        var selColumn = new BeepColumnConfig
                        {
                            ColumnCaption = "?",
                            ColumnName = "Sel",
                            Width = _selectionColumnWidth,
                            Index = currentIndex++,
                            Visible = ShowCheckboxes,
                            Sticked = true,
                            IsUnbound = true,
                            IsSelectionCheckBox = true,
                            PropertyTypeName = typeof(bool).AssemblyQualifiedName,
                            CellEditor = BeepColumnType.CheckBoxBool,
                            GuidID = Guid.NewGuid().ToString(),
                            SortMode = DataGridViewColumnSortMode.NotSortable,
                            Resizable = DataGridViewTriState.False,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                            ColumnType = MapPropertyTypeToDbFieldCategory(typeof(bool).AssemblyQualifiedName)
                        };
                        _columns.Add(selColumn);
                    }

                    if (!_columns.Any(c => c.ColumnName == "RowNum"))
                    {
                        var rowNumColumn = new BeepColumnConfig
                        {
                            ColumnCaption = "#",
                            ColumnName = "RowNum",
                            Width = 30,
                            Index = currentIndex++,
                            Visible = ShowRowNumbers,
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
                            AggregationType = AggregationType.Count,
                            ColumnType = MapPropertyTypeToDbFieldCategory(typeof(int).AssemblyQualifiedName)
                        };
                        _columns.Add(rowNumColumn);
                    }

                    if (!_columns.Any(c => c.ColumnName == "RowID"))
                    {
                        var rowIdColumn = new BeepColumnConfig
                        {
                            ColumnCaption = "RowID",
                            ColumnName = "RowID",
                            Width = 30,
                            Index = currentIndex++,
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
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                            ColumnType = MapPropertyTypeToDbFieldCategory(typeof(int).AssemblyQualifiedName)
                        };
                        _columns.Add(rowIdColumn);
                    }

                    foreach (var prop in properties)
                    {
                        if (!_columns.Any(c => c.ColumnName == prop.Name))
                        {
                            string propertyTypeName = prop.PropertyType.AssemblyQualifiedName;
                            var columnConfig = new BeepColumnConfig
                            {
                                ColumnCaption = prop.Name,
                                ColumnName = prop.Name,
                                Width = 100,
                                Index = currentIndex++,
                                Visible = true,
                                PropertyTypeName = propertyTypeName,
                                GuidID = Guid.NewGuid().ToString(),
                                CellEditor = MapPropertyTypeToCellEditor(propertyTypeName),
                                ColumnType = MapPropertyTypeToDbFieldCategory(propertyTypeName)
                            };
                            _columns.Add(columnConfig);
                        }
                    }

                    for (int i = 0; i < _columns.Count; i++)
                    {
                        _columns[i].Index = i;
                    }
                }
            }

            originalList.Clear();
            originalList.AddRange(_fullData);
            Trackings.Clear();
            for (int i = 0; i < _fullData.Count; i++)
            {
                Trackings.Add(new Tracking(Guid.NewGuid(), i, i) { EntityState = EntityState.Unchanged });
            }

            filterColumnComboBox.Items.Clear();
            filterColumnComboBox.Items.Add(new SimpleItem { Text = "All Columns", Value = null });
            foreach (var col in _columns)
            {
                if (col.Visible)
                    filterColumnComboBox.Items.Add(new SimpleItem { Text = col.ColumnCaption ?? col.ColumnName, Value = col.ColumnName });
            }
            filterColumnComboBox.SelectedIndex = 0;
        }

        #endregion

        #region CRUD Operations - IList

        private void CreateNewRecordForIList()
        {
            try
            {
                if (Entity == null)
                {
                    return;
                }

                var newItem = Activator.CreateInstance(_entityType);

                if (_dataSource is IList list)
                {
                    list.Add(newItem);
                }
                else
                {
                    return;
                }

                var wrapper = new DataRowWrapper(newItem, _fullData.Count)
                {
                    TrackingUniqueId = Guid.NewGuid(),
                    RowState = DataRowState.Added,
                    DateTimeChange = DateTime.Now
                };
                _fullData.Add(wrapper);

                Tracking newTracking = new Tracking(Guid.NewGuid(), originalList.Count, _fullData.Count - 1)
                {
                    EntityState = EntityState.Added
                };
                originalList.Add(wrapper);
                Trackings.Add(newTracking);

                int newOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
                StartSmoothScroll(newOffset);
                FillVisibleRows();

                int newRowIndex = _fullData.Count - 1 - _dataOffset;
                if (newRowIndex >= 0 && newRowIndex < Rows.Count)
                {
                    SelectCell(newRowIndex, 0);
                    BeginEdit();
                }

                if (IsLogging)
                {
                    UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                    {
                        TrackingRecord = newTracking,
                        LogAction = LogAction.Insert,
                        UpdatedFields = new Dictionary<string, object>()
                    };
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateIListDataSource(int dataIndex, object updatedItem)
        {
            if (_dataSource is IList list && dataIndex >= 0 && dataIndex < list.Count)
            {
                list[dataIndex] = updatedItem;
            }
        }

        #endregion

        #region CRUD Operations - BindingSource

        private void CreateNewRecordForBindingSource()
        {
            try
            {
                if (Entity == null)
                {
                    MessageBox.Show("Entity structure not defined.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_entityType == null)
                {
                    MessageBox.Show($"Invalid entity type: {Entity.EntityName}", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_bindingSource == null)
                {
                    MessageBox.Show("BindingSource is not initialized.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var newItem = Activator.CreateInstance(_entityType);
                _isAddingNewRecord = true;
                _bindingSource.Add(newItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding new record: {ex.Message}", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _isAddingNewRecord = false;
            }
        }

        private void UpdateBindingSourceDataSource(int dataIndex, object updatedItem)
        {
            try
            {
                if (_bindingSource == null)
                {
                    return;
                }

                if (dataIndex >= 0 && dataIndex < _bindingSource.Count)
                {
                    _bindingSource[dataIndex] = updatedItem;
                    _bindingSource.ResetItem(dataIndex);
                }
                else
                {
                    if (dataIndex >= 0 && dataIndex < _fullData.Count)
                    {
                        var wrapper = _fullData[dataIndex] as DataRowWrapper;
                        if (wrapper != null)
                        {
                            object originalItem = wrapper.OriginalData;
                            int bsIndex = _bindingSource.IndexOf(originalItem);
                            if (bsIndex >= 0)
                            {
                                _bindingSource[bsIndex] = updatedItem;
                                _bindingSource.ResetItem(bsIndex);
                            }
                            else
                            {
                                wrapper.OriginalData = updatedItem;
                                wrapper.RowState = DataRowState.Modified;
                                wrapper.DateTimeChange = DateTime.Now;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region CRUD Operations - Common

        private void InsertIntoDataSource(object newData)
        {
            if (_dataSource is BindingSource)
            {
                if (_bindingSource != null)
                {
                    _bindingSource.Add(newData);
                }
            }
            else if (_dataSource is IList list)
            {
                list.Add(newData);
            }
        }

        private void UpdateInDataSource(object originalData, Dictionary<string, object> changes)
        {
            if (_dataSource is BindingSource)
            {
                if (_bindingSource != null)
                {
                    int dataIndex = _bindingSource.IndexOf(originalData);
                    if (dataIndex >= 0 && dataIndex < _bindingSource.Count)
                    {
                        _bindingSource[dataIndex] = originalData;
                        _bindingSource.ResetItem(dataIndex);
                    }
                    else
                    {
                        DataSetup();
                        InitializeData();
                    }
                }
            }
            else if (_dataSource is IList list)
            {
                int dataIndex = list.IndexOf(originalData);
                if (dataIndex >= 0 && dataIndex < list.Count)
                {
                    list[dataIndex] = originalData;
                }
            }
        }

        private void DeleteFromDataSource(object originalData)
        {
            try
            {
                if (originalData == null)
                {
                    return;
                }

                DataRowWrapper wrapper = _fullData.OfType<DataRowWrapper>().FirstOrDefault(w => w.OriginalData == originalData);
                int dataIndex = wrapper != null ? _fullData.IndexOf(wrapper) : -1;
                if (wrapper == null || dataIndex < 0)
                {
                    return;
                }

                Tracking tracking = GetTrackingItem(wrapper);
                if (tracking != null)
                {
                    tracking.EntityState = EntityState.Deleted;
                }

                if (_dataSource is BindingSource && _bindingSource != null)
                {
                    int bsIndex = _bindingSource.IndexOf(originalData);
                    if (bsIndex >= 0 && bsIndex < _bindingSource.Count)
                    {
                        _bindingSource.RemoveAt(bsIndex);
                    }
                }
                else if (_dataSource is IList list)
                {
                    int listIndex = list.IndexOf(originalData);
                    if (listIndex >= 0 && listIndex < list.Count)
                    {
                        list.RemoveAt(listIndex);
                        _fullData.RemoveAt(dataIndex);
                        for (int i = 0; i < _fullData.Count; i++)
                        {
                            if (_fullData[i] is DataRowWrapper dataRow) dataRow.RowID = i;
                        }
                        originalList.RemoveAt(dataIndex);
                        if (tracking != null) Trackings.Remove(tracking);
                        deletedList.Add(wrapper);

                        if (IsLogging)
                        {
                            UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                            {
                                TrackingRecord = tracking,
                                LogAction = LogAction.Delete,
                                UpdatedFields = new Dictionary<string, object>()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateDataRecordFromRow(BeepCellConfig editingCell)
        {
            if (Rows == null || editingCell == null || editingCell.RowIndex < 0 || editingCell.RowIndex >= Rows.Count || _fullData == null || !_fullData.Any())
                return;

            BeepRowConfig row = Rows[editingCell.RowIndex];

            BeepColumnConfig rowIdCol = Columns.Find(p => p.IsRowID);
            var rowIdCell = row.Cells.FirstOrDefault(c => Columns[c.ColumnIndex].IsRowID);
            if (rowIdCell == null || rowIdCell.CellValue == null || !int.TryParse(rowIdCell.CellValue.ToString(), out int rowID))
            {
                return;
            }

            DataRowWrapper dataItem = null;
            int dataIndex = -1;
            for (int i = 0; i < _fullData.Count; i++)
            {
                var wrapper = _fullData[i] as DataRowWrapper;
                if (wrapper != null && wrapper.RowID == rowID)
                {
                    dataItem = wrapper;
                    dataIndex = i;
                    break;
                }
            }

            if (dataItem == null || dataIndex < 0)
            {
                return;
            }

            object originalData = dataItem.OriginalData;
            bool hasChanges = false;
            int bsIndex = -1;

            if (_bindingSource != null)
            {
                bsIndex = _bindingSource.IndexOf(originalData);
            }

            foreach (var cell in row.Cells)
            {
                if (cell.IsDirty)
                {
                    var col = Columns[cell.ColumnIndex];
                    if (col.IsSelectionCheckBox || col.IsRowNumColumn || col.IsRowID)
                        continue;

                    var prop = originalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                    if (prop != null)
                    {
                        object convertedValue = MiscFunctions.ConvertValueToPropertyType(prop.PropertyType, cell.CellValue);
                        object originalValue = prop.GetValue(originalData);
                        if (!Equals(convertedValue, originalValue))
                        {
                            prop.SetValue(originalData, convertedValue);
                            hasChanges = true;
                        }
                    }
                    row.IsDirty = true;
                }
            }

            if (hasChanges && _bindingSource != null)
            {
                if (bsIndex >= 0 && bsIndex < _bindingSource.Count)
                {
                    _bindingSource[bsIndex] = originalData;
                    _bindingSource.ResetItem(bsIndex);
                }
                else
                {
                    for (int i = 0; i < _bindingSource.Count; i++)
                    {
                        var bsItem = _bindingSource[i];
                        var wrapper = _fullData.FirstOrDefault(w => w is DataRowWrapper dw && dw.OriginalData == bsItem) as DataRowWrapper;
                        if (wrapper != null && wrapper.RowID == rowID)
                        {
                            _bindingSource[i] = originalData;
                            _bindingSource.ResetItem(i);
                            break;
                        }
                    }
                }

                dataItem.OriginalData = originalData;
                dataItem.RowState = DataRowState.Modified;
                dataItem.DateTimeChange = DateTime.Now;
            }
        }

        private void SaveToDataSource()
        {
            try
            {
                if (_fullData == null || originalList == null || DataSource == null)
                {
                    return;
                }

                if (DataSource is BindingSource bindingSource)
                {
                    foreach (var tracking in Trackings.Where(t => t.EntityState == EntityState.Added || t.EntityState == EntityState.Modified))
                    {
                        var wrappedItem = _fullData[tracking.CurrentIndex] as DataRowWrapper;
                        if (wrappedItem != null)
                        {
                            switch (wrappedItem.RowState)
                            {
                                case DataRowState.Added:
                                    if (tracking.EntityState == EntityState.Added)
                                    {
                                        bindingSource.Add(wrappedItem.OriginalData);
                                        originalList.Add(wrappedItem);
                                        tracking.EntityState = EntityState.Unchanged;
                                        wrappedItem.RowState = DataRowState.Unchanged;
                                    }
                                    break;

                                case DataRowState.Modified:
                                    if (tracking.EntityState == EntityState.Modified && ChangedValues.ContainsKey(wrappedItem))
                                    {
                                        tracking.EntityState = EntityState.Unchanged;
                                        wrappedItem.RowState = DataRowState.Unchanged;
                                        ChangedValues.Remove(wrappedItem);
                                    }
                                    break;
                            }
                        }
                    }

                    for (int i = 0; i < deletedList.Count; i++)
                    {
                        var wrappedItem = deletedList[i] as DataRowWrapper;
                        if (wrappedItem != null && wrappedItem.RowState == DataRowState.Deleted)
                        {
                            Tracking tracking = GetTrackingItem(wrappedItem);
                            if (tracking != null && tracking.EntityState == EntityState.Deleted)
                            {
                                int sourceIndex = bindingSource.IndexOf(wrappedItem.OriginalData);
                                if (sourceIndex >= 0)
                                {
                                    bindingSource.RemoveAt(sourceIndex);
                                }
                                originalList.Remove(wrappedItem);
                                Trackings.Remove(tracking);
                                deletedList.RemoveAt(i);
                                i--;
                            }
                        }
                    }

                    bindingSource.ResetBindings(false);
                }
                else if (DataSource is IList list)
                {
                    IList dataSourceList = list;

                    foreach (var tracking in Trackings.Where(t => t.EntityState == EntityState.Added || t.EntityState == EntityState.Modified))
                    {
                        var wrappedItem = _fullData[tracking.CurrentIndex] as DataRowWrapper;
                        if (wrappedItem != null)
                        {
                            switch (wrappedItem.RowState)
                            {
                                case DataRowState.Added:
                                    if (tracking.EntityState == EntityState.Added)
                                    {
                                        dataSourceList.Add(wrappedItem.OriginalData);
                                        originalList.Add(wrappedItem);
                                        tracking.EntityState = EntityState.Unchanged;
                                        wrappedItem.RowState = DataRowState.Unchanged;
                                    }
                                    break;

                                case DataRowState.Modified:
                                    if (tracking.EntityState == EntityState.Modified && ChangedValues.ContainsKey(wrappedItem))
                                    {
                                        tracking.EntityState = EntityState.Unchanged;
                                        wrappedItem.RowState = DataRowState.Unchanged;
                                        ChangedValues.Remove(wrappedItem);
                                    }
                                    break;
                            }
                        }
                    }

                    for (int i = 0; i < deletedList.Count; i++)
                    {
                        var wrappedItem = deletedList[i] as DataRowWrapper;
                        if (wrappedItem != null && wrappedItem.RowState == DataRowState.Deleted)
                        {
                            Tracking tracking = GetTrackingItem(wrappedItem);
                            if (tracking != null && tracking.EntityState == EntityState.Deleted)
                            {
                                int sourceIndex = -1;
                                for (int j = 0; j < dataSourceList.Count; j++)
                                {
                                    if (ReferenceEquals(dataSourceList[j], wrappedItem.OriginalData))
                                    {
                                        sourceIndex = j;
                                        break;
                                    }
                                }
                                if (sourceIndex >= 0)
                                {
                                    dataSourceList.RemoveAt(sourceIndex);
                                }
                                originalList.Remove(wrappedItem);
                                Trackings.Remove(tracking);
                                deletedList.RemoveAt(i);
                                i--;
                            }
                        }
                    }

                    FillVisibleRows();
                }

                UpdateScrollBars();
                Invalidate();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region BindingSource Management

        private void AssignBindingSource(BindingSource bindingSrc)
        {
            if (_bindingSource != null)
            {
                _bindingSource.ListChanged -= BindingSource_ListChanged;
            }

            _bindingSource = bindingSrc;

            if (_dataSource is IList list && !(_dataSource is BindingSource))
            {
                _bindingSource = new BindingSource { DataSource = list };
            }

            if (_bindingSource != null)
            {
                _bindingSource.ListChanged += BindingSource_ListChanged;
            }
        }

        private void BindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    HandleItemAdded(e.NewIndex);
                    if (_isAddingNewRecord && e.NewIndex >= 0 && e.NewIndex < _fullData.Count)
                    {
                        int newOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
                        StartSmoothScroll(newOffset);
                        int newRowIndex = _fullData.Count - 1 - _dataOffset;
                        if (newRowIndex >= 0 && newRowIndex < Rows.Count)
                        {
                            SelectCell(newRowIndex, 0);
                            BeginEdit();
                        }
                        _isAddingNewRecord = false;

                        MessageBox.Show("New record added successfully.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;

                case ListChangedType.ItemChanged:
                    HandleItemChanged(e.NewIndex);
                    break;

                case ListChangedType.ItemDeleted:
                    HandleItemDeleted(e.NewIndex);
                    break;

                case ListChangedType.Reset:
                    DataSetup();
                    InitializeData();
                    break;

                case ListChangedType.ItemMoved:
                    HandleItemMoved(e.OldIndex, e.NewIndex);
                    break;

                default:
                    DataSetup();
                    InitializeData();
                    break;
            }

            UpdateTrackingIndices();
            SyncSelectedRowIndexAndEditor();

            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }

        private void HandleItemAdded(int newIndex)
        {
            try
            {
                if (newIndex < 0 || _bindingSource == null || newIndex >= _bindingSource.Count)
                {
                    return;
                }

                var newItem = _bindingSource[newIndex];
                var wrapper = new DataRowWrapper(newItem, newIndex)
                {
                    TrackingUniqueId = Guid.NewGuid(),
                    RowState = DataRowState.Added,
                    DateTimeChange = DateTime.Now
                };

                if (newIndex == _fullData.Count)
                {
                    _fullData.Add(wrapper);
                }
                else
                {
                    _fullData.Insert(newIndex, wrapper);
                }

                for (int i = 0; i < _fullData.Count; i++)
                {
                    if (_fullData[i] is DataRowWrapper dataRow)
                    {
                        dataRow.RowID = i;
                    }
                }

                if (!originalList.Contains(wrapper))
                {
                    originalList.Add(wrapper);
                }

                EnsureTrackingForItem(wrapper);

                if (_dataOffset > newIndex)
                {
                    _dataOffset++;
                }

                if (IsLogging)
                {
                    var tracking = Trackings.FirstOrDefault(t => t.UniqueId == wrapper.TrackingUniqueId);
                    UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                    {
                        TrackingRecord = tracking,
                        LogAction = LogAction.Insert,
                        UpdatedFields = new Dictionary<string, object>()
                    };
                }

                UpdateRowCount();
            }
            catch (Exception ex)
            {
            }
        }

        private void HandleItemChanged(int index)
        {
            try
            {
                if (index < 0 || index >= _fullData.Count || _bindingSource == null || index >= _bindingSource.Count)
                {
                    return;
                }

                var updatedItem = _bindingSource[index];
                var wrapper = _fullData[index] as DataRowWrapper;
                if (wrapper != null)
                {
                    var oldData = wrapper.OriginalData;
                    var changes = GetChangedFields(oldData, updatedItem);
                    if (changes.Any())
                    {
                        wrapper.OriginalData = updatedItem;
                        wrapper.RowState = DataRowState.Modified;
                        wrapper.DateTimeChange = DateTime.Now;

                        EnsureTrackingForItem(wrapper);

                        if (IsLogging)
                        {
                            Tracking tracking = GetTrackingItem(wrapper);
                            if (tracking != null)
                            {
                                tracking.EntityState = MapRowStateToEntityState(wrapper.RowState);
                                if (!ChangedValues.ContainsKey(wrapper))
                                {
                                    ChangedValues[wrapper] = changes;
                                }
                                else
                                {
                                    foreach (var kvp in changes)
                                    {
                                        ChangedValues[wrapper][kvp.Key] = kvp.Value;
                                    }
                                }
                                UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                                {
                                    TrackingRecord = tracking,
                                    LogAction = LogAction.Update,
                                    UpdatedFields = ChangedValues[wrapper]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void HandleItemDeleted(int index)
        {
            try
            {
                if (index < 0 || index >= _fullData.Count || _bindingSource == null || index >= _bindingSource.Count)
                {
                    return;
                }

                var wrapper = _fullData[index] as DataRowWrapper;
                if (wrapper != null)
                {
                    wrapper.RowState = DataRowState.Deleted;
                    wrapper.DateTimeChange = DateTime.Now;

                    EnsureTrackingForItem(wrapper);

                    var tracking = Trackings.FirstOrDefault(t => t.UniqueId == wrapper.TrackingUniqueId);
                    if (tracking != null)
                    {
                        tracking.EntityState = EntityState.Deleted;
                        Trackings.Remove(tracking);

                        if (IsLogging)
                        {
                            UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                            {
                                TrackingRecord = tracking,
                                LogAction = LogAction.Delete,
                                UpdatedFields = new Dictionary<string, object>()
                            };
                        }
                    }

                    originalList.RemoveAt(index);
                    deletedList.Add(wrapper);
                }

                _fullData.RemoveAt(index);

                for (int i = 0; i < _fullData.Count; i++)
                {
                    if (_fullData[i] is DataRowWrapper dataRow)
                    {
                        dataRow.RowID = i;
                    }
                }

                if (_dataOffset > index)
                {
                    _dataOffset--;
                }

                UpdateRowCount();
            }
            catch (Exception ex)
            {
            }
        }

        private void HandleItemMoved(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= _fullData.Count || newIndex < 0 || newIndex >= _fullData.Count || _bindingSource == null) return;

            var item = _fullData[oldIndex];
            _fullData.RemoveAt(oldIndex);
            _fullData.Insert(newIndex, item);

            for (int i = 0; i < _fullData.Count; i++)
            {
                if (_fullData[i] is DataRowWrapper dataRow)
                {
                    dataRow.RowID = i;
                    EnsureTrackingForItem(dataRow);
                }
            }

            var wrapper = item as DataRowWrapper;
            if (wrapper != null)
            {
                var tracking = Trackings.FirstOrDefault(t => t.UniqueId == wrapper.TrackingUniqueId);
                if (tracking != null)
                {
                    tracking.OriginalIndex = originalList.IndexOf(wrapper);
                    tracking.CurrentIndex = newIndex;

                    if (IsLogging)
                    {
                        UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                        {
                            TrackingRecord = tracking,
                            LogAction = LogAction.Update,
                            UpdatedFields = new Dictionary<string, object>()
                        };
                    }
                }
            }

            if (_dataOffset == oldIndex)
            {
                _dataOffset = newIndex;
            }
            else if (_dataOffset > oldIndex && _dataOffset <= newIndex)
            {
                _dataOffset--;
            }
            else if (_dataOffset < oldIndex && _dataOffset >= newIndex)
            {
                _dataOffset++;
            }
        }

        #endregion

        #region Navigator Event Management

        /// <summary>
        /// Detaches events from the navigator
        /// </summary>
        private void DetachNavigatorEvents()
        {
            if (_dataNavigator == null) return;
            _dataNavigator.CallPrinter -= DataNavigator_CallPrinter;
            _dataNavigator.SendMessage -= DataNavigator_SendMessage;
            _dataNavigator.ShowSearch -= DataNavigator_ShowSearch;
            _dataNavigator.NewRecordCreated -= DataNavigator_NewRecordCreated;
            _dataNavigator.SaveCalled -= DataNavigator_SaveCalled;
            _dataNavigator.DeleteCalled -= DataNavigator_DeleteCalled;
            _dataNavigator.EditCalled -= DataNavigator_EditCalled;
            _dataNavigator.PositionChanged -= DataNavigator_PositionChanged;
            _dataNavigator.CurrentChanged -= DataNavigator_CurrentChanged;
            _dataNavigator.ListChanged -= DataNavigator_ListChanged;
            _dataNavigator.DataSourceChanged -= DataNavigator_DataSourceChanged;
        }

        /// <summary>
        /// Attaches events to the navigator
        /// </summary>
        private void AttachNavigatorEvents()
        {
            if (_dataNavigator == null) return;
            _dataNavigator.CallPrinter += DataNavigator_CallPrinter;
            _dataNavigator.SendMessage += DataNavigator_SendMessage;
            _dataNavigator.ShowSearch += DataNavigator_ShowSearch;
            _dataNavigator.NewRecordCreated += DataNavigator_NewRecordCreated;
            _dataNavigator.SaveCalled += DataNavigator_SaveCalled;
            _dataNavigator.DeleteCalled += DataNavigator_DeleteCalled;
            _dataNavigator.EditCalled += DataNavigator_EditCalled;
            _dataNavigator.PositionChanged += DataNavigator_PositionChanged;
            _dataNavigator.CurrentChanged += DataNavigator_CurrentChanged;
            _dataNavigator.ListChanged += DataNavigator_ListChanged;
            _dataNavigator.DataSourceChanged += DataNavigator_DataSourceChanged;
        }

        #endregion

        #region DataNavigator Event Handlers

        private void DataNavigator_CallPrinter(object sender, BindingSource bs)
        {
            try
            {
                // Raise the grid's PrinterButton event
                CallPrinter?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
            }
        }

        private void DataNavigator_SendMessage(object sender, BindingSource bs)
        {
            try
            {
                SendMessage?.Invoke(this, EventArgs.Empty);
                if (_currentRow != null && _currentRow.DisplayIndex >= 0 && _currentRow.DisplayIndex < _fullData.Count)
                {
                    var selectedItem = _fullData[_currentRow.DisplayIndex];
                    MessageBox.Show($"Sharing item: {selectedItem}", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No item selected to share.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void DataNavigator_ShowSearch(object sender, BindingSource bs)
        {
            try
            {
                ShowSearch?.Invoke(this, EventArgs.Empty);
                MessageBox.Show("Search/Filter UI not implemented yet.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
            }
        }

        private void DataNavigator_NewRecordCreated(object sender, BindingSource bs)
        {
            try
            {
                if (_dataSource is BindingSource)
                {
                    CreateNewRecordForBindingSource();
                }
                else if (_dataSource is IList)
                {
                    CreateNewRecordForIList();
                }

                NewRecordCreated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
            }
        }

        private void DataNavigator_SaveCalled(object sender, BindingSource bs)
        {
            try
            {
                SaveCalled?.Invoke(sender, null);
                SaveToDataSource();
                MessageBox.Show("Changes saved.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataNavigator_DeleteCalled(object sender, BindingSource bs)
        {
            try
            {
                DeleteCalled?.Invoke(sender, EventArgs.Empty);

                if (_currentRow == null || _fullData == null || !_fullData.Any())
                {
                    return;
                }

                int dataIndex = _currentRow.DisplayIndex;
                if (dataIndex < 0 || dataIndex >= _fullData.Count)
                {
                    return;
                }

                var item = _fullData[dataIndex];
                var wrapper = item as DataRowWrapper;
                if (wrapper == null)
                {
                    return;
                }

                DeleteFromDataSource(wrapper.OriginalData);

                if (_fullData.Any())
                {
                    int newSelectedIndex = Math.Min(dataIndex, _fullData.Count - 1);
                    if (newSelectedIndex >= _dataOffset && newSelectedIndex < _dataOffset + Rows.Count)
                    {
                        SelectCell(newSelectedIndex - _dataOffset, _selectedColumnIndex >= 0 ? _selectedColumnIndex : 0);
                    }
                }
                else
                {
                    _currentRow = null;
                    _currentRowIndex = -1;
                }

                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting record: {ex.Message}", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataNavigator_EditCalled(object sender, BindingSource bs)
        {
            try
            {
                EditCalled?.Invoke(sender, EventArgs.Empty);
                if (_selectedCell != null)
                {
                    BeginEdit();
                }
                else
                {
                    MessageBox.Show("Please select a cell to edit.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void DataNavigator_PositionChanged(object sender, EventArgs e)
        {
            int targetIndex = DataNavigator?.BindingSource.Position ?? -1;
            if (targetIndex >= 0 && targetIndex < _fullData.Count)
            {
                if (_currentRow == null || _currentRow.DisplayIndex != targetIndex)
                {
                    int visibleRowCount = GetVisibleRowCount();
                    int newOffset = Math.Max(0, Math.Min(targetIndex - (visibleRowCount / 2), _fullData.Count - visibleRowCount));
                    _dataOffset = newOffset;

                    FillVisibleRows();
                    int newRowIndex = targetIndex - _dataOffset;
                    if (newRowIndex >= 0 && newRowIndex < Rows.Count)
                    {
                        SelectCell(newRowIndex, _selectedColumnIndex >= 0 ? _selectedColumnIndex : 0);
                    }

                    UpdateScrollBars();
                    Invalidate();
                }
            }
        }

        private void DataNavigator_CurrentChanged(object sender, EventArgs e)
        {
            FillVisibleRows();
            Invalidate();
        }

        private void DataNavigator_ListChanged(object sender, ListChangedEventArgs e)
        {
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }

        private void DataNavigator_DataSourceChanged(object sender, EventArgs e)
        {
            InitializeRows();
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }

        #endregion
    }
}
