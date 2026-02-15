using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Partial class containing advanced filtering functionality for BeepGridPro
    /// </summary>
    public partial class BeepGridPro
    {
        #region Filter Fields

        private FilterConfiguration? _activeFilter;
        private List<int>? _filteredRowIndices;
        private bool _isFiltered;

        #endregion

        #region Filter Properties

        /// <summary>
        /// Gets or sets the active filter configuration
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FilterConfiguration? ActiveFilter
        {
            get => _activeFilter;
            set
            {
                _activeFilter = value;
                ApplyActiveFilter();
            }
        }

        /// <summary>
        /// Gets whether the grid currently has an active filter
        /// </summary>
        [Browsable(false)]
        public bool IsFiltered => _isFiltered;

        /// <summary>
        /// Gets the count of filtered (visible) rows when a filter is active
        /// </summary>
        [Browsable(false)]
        public int FilteredRowCount => _isFiltered && _filteredRowIndices != null 
            ? _filteredRowIndices.Count 
            : Data.Rows.Count;

        #endregion

        #region Filter Events

        /// <summary>
        /// Raised when a filter is applied
        /// </summary>
        public event EventHandler<FilterAppliedEventArgs> FilterApplied;

        /// <summary>
        /// Raised when a filter is cleared
        /// </summary>
        public event EventHandler FilterCleared;

        #endregion

        #region Filter Methods

        /// <summary>
        /// Shows the advanced filter dialog using BeepFilter
        /// </summary>
        public void ShowAdvancedFilterDialog()
        {
            using (var filterForm = new BeepiFormPro())
            {
                filterForm.Text = "Advanced Filter";
                filterForm.Size = new System.Drawing.Size(900, 500);
                filterForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                filterForm.FormStyle = BeepThemesManager.CurrentStyle;
                filterForm.ShowCaptionBar = true;
                filterForm.ShowMinMaxButtons = false;
                filterForm.ShowCloseButton = true;
                filterForm.MaximizeBox = true;
                filterForm.MinimizeBox = false;
                filterForm.Theme = this.Theme;

                var beepFilter = new BeepFilter
                {
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    FilterStyle = FilterStyle.AdvancedDialog,
                    DisplayMode = FilterDisplayMode.AlwaysVisible,
                    AutoApply = false,
                    ShowActionButtons = true,
                    IsChild = true,
                    IsFrameless = true,
                    Theme = this.Theme,
                    EntityStructure = ConvertColumnsToEntityStructure(Data.Columns)
                };

                var initialFilter = _activeFilter?.Clone() ?? new FilterConfiguration("Advanced Filter");
                if (initialFilter.Criteria.Count == 0)
                {
                    var firstColumn = beepFilter.AvailableColumns.FirstOrDefault()?.ColumnName ?? string.Empty;
                    initialFilter.Criteria.Add(new FilterCriteria
                    {
                        ColumnName = firstColumn,
                        Operator = FilterOperator.Contains,
                        Value = string.Empty,
                        IsEnabled = true
                    });
                }
                beepFilter.ActiveFilter = initialFilter;

                void UpdateCriterion(int index, Action<FilterCriteria> updater)
                {
                    if (index < 0 || index >= beepFilter.ActiveFilter.Criteria.Count || updater == null)
                    {
                        return;
                    }

                    var updatedConfig = beepFilter.ActiveFilter.Clone();
                    var criterion = updatedConfig.Criteria[index]?.Clone() ?? new FilterCriteria();
                    updater(criterion);
                    updatedConfig.Criteria[index] = criterion;
                    beepFilter.ActiveFilter = updatedConfig;
                }

                System.Windows.Forms.ContextMenuStrip? activeContextMenu = null;

                void CloseActiveContextMenu()
                {
                    if (activeContextMenu == null)
                    {
                        return;
                    }

                    try
                    {
                        if (!activeContextMenu.IsDisposed)
                        {
                            activeContextMenu.Close();
                            activeContextMenu.Dispose();
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    finally
                    {
                        activeContextMenu = null;
                    }
                }

                void ShowContextMenu(System.Windows.Forms.ContextMenuStrip menu, System.Drawing.Point location)
                {
                    CloseActiveContextMenu();
                    activeContextMenu = menu;

                    menu.Closed += (_, __) =>
                    {
                        if (ReferenceEquals(activeContextMenu, menu))
                        {
                            activeContextMenu = null;
                        }

                        if (!filterForm.IsDisposed)
                        {
                            filterForm.BeginInvoke(new Action(() =>
                            {
                                if (!menu.IsDisposed)
                                {
                                    menu.Dispose();
                                }
                            }));
                        }
                        else if (!menu.IsDisposed)
                        {
                            menu.Dispose();
                        }
                    };

                    menu.Show(beepFilter, location);
                }

                filterForm.FormClosed += (_, __) => CloseActiveContextMenu();

                beepFilter.FieldSelectionRequested += (s, e) =>
                {
                    if (e.Index < 0 || e.Index >= beepFilter.ActiveFilter.Criteria.Count)
                    {
                        return;
                    }

                    var menu = new System.Windows.Forms.ContextMenuStrip();
                    foreach (var column in beepFilter.AvailableColumns)
                    {
                        var selectedColumn = column.ColumnName;
                        var display = string.IsNullOrWhiteSpace(column.DisplayName) ? selectedColumn : column.DisplayName;
                        var item = new System.Windows.Forms.ToolStripMenuItem(display) { Tag = selectedColumn };
                        item.Click += (_, __) =>
                        {
                            UpdateCriterion(e.Index, c =>
                            {
                                c.ColumnName = selectedColumn;
                                var ops = FilterOperatorExtensions.GetOperatorsForType(column.DataType ?? typeof(string));
                                if (ops.Length > 0)
                                {
                                    c.Operator = ops[0];
                                }
                                c.Value = string.Empty;
                                c.Value2 = string.Empty;
                            });
                        };
                        menu.Items.Add(item);
                    }

                    ShowContextMenu(menu, new System.Drawing.Point(e.Bounds.Left, e.Bounds.Bottom));
                };

                beepFilter.OperatorSelectionRequested += (s, e) =>
                {
                    if (e.Index < 0 || e.Index >= beepFilter.ActiveFilter.Criteria.Count)
                    {
                        return;
                    }

                    var current = beepFilter.ActiveFilter.Criteria[e.Index];
                    var selectedType = beepFilter.AvailableColumns
                        .FirstOrDefault(c => c.ColumnName == current.ColumnName)
                        ?.DataType ?? typeof(string);
                    var operators = FilterOperatorExtensions.GetOperatorsForType(selectedType);

                    var menu = new System.Windows.Forms.ContextMenuStrip();
                    foreach (var op in operators)
                    {
                        var operatorValue = op;
                        var item = new System.Windows.Forms.ToolStripMenuItem(op.GetDisplayName()) { Tag = operatorValue };
                        item.Click += (_, __) => UpdateCriterion(e.Index, c => c.Operator = operatorValue);
                        menu.Items.Add(item);
                    }

                    ShowContextMenu(menu, new System.Drawing.Point(e.Bounds.Left, e.Bounds.Bottom));
                };

                beepFilter.FilterAdded += (s, e) =>
                {
                    var lastIndex = Math.Max(0, beepFilter.ActiveFilter.Criteria.Count - 1);
                    if (lastIndex >= 0 && lastIndex < beepFilter.ActiveFilter.Criteria.Count)
                    {
                        var defaultColumn = beepFilter.AvailableColumns.FirstOrDefault()?.ColumnName ?? string.Empty;
                        UpdateCriterion(lastIndex, c =>
                        {
                            c.ColumnName = string.IsNullOrWhiteSpace(c.ColumnName) ? defaultColumn : c.ColumnName;
                        });
                    }
                };

                beepFilter.FilterApplied += (s, e) =>
                {
                    filterForm.DialogResult = System.Windows.Forms.DialogResult.OK;
                    filterForm.Close();
                };

                beepFilter.FilterCanceled += (s, e) =>
                {
                    filterForm.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    filterForm.Close();
                };

                filterForm.Controls.Add(beepFilter);

                filterForm.Shown += (s, e) =>
                {
                    beepFilter.ApplyTheme();
                    beepFilter.ActiveFilter = beepFilter.ActiveFilter.Clone();
                    beepFilter.Refresh();
                };

                if (filterForm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    ActiveFilter = beepFilter.ActiveFilter;
                }
            }
        }

        private EntityStructure ConvertColumnsToEntityStructure(BeepGridColumnConfigCollection columns)
        {
            var entityStructure = new EntityStructure
            {
                EntityName = "GridData",
                Fields = new List<EntityField>()
            };

            foreach (var column in columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID && c.Visible))
            {
                entityStructure.Fields.Add(new EntityField
                {
                    FieldName = column.ColumnName,
                    Originalfieldname = string.IsNullOrWhiteSpace(column.ColumnCaption) ? column.ColumnName : column.ColumnCaption,
                    Fieldtype = column.DataType?.FullName ?? typeof(string).FullName,
                    IsKey = false,
                    AllowDBNull = true
                });
            }

            return entityStructure;
        }

        /// <summary>
        /// Applies a quick filter to search across columns
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        /// <param name="columnName">Column to search, or null for all columns</param>
        public void ApplyQuickFilter(string searchText, string? columnName = null)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ClearFilter();
                return;
            }

            // Manually apply quick filter across all or specific column
            var searchLower = searchText.ToLowerInvariant();
            var matchingIndices = new List<int>();

            for (int i = 0; i < Data.Rows.Count; i++)
            {
                var row = Data.Rows[i];
                bool matches = false;

                if (string.IsNullOrEmpty(columnName))
                {
                    // Search all columns
                    foreach (var cell in row.Cells)
                    {
                        string cellText = cell.CellValue?.ToString() ?? string.Empty;
                        if (cellText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            matches = true;
                            break;
                        }
                    }
                }
                else
                {
                    // Search specific column
                    var cell = row.Cells.FirstOrDefault(c => c.ColumnName == columnName);
                    if (cell != null)
                    {
                        string cellText = cell.CellValue?.ToString() ?? string.Empty;
                        matches = cellText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                }

                if (matches)
                {
                    matchingIndices.Add(i);
                }
            }

            // Create and store filter configuration for display purposes
            var config = new FilterConfiguration("Quick Filter")
            {
                IsActive = true
            };
            config.AddCriteria(new FilterCriteria(
                columnName ?? "All Columns",
                FilterOperator.Contains,
                searchText
            ));

            // Apply the matching indices
            _filteredRowIndices = matchingIndices;
            _isFiltered = true;
            _activeFilter = config;

            UpdateFilteredDisplay();
            OnFilterApplied(new FilterAppliedEventArgs(_activeFilter, _filteredRowIndices.Count));
        }

        /// <summary>
        /// Applies the current active filter configuration
        /// </summary>
        public void ApplyActiveFilter()
        {
            if (_activeFilter == null || _activeFilter.Criteria.Count == 0)
            {
                ClearFilter();
                return;
            }

            _filteredRowIndices = ApplyFilterToRows(_activeFilter);
            _isFiltered = true;
            _activeFilter.IsActive = true;

            UpdateFilteredDisplay();
            OnFilterApplied(new FilterAppliedEventArgs(_activeFilter, _filteredRowIndices.Count));
        }

        /// <summary>
        /// Clears the active filter and shows all rows
        /// </summary>
        public void ClearFilter()
        {
            _filteredRowIndices = null;
            _isFiltered = false;
            
            if (_activeFilter != null)
            {
                _activeFilter.IsActive = false;
                _activeFilter = null;
            }

            UpdateFilteredDisplay();
            OnFilterCleared();
        }

        /// <summary>
        /// Adds a filter criterion to the active filter
        /// </summary>
        public void AddFilterCriterion(FilterCriteria criterion)
        {
            if (_activeFilter == null)
            {
                _activeFilter = new FilterConfiguration("Custom Filter");
            }

            _activeFilter.AddCriteria(criterion);
            ApplyActiveFilter();
        }

        /// <summary>
        /// Removes a filter criterion by column name
        /// </summary>
        public bool RemoveFilterCriterion(string columnName)
        {
            if (_activeFilter == null)
                return false;

            var removed = _activeFilter.RemoveCriteria(columnName);
            if (removed)
            {
                if (_activeFilter.Criteria.Count == 0)
                {
                    ClearFilter();
                }
                else
                {
                    ApplyActiveFilter();
                }
            }

            return removed;
        }

        /// <summary>
        /// Saves the current filter configuration to a JSON file
        /// </summary>
        public void SaveFilterConfiguration(string filePath)
        {
            if (_activeFilter == null)
                throw new InvalidOperationException("No active filter to save");

            var json = System.Text.Json.JsonSerializer.Serialize(_activeFilter, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            System.IO.File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads a filter configuration from a JSON file
        /// </summary>
        public void LoadFilterConfiguration(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException("Filter configuration file not found", filePath);

            var json = System.IO.File.ReadAllText(filePath);
            var config = System.Text.Json.JsonSerializer.Deserialize<FilterConfiguration>(json);

            if (config != null)
            {
                ActiveFilter = config;
            }
        }

        /// <summary>
        /// Gets all saved filter configurations from a directory
        /// </summary>
        public List<FilterConfiguration> GetSavedFilterConfigurations(string directory)
        {
            var configs = new List<FilterConfiguration>();

            if (!System.IO.Directory.Exists(directory))
                return configs;

            foreach (var file in System.IO.Directory.GetFiles(directory, "*.json"))
            {
                try
                {
                    var json = System.IO.File.ReadAllText(file);
                    var config = System.Text.Json.JsonSerializer.Deserialize<FilterConfiguration>(json);
                    if (config != null)
                    {
                        configs.Add(config);
                    }
                }
                catch
                {
                    // Skip invalid files
                }
            }

            return configs;
        }

        #endregion

        #region Private Filter Methods

        /// <summary>
        /// Applies filter configuration to grid rows using the generic FilterEngine
        /// </summary>
        private List<int> ApplyFilterToRows(FilterConfiguration config)
        {
            // Convert grid rows to ExpandoObject for generic filtering
            var wrappers = Data.Rows.Select((row, index) =>
            {
                var wrapper = new ExpandoObject() as IDictionary<string, object?>;
                wrapper["__RowIndex"] = index;

                // Add each cell as a property with column name
                foreach (var cell in row.Cells)
                {
                    wrapper[cell.ColumnName] = cell.CellValue;
                }

                return (ExpandoObject)wrapper;
            }).ToList();

            // Create filter engine and apply filter
            var filterEngine = new FilterEngine<ExpandoObject>(wrappers);
            return filterEngine.ApplyFilterGetIndices(config);
        }

        private void UpdateFilteredDisplay()
        {
            if (!_isFiltered || _filteredRowIndices == null)
            {
                // Show all rows
                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    Data.Rows[i].IsVisible = true;
                }
            }
            else
            {
                // Hide rows that don't match the filter
                var visibleSet = new HashSet<int>(_filteredRowIndices);
                
                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    Data.Rows[i].IsVisible = visibleSet.Contains(i);
                }
            }

            // Recalculate layout and refresh display
            Layout.Recalculate();
            ScrollBars?.UpdateBars();
            SafeInvalidate();
        }

        private void OnFilterApplied(FilterAppliedEventArgs args)
        {
            FilterApplied?.Invoke(this, args);
        }

        private void OnFilterCleared()
        {
            FilterCleared?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }

    /// <summary>
    /// Event args for filter applied event
    /// </summary>
    public class FilterAppliedEventArgs : EventArgs
    {
        /// <summary>
        /// The filter configuration that was applied
        /// </summary>
        public FilterConfiguration FilterConfiguration { get; }

        /// <summary>
        /// Number of rows that match the filter
        /// </summary>
        public int MatchingRowCount { get; }

        /// <summary>
        /// Initializes a new instance of the FilterAppliedEventArgs class
        /// </summary>
        /// <param name="config">The filter configuration that was applied</param>
        /// <param name="matchingRowCount">Number of rows that match the filter</param>
        public FilterAppliedEventArgs(FilterConfiguration config, int matchingRowCount)
        {
            FilterConfiguration = config;
            MatchingRowCount = matchingRowCount;
        }
    }
}
