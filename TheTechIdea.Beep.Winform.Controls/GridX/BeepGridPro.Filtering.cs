using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.GridX.Filtering;

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

        /// <summary>
        /// Gets or sets whether to show the quick filter bar
        /// </summary>
        [Browsable(true)]
        [Category("Filtering")]
        [DefaultValue(false)]
        [Description("Shows a quick filter bar above the grid for instant filtering")]
        public bool ShowQuickFilterBar { get; set; } = false;

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
        /// Shows the advanced filter dialog
        /// </summary>
        public void ShowAdvancedFilterDialog()
        {
            using (var dialog = new BeepAdvancedFilterDialog(Data.Columns, _activeFilter))
            {
                if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    ActiveFilter = dialog.Result;
                }
            }
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

            // Create filter configuration for quick filter
            var config = new FilterConfiguration("Quick Filter")
            {
                IsActive = true
            };
            config.AddCriteria(new FilterCriteria(
                columnName ?? "All",
                FilterOperator.Contains,
                searchText
            ));

            // Apply filter using generic engine
            _filteredRowIndices = ApplyFilterToRows(config);
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
            Invalidate();
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
