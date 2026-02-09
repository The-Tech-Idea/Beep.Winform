using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing column management functionality for BeepSimpleGrid
    /// Handles column creation, configuration, resizing, and type mapping
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Column Creation

        private void EnsureDefaultColumns()
        {
            if (_columns == null)
                _columns = new List<BeepColumnConfig>();

            // Check if Sel column exists
            if (!_columns.Any(c => c.ColumnName == "Sel"))
            {
                var selColumn = new BeepColumnConfig
                {
                    ColumnCaption = "?",
                    ColumnName = "Sel",
                    Width = _selectionColumnWidth > 0 ? _selectionColumnWidth : 30,
                    Index = 0,
                    Visible = true,
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
                _columns.Add(selColumn);
            }

            // Check if RowNum column exists
            if (!_columns.Any(c => c.ColumnName == "RowNum"))
            {
                var rowNumColumn = new BeepColumnConfig
                {
                    ColumnCaption = "#",
                    ColumnName = "RowNum",
                    Width = 30,
                    Index = 1,
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
                _columns.Add(rowNumColumn);
            }

            // Check if RowID column exists
            if (!_columns.Any(c => c.ColumnName == "RowID"))
            {
                var rowIdColumn = new BeepColumnConfig
                {
                    ColumnCaption = "RowID",
                    ColumnName = "RowID",
                    Width = 30,
                    Index = 2,
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
                _columns.Add(rowIdColumn);
            }
        }

        public void CreateColumnsForEntity()
        {
            try
            {
                if (Entity == null || Entity.Fields == null)
                {
                    return;
                }

                // Initialize columns collection if null
                if (_columns == null)
                    _columns = new List<BeepColumnConfig>();

                // Ensure we have the system columns
                EnsureSystemColumns();

                // Get the starting index for data columns (after system columns)
                int startIndex = _columns.Count(c => c.IsSelectionCheckBox || c.IsRowNumColumn || c.IsRowID);

                // Create a dictionary of existing columns for quick lookup
                Dictionary<string, BeepColumnConfig> existingColumns = _columns
                    .Where(c => c.ColumnName != null && !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID)
                    .ToDictionary(c => c.ColumnName, StringComparer.OrdinalIgnoreCase);

                // Track which entity fields we've processed
                HashSet<string> processedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // Update existing columns or add new ones for entity fields
                foreach (var field in Entity.Fields)
                {
                    if (string.IsNullOrEmpty(field.FieldName))
                        continue;

                    processedFields.Add(field.FieldName);

                    if (existingColumns.TryGetValue(field.FieldName, out BeepColumnConfig existingColumn))
                    {
                        // Update properties of existing column
                        existingColumn.PropertyTypeName = field.Fieldtype;
                        existingColumn.ColumnType = MapPropertyTypeToDbFieldCategory(field.Fieldtype);
                        existingColumn.CellEditor = MapPropertyTypeToCellEditor(field.Fieldtype);

                        // Only update display properties if they aren't customized
                        if (string.IsNullOrEmpty(existingColumn.ColumnCaption) ||
                            existingColumn.ColumnCaption == existingColumn.ColumnName)
                        {
                            existingColumn.ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(field.FieldName);
                        }

                        // Update format for date/time fields if not already set
                        if (string.IsNullOrEmpty(existingColumn.Format))
                        {
                            Type Fieldtype = Type.GetType(field.Fieldtype, throwOnError: false) ?? typeof(object);
                            if (Fieldtype == typeof(DateTime))
                            {
                                existingColumn.Format = "g";
                            }
                        }

                        // Update enum items if applicable
                        Type fieldType2 = Type.GetType(field.Fieldtype, throwOnError: false);
                        if (fieldType2?.IsEnum == true && (existingColumn.Items == null || !existingColumn.Items.Any()))
                        {
                            existingColumn.Items = CreateEnumItems(fieldType2);
                        }
                    }
                    else
                    {
                        // Create a new column for this field
                        var colConfig = new BeepColumnConfig
                        {
                            ColumnName = field.FieldName,
                            ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(field.FieldName),
                            PropertyTypeName = field.Fieldtype,
                            GuidID = Guid.NewGuid().ToString(),
                            Visible = true,
                            Width = 100,
                            SortMode = DataGridViewColumnSortMode.Automatic,
                            Resizable = DataGridViewTriState.True,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                            Index = startIndex++
                        };

                        colConfig.ColumnType = MapPropertyTypeToDbFieldCategory(colConfig.PropertyTypeName);
                        colConfig.CellEditor = MapPropertyTypeToCellEditor(colConfig.PropertyTypeName);

                        Type Fieldtype = Type.GetType(colConfig.PropertyTypeName, throwOnError: false);
                        if (Fieldtype == typeof(DateTime))
                        {
                            colConfig.Format = "g";
                        }
                        else if (Fieldtype?.IsEnum == true)
                        {
                            colConfig.Items = CreateEnumItems(Fieldtype);
                        }

                        _columns.Add(colConfig);
                    }
                }

                // Reindex columns to ensure proper ordering
                for (int i = 0; i < _columns.Count; i++)
                {
                    _columns[i].Index = i;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void CreateColumnsFromType(Type entityType)
        {
            try
            {
                if (entityType == null)
                {
                    return;
                }

                // Initialize columns collection if null
                if (_columns == null)
                    _columns = new List<BeepColumnConfig>();

                // Ensure we have the system columns
                EnsureSystemColumns();

                // Get the starting index for data columns (after system columns)
                int startIndex = _columns.Count(c => c.IsSelectionCheckBox || c.IsRowNumColumn || c.IsRowID);

                // Create a dictionary of existing columns for quick lookup
                Dictionary<string, BeepColumnConfig> existingColumns = _columns
                    .Where(c => c.ColumnName != null && !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID)
                    .ToDictionary(c => c.ColumnName, StringComparer.OrdinalIgnoreCase);

                // Track which properties we've processed
                HashSet<string> processedProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // Update existing columns or add new ones for entity properties
                foreach (var prop in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    // Skip indexers and other non-data properties
                    if (prop.GetIndexParameters().Length > 0) continue;

                    string propertyName = prop.Name;
                    processedProperties.Add(propertyName);
                    string propertyTypeName = prop.PropertyType.AssemblyQualifiedName;

                    if (existingColumns.TryGetValue(propertyName, out BeepColumnConfig existingColumn))
                    {
                        // Update properties of existing column
                        existingColumn.PropertyTypeName = propertyTypeName;
                        existingColumn.ColumnType = MapPropertyTypeToDbFieldCategory(propertyTypeName);
                        existingColumn.CellEditor = MapPropertyTypeToCellEditor(propertyTypeName);

                        // Only update display properties if they aren't customized
                        if (string.IsNullOrEmpty(existingColumn.ColumnCaption) ||
                            existingColumn.ColumnCaption == existingColumn.ColumnName)
                        {
                            existingColumn.ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(propertyName);
                        }

                        // Update format for date/time fields if not already set
                        if (string.IsNullOrEmpty(existingColumn.Format) && prop.PropertyType == typeof(DateTime))
                        {
                            existingColumn.Format = "g";
                        }

                        // Update enum items if applicable
                        if (prop.PropertyType.IsEnum && (existingColumn.Items == null || !existingColumn.Items.Any()))
                        {
                            existingColumn.Items = CreateEnumItems(prop.PropertyType);
                        }
                    }
                    else
                    {
                        // Create a new column for this property
                        var colConfig = new BeepColumnConfig
                        {
                            ColumnName = propertyName,
                            ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(propertyName),
                            PropertyTypeName = propertyTypeName,
                            GuidID = Guid.NewGuid().ToString(),
                            Visible = true,
                            Width = 100,
                            SortMode = DataGridViewColumnSortMode.Automatic,
                            Resizable = DataGridViewTriState.True,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                            Index = startIndex++
                        };

                        colConfig.ColumnType = MapPropertyTypeToDbFieldCategory(colConfig.PropertyTypeName);
                        colConfig.CellEditor = MapPropertyTypeToCellEditor(colConfig.PropertyTypeName);

                        // Set format for date/time columns
                        if (prop.PropertyType == typeof(DateTime))
                        {
                            colConfig.Format = "g";
                        }

                        // Handle enum properties
                        if (prop.PropertyType.IsEnum)
                        {
                            colConfig.Items = CreateEnumItems(prop.PropertyType);
                        }

                        _columns.Add(colConfig);
                    }
                }

                // Reindex columns to ensure proper ordering
                for (int i = 0; i < _columns.Count; i++)
                {
                    _columns[i].Index = i;
                }
            }
            catch (Exception ex)
            {
                // Don't call EnsureDefaultColumns() here to preserve existing columns on error
            }
        }

        private void EnsureSystemColumns()
        {
            // Add selection checkbox column if missing
            if (!_columns.Any(c => c.IsSelectionCheckBox))
            {
                var selColumn = new BeepColumnConfig
                {
                    ColumnCaption = "?",
                    ColumnName = "Sel",
                    Width = _selectionColumnWidth,
                    Index = 0,
                    Visible = ShowCheckboxes,
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
                _columns.Add(selColumn);
            }

            // Add row number column if missing
            if (!_columns.Any(c => c.IsRowNumColumn))
            {
                var rowNumColumn = new BeepColumnConfig
                {
                    ColumnCaption = "#",
                    ColumnName = "RowNum",
                    Width = 30,
                    Index = _columns.Count(c => c.IsSelectionCheckBox),
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
                    AggregationType = AggregationType.Count
                };
                rowNumColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowNumColumn.PropertyTypeName);
                _columns.Add(rowNumColumn);
            }

            // Add RowID column if missing
            if (!_columns.Any(c => c.IsRowID))
            {
                var rowIdColumn = new BeepColumnConfig
                {
                    ColumnCaption = "RowID",
                    ColumnName = "RowID",
                    Width = 30,
                    Index = _columns.Count(c => c.IsSelectionCheckBox || c.IsRowNumColumn),
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
                _columns.Add(rowIdColumn);
            }
        }

        private List<SimpleItem> CreateEnumItems(Type enumType)
        {
            var items = new List<SimpleItem>();
            if (enumType != null && enumType.IsEnum)
            {
                foreach (var val in Enum.GetValues(enumType))
                {
                    items.Add(new SimpleItem
                    {
                        Value = val,
                        Text = val.ToString(),
                        DisplayField = val.ToString()
                    });
                }
            }
            return items;
        }

        #endregion

        #region Column Helper Methods

        public BeepColumnConfig GetColumnByName(string columnName)
        {
            // Find index first
            int index = Columns.FindIndex(c => c.ColumnName.Equals(columnName, StringComparison.InvariantCultureIgnoreCase));
            if (index == -1) return null;
            return Columns[index];
        }

        public Dictionary<string, BeepColumnConfig> GetDictionaryColumns()
        {
            Dictionary<string, BeepColumnConfig> dict = new Dictionary<string, BeepColumnConfig>();
            foreach (var col in Columns)
            {
                dict.Add(col.ColumnName, col);
            }
            return dict;
        }

        public BeepColumnConfig GetColumnByIndex(int index)
        {
            // Find index first
            int retindex = Columns.FindIndex(c => c.Index == index);
            if (retindex == -1) return null;
            return Columns[retindex];
        }

        public BeepColumnConfig GetColumnByCaption(string caption)
        {
            int index = Columns.FindIndex(c => c.ColumnName.Equals(caption, StringComparison.InvariantCultureIgnoreCase));
            if (index == -1) return null;

            return Columns[index];
        }

        private int GetColumnWidth(BeepColumnConfig column)
        {
            if (column == null) return _defaultcolumnheaderwidth;

            int maxWidth = 50; // Minimum width
            int padding = 8; // Padding for text

            using (Graphics g = CreateGraphics())
            {
                // Measure header text
                string headerText = column.ColumnCaption ?? column.ColumnName ?? "";
                if (!string.IsNullOrEmpty(headerText))
                {
                    SizeF headerSize = TextUtils.MeasureText(g, headerText, _columnHeadertextFont ?? Font);
                    maxWidth = Math.Max(maxWidth, (int)headerSize.Width + padding);
                }

                // Add space for sort/filter icons if enabled
                if (column.ShowSortIcon || column.ShowFilterIcon)
                {
                    int iconSpace = 0;
                    if (column.ShowSortIcon) iconSpace += 20; // Sort icon width
                    if (column.ShowFilterIcon) iconSpace += 20; // Filter icon width
                    maxWidth += iconSpace + 4; // Extra padding for icons
                }

                // Measure cell content if we have data
                if (_fullData != null && _fullData.Any())
                {
                    int sampleSize = Math.Min(100, _fullData.Count); // Sample first 100 rows for performance

                    for (int i = 0; i < sampleSize; i++)
                    {
                        var wrapper = _fullData[i] as DataRowWrapper;
                        if (wrapper?.OriginalData != null)
                        {
                            var prop = wrapper.OriginalData.GetType().GetProperty(column.ColumnName ?? column.ColumnCaption);
                            if (prop != null)
                            {
                                var value = prop.GetValue(wrapper.OriginalData);
                                string cellText = value?.ToString() ?? "";

                                if (!string.IsNullOrEmpty(cellText))
                                {
                                    SizeF cellSize = TextUtils.MeasureText(g, cellText, Font);
                                    maxWidth = Math.Max(maxWidth, (int)cellSize.Width + padding);
                                }
                            }
                        }
                    }
                }
            }

            // Cap the maximum width to prevent extremely wide columns
            return Math.Min(maxWidth, 300);
        }

        public void SetColumnWidth(string columnName, int width)
        {
            var column = GetColumnByName(columnName);
            if (column != null)
            {
                column.Width = Math.Max(20, width); // Ensure minimum width

                Invalidate(); // Redraw grid with updated column width
            }
        }

        private void AutoResizeColumnsToFitContent()
        {
            // want to resize all  columns its content
            if (Columns == null || Columns.Count == 0)
                return;
            // Calculate total width of all visible columns
            int totalWidth = Columns.Where(c => c.Visible).Sum(c => c.Width);
            int availableWidth = gridRect.Width - (ShowCheckboxes ? _selectionColumnWidth : 0) - (ShowRowNumbers ? 30 : 0);
            // If total width exceeds available width, resize columns proportionally
            if (totalWidth > availableWidth)
            {
                float scaleFactor = (float)availableWidth / totalWidth;
                foreach (var col in Columns.Where(c => c.Visible))
                {
                    col.Width = (int)(col.Width * scaleFactor);
                }
            }
        }

        #endregion

        #region Column Color Management

        /// <summary>
        /// Sets custom colors for a specific column
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="backColor">Background color for cells</param>
        /// <param name="foreColor">Text color for cells</param>
        /// <param name="headerBackColor">Background color for header (optional)</param>
        /// <param name="headerForeColor">Text color for header (optional)</param>
        public void SetColumnColors(string columnName, Color backColor, Color foreColor,
            Color? headerBackColor = null, Color? headerForeColor = null)
        {
            var column = GetColumnByName(columnName);
            if (column != null)
            {
                column.UseCustomColors = true;
                column.ColumnBackColor = backColor;
                column.ColumnForeColor = foreColor;

                if (headerBackColor.HasValue)
                    column.ColumnHeaderBackColor = headerBackColor.Value;
                if (headerForeColor.HasValue)
                    column.ColumnHeaderForeColor = headerForeColor.Value;

                Invalidate(); // Redraw grid with new colors
            }
        }

        /// <summary>
        /// Sets custom colors for a column by index
        /// </summary>
        public void SetColumnColors(int columnIndex, Color backColor, Color foreColor,
            Color? headerBackColor = null, Color? headerForeColor = null)
        {
            var column = GetColumnByIndex(columnIndex);
            if (column != null)
            {
                column.UseCustomColors = true;
                column.ColumnBackColor = backColor;
                column.ColumnForeColor = foreColor;

                if (headerBackColor.HasValue)
                    column.ColumnHeaderBackColor = headerBackColor.Value;
                if (headerForeColor.HasValue)
                    column.ColumnHeaderForeColor = headerForeColor.Value;

                Invalidate();
            }
        }

        /// <summary>
        /// Removes custom colors from a column (reverts to theme colors)
        /// </summary>
        public void ClearColumnColors(string columnName)
        {
            var column = GetColumnByName(columnName);
            if (column != null)
            {
                column.UseCustomColors = false;
                column.ColumnBackColor = Color.Empty;
                column.ColumnForeColor = Color.Empty;
                column.ColumnHeaderBackColor = Color.Empty;
                column.ColumnHeaderForeColor = Color.Empty;
                column.ColumnBorderColor = Color.Empty;

                Invalidate();
            }
        }

        /// <summary>
        /// Gets the effective background color for a column
        /// </summary>
        public Color GetColumnBackColor(BeepColumnConfig column)
        {
            return column.HasCustomBackColor ? column.ColumnBackColor : _currentTheme.GridBackColor;
        }

        /// <summary>
        /// Gets the effective foreground color for a column
        /// </summary>
        public Color GetColumnForeColor(BeepColumnConfig column)
        {
            return column.HasCustomForeColor ? column.ColumnForeColor : _currentTheme.GridForeColor;
        }

        #endregion

        #region Type Mapping

        private DbFieldCategory MapPropertyTypeToDbFieldCategory(Type type)
        {
            if (type == typeof(string)) return DbFieldCategory.String;
            if (type == typeof(Char)) return DbFieldCategory.Char;
            if (type == typeof(int) || type == typeof(long)) return DbFieldCategory.Numeric;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return DbFieldCategory.Numeric;
            if (type == typeof(DateTime)) return DbFieldCategory.Date;
            if (type == typeof(bool)) return DbFieldCategory.Boolean;
            if (type == typeof(Guid)) return DbFieldCategory.Guid;

            // Default to string for unknown types
            return DbFieldCategory.String;
        }

        private BeepColumnType MapPropertyTypeToCellEditor(Type type)
        {
            if (type == typeof(string)) return BeepColumnType.Text;
            if (type == typeof(int) || type == typeof(long)) return BeepColumnType.NumericUpDown;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return BeepColumnType.Text;
            if (type == typeof(DateTime)) return BeepColumnType.DateTime;
            if (type == typeof(bool)) return BeepColumnType.CheckBoxBool;
            if (type == typeof(Char)) return BeepColumnType.CheckBoxChar;
            if (type == typeof(Guid)) return BeepColumnType.Text;

            // Fallback to text if the type is unknown
            return BeepColumnType.Text;
        }

        private DbFieldCategory MapPropertyTypeToDbFieldCategory(string propertyTypeName)
        {
            // Default to String if typeName is null or empty
            if (string.IsNullOrWhiteSpace(propertyTypeName))
            {
                return DbFieldCategory.String;
            }

            // Attempt to resolve the Type from the AssemblyQualifiedName
            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
            {
                return DbFieldCategory.String;
            }

            // Map resolved Type to DbFieldCategory
            if (type == typeof(string)) return DbFieldCategory.String;
            if (type == typeof(int) || type == typeof(long)) return DbFieldCategory.Numeric;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return DbFieldCategory.Numeric;
            if (type == typeof(DateTime)) return DbFieldCategory.Date;
            if (type == typeof(bool)) return DbFieldCategory.Boolean;
            if (type == typeof(Char)) return DbFieldCategory.Char;
            if (type == typeof(Guid)) return DbFieldCategory.Guid;

            // Default to String for unknown types
            return DbFieldCategory.String;
        }

        private BeepColumnType MapPropertyTypeToCellEditor(string propertyTypeName)
        {
            // Default to Text if typeName is null or empty
            if (string.IsNullOrWhiteSpace(propertyTypeName))
            {
                return BeepColumnType.Text;
            }

            // Attempt to resolve the Type from the AssemblyQualifiedName
            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
            {
                return BeepColumnType.Text;
            }

            // Map resolved Type to BeepColumnType
            if (type == typeof(string)) return BeepColumnType.Text;
            if (type == typeof(int) || type == typeof(long)) return BeepColumnType.NumericUpDown;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return BeepColumnType.Text;
            if (type == typeof(DateTime)) return BeepColumnType.DateTime;
            if (type == typeof(bool)) return BeepColumnType.CheckBoxBool;
            if (type == typeof(Char)) return BeepColumnType.CheckBoxChar;
            if (type == typeof(Guid)) return BeepColumnType.Text;

            // Fallback to Text for unknown types
            return BeepColumnType.Text;
        }

        #endregion
    }
}
