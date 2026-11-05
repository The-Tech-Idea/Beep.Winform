using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.GridX.Query;
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
        public bool IsFiltered => _isFiltered || (SortFilter?.HasActiveFilters ?? false);

        /// <summary>
        /// Gets the count of filtered (visible) rows when a filter is active
        /// </summary>
        [Browsable(false)]
        public int FilteredRowCount => IsFiltered
            ? FilteredRowIndices?.Count ?? 0
            : Data.Rows.Count;

        [Browsable(false)]
        public IReadOnlyList<int>? FilteredRowIndices => ComputeFilteredRowIndices();

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
                ClearToolbarSearch();
                return;
            }

            if (string.IsNullOrWhiteSpace(columnName) && !string.IsNullOrWhiteSpace(SearchColumnName))
                columnName = SearchColumnName;

            var query = new GridQuery();
            var group = new GridFilterGroup { Logic = FilterLogic.Or };

            if (string.IsNullOrEmpty(columnName))
            {
                foreach (var column in Data.Columns.Where(c =>
                    c.Visible && !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID))
                {
                    group.Conditions.Add(new GridFilterCondition
                    {
                        ColumnName = column.ColumnName,
                        Operator = FilterOperator.Contains,
                        Value = searchText
                    });
                }
            }
            else
            {
                group.Conditions.Add(new GridFilterCondition
                {
                    ColumnName = columnName,
                    Operator = FilterOperator.Contains,
                    Value = searchText
                });
            }

            if (group.Conditions.Count == 0)
            {
                ClearFilter();
                return;
            }

            query.Filters.Add(group);
            SortFilter.AppendQueryConditions(query);

            // Store the same criteria the query used so the active-filter
            // chips and save/load paths describe the real condition.
            var config = new FilterConfiguration("Quick Filter")
            {
                IsActive = true,
                Logic = group.Logic
            };
            foreach (var condition in group.Conditions)
            {
                config.AddCriteria(new FilterCriteria(
                    condition.ColumnName,
                    condition.Operator,
                    condition.Value ?? string.Empty));
            }

            _isFiltered = true;
            _activeFilter = config;
            ToolbarState.SearchText = searchText;

            if (EnableVirtualization && VirtualDataSource != null)
            {
                RowVirtualizer.SetQueryFilters(query.Filters);
                ToolbarState.SearchMatchCount = 0;
                RowVirtualizer.Refresh();
                SafeInvalidate();
                OnFilterApplied(new FilterAppliedEventArgs(_activeFilter, 0));
                return;
            }

            var indices = ComputeFilteredRowIndices() ?? new List<int>();
            UpdateFilteredDisplay(indices);
            OnFilterApplied(new FilterAppliedEventArgs(_activeFilter, indices.Count));
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

            _isFiltered = true;
            _activeFilter.IsActive = true;

            var indices = ComputeFilteredRowIndices() ?? new List<int>();
            UpdateFilteredDisplay(indices);
            OnFilterApplied(new FilterAppliedEventArgs(_activeFilter, indices.Count));
        }

        /// <summary>
        /// Clears the active filter and shows all rows
        /// </summary>
        /// <summary>
        /// Clears every filter on the grid, whichever route applied it.
        /// </summary>
        /// <remarks>
        /// The grid reaches a filtered state through two systems that share no state: this one,
        /// which owns <see cref="ActiveFilter"/>, and <c>GridSortFilterHelper</c>, which owns the
        /// per-column contains/in filters behind the header popups. Both write
        /// <c>row.IsVisible</c> and neither used to read the other, so clearing through one left
        /// the other's criteria in force - or, worse, left this one reporting
        /// <see cref="IsFiltered"/> as true over a grid showing every row.
        ///
        /// <c>_isClearingAllFilters</c> guards the mutual call: each route now clears the other,
        /// and without it they would call back and forth.
        /// </remarks>
        public void ClearFilter()
        {
            _isFiltered = false;
            ToolbarState.SearchText = string.Empty;

            if (EnableVirtualization && VirtualDataSource != null)
                RowVirtualizer.ClearQueryFilters();

            if (_activeFilter != null)
            {
                _activeFilter.IsActive = false;
                _activeFilter = null;
            }

            if (!_isClearingAllFilters)
            {
                _isClearingAllFilters = true;
                try { SortFilter?.ClearFilters(); }
                finally { _isClearingAllFilters = false; }
            }

            UpdateFilteredDisplay();
            if (EnableVirtualization && VirtualDataSource != null)
                RowVirtualizer.Refresh();
            OnFilterCleared();
        }

        /// <summary>Guards the mutual clear between the grid's two filter systems.</summary>
        internal bool _isClearingAllFilters;

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
        /// Removes the filter criterion at the given index and re-applies
        /// the remaining criteria.  Used by the active-filter chips row so
        /// each chip's "x" removes exactly that condition.
        /// </summary>
        public bool RemoveFilterCriterionAt(int index)
        {
            if (_activeFilter == null || index < 0 || index >= _activeFilter.Criteria.Count)
                return false;

            _activeFilter.Criteria.RemoveAt(index);

            if (_activeFilter.Criteria.Count == 0)
            {
                ClearFilter();
            }
            else
            {
                ApplyActiveFilter();
            }

            return true;
        }

        /// <summary>
        /// Saves the current filter configuration to a JSON file
        /// </summary>
        public void SaveFilterConfiguration(string filePath)
        {
            if (_activeFilter == null)
            {
                ReportOperationError("Filter.Save",
                    new InvalidOperationException("No active filter to save"));
                return;
            }

            try
            {
                var envelope = new SavedFilterEnvelope
                {
                    Version = 1,
                    Filter = _activeFilter
                };
                var json = System.Text.Json.JsonSerializer.Serialize(envelope,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(filePath, json);
            }
            catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
            {
                ReportOperationError("Filter.Save", ex);
            }
        }

        /// <summary>
        /// Loads a filter configuration from a JSON file. Unknown columns are
        /// removed and reported instead of being applied.
        /// </summary>
        public void LoadFilterConfiguration(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                ReportOperationError("Filter.Load",
                    new System.IO.FileNotFoundException("Filter configuration file not found", filePath));
                return;
            }

            var config = ReadFilterConfiguration(System.IO.File.ReadAllText(filePath));
            if (config != null)
                ActiveFilter = config;
        }

        private FilterConfiguration? ReadFilterConfiguration(string json)
        {
            try
            {
                var envelope = System.Text.Json.JsonSerializer.Deserialize<SavedFilterEnvelope>(json);
                var config = envelope?.Filter
                    ?? System.Text.Json.JsonSerializer.Deserialize<FilterConfiguration>(json);
                if (config == null) return null;

                int removed = config.Criteria.RemoveAll(c => !IsKnownFilterColumn(c.ColumnName));
                if (removed > 0)
                {
                    ReportOperationError("Filter.LoadSaved",
                        new InvalidOperationException(
                            $"Removed {removed} saved filter condition(s) for unknown columns."));
                }

                return config;
            }
            catch (Exception ex) when (ex is System.Text.Json.JsonException or System.IO.IOException)
            {
                ReportOperationError("Filter.LoadSaved", ex);
                return null;
            }
        }

        internal void PruneUnknownFilterColumns()
        {
            SortFilter.PruneUnknownColumns();

            if (_activeFilter == null)
                return;

            int removed = _activeFilter.Criteria.RemoveAll(c => !IsKnownFilterColumn(c.ColumnName));
            if (removed == 0) return;

            if (_activeFilter.Criteria.Count == 0)
            {
                _activeFilter = null;
                _isFiltered = false;
            }
        }

        private bool IsKnownFilterColumn(string? columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName) ||
                string.Equals(columnName, "All Columns", StringComparison.OrdinalIgnoreCase))
                return true;

            return Data.Columns.Any(c =>
                string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
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
                    var config = ReadFilterConfiguration(System.IO.File.ReadAllText(file));
                    if (config != null)
                        configs.Add(config);
                }
                catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException)
                {
                    ReportOperationError("Filter.LoadSaved", ex);
                }
            }

            return configs;
        }

        #endregion

        #region Private Filter Methods

        /// <summary>
        /// Applies a query to the current rows through the shared pipeline and
        /// returns the source indices of matching rows.
        /// </summary>
        private List<int> ApplyQueryFilterToRows(GridQuery query)
        {
            var view = GridView.Build(Data.Rows, Data.Columns, includeHidden: true);
            view.Apply(query);
            return view.Rows.Select(r => r.SourceIndex).ToList();
        }

        /// <summary>
        /// Converts an <see cref="FilterConfiguration"/> into the shared query
        /// representation so the active filter and quick filter use one engine.
        /// </summary>
        internal static GridQuery BuildQuery(FilterConfiguration config)
        {
            var query = new GridQuery();
            var group = new GridFilterGroup { Logic = config.Logic };

            foreach (var criterion in config.Criteria.Where(c => c.IsEnabled))
            {
                group.Conditions.Add(new GridFilterCondition
                {
                    ColumnName = criterion.ColumnName,
                    Operator = criterion.Operator,
                    Value = ToQueryValue(criterion.Value),
                    Value2 = ToQueryValue(criterion.Value2)
                });
            }

            if (group.Conditions.Count > 0)
                query.Filters.Add(group);

            return query;
        }

        private static string? ToQueryValue(object? value)
        {
            if (value == null) return null;
            if (value is string text) return text;
            if (value is IEnumerable enumerable)
                return string.Join("\u001F", enumerable.Cast<object?>()
                    .Select(v => Convert.ToString(v, CultureInfo.InvariantCulture)));
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private List<int>? ComputeFilteredRowIndices()
        {
            var query = _activeFilter != null
                ? BuildQuery(_activeFilter)
                : new GridQuery();
            SortFilter.AppendQueryConditions(query);

            if (query.Filters.Count == 0)
                return null;

            return ApplyQueryFilterToRows(query);
        }

        private void UpdateFilteredDisplay(IReadOnlyList<int>? filteredIndices = null)
        {
            Footer.Invalidate();

            var indices = filteredIndices ?? ComputeFilteredRowIndices();
            ToolbarState.SearchMatchCount = string.IsNullOrWhiteSpace(ToolbarState.SearchText)
                ? 0
                : indices?.Count ?? 0;

            if (indices == null)
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
                var visibleSet = new HashSet<int>(indices);

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    Data.Rows[i].IsVisible = visibleSet.Contains(i);
                }
            }

            Layout.RowMetrics.Invalidate();

            // Recalculate layout and refresh display
            Layout.Recalculate();
            ScrollBars?.UpdateBars();
            SafeInvalidate();

            // Recompute groups when a filter is applied/removed while grouping is active
            if (GroupEngine?.IsGrouped == true)
            {
                GroupEngine.ApplyGrouping();
            }

            // A changed query starts at the first visible row, not at row zero,
            // which may be hidden by the filter.
            if (Data.Rows.Count > 0)
            {
                if (indices == null)
                {
                    GoToFirstPage();
                }
                else
                {
                    int firstVisible = 0;
                    var visibleSet = new HashSet<int>(indices);
                    for (int i = 0; i < Data.Rows.Count; i++)
                    {
                        if (visibleSet.Contains(i))
                        {
                            firstVisible = i;
                            break;
                        }
                    }

                    Selection.SelectCell(firstVisible, Selection.ColumnIndex);
                    Scroll?.SetVerticalIndex(firstVisible);
                    SafeInvalidate();
                }
            }

            UpdateAccessibleDescription();
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
    /// Versioned envelope for saved filter files. Older files that contain a
    /// bare <see cref="FilterConfiguration"/> are still accepted by the loader.
    /// </summary>
    internal sealed class SavedFilterEnvelope
    {
        public int Version { get; set; } = 1;
        public FilterConfiguration? Filter { get; set; }
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
