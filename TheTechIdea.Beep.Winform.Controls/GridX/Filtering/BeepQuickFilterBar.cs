using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Filtering;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Filtering
{
    /// <summary>
    /// Excel-like quick filter bar for BeepGridPro
    /// Provides instant filtering across all columns
    /// </summary>
    public class BeepQuickFilterBar : BaseControl
    {
        private BeepTextBox _searchTextBox = null!;
        private BeepButton _clearButton = null!;
        private BeepButton _advancedButton = null!;
        private BeepComboBox _columnSelector = null!;
        private BeepLabel _searchLabel = null!;
        private BeepLabel _filterCountLabel = null!;

        /// <summary>
        /// Raised when filter changes
        /// </summary>
        public event EventHandler<FilterEventArgs>? FilterChanged;
        
        /// <summary>
        /// Raised when advanced filter is requested
        /// </summary>
        public event EventHandler? AdvancedFilterRequested;

        private int _activeFilterCount = 0;

        /// <summary>
        /// Gets or sets the search text
        /// </summary>
        public string SearchText
        {
            get => _searchTextBox.Text;
            set => _searchTextBox.Text = value;
        }

        /// <summary>
        /// Gets or sets the selected column to filter (or "All Columns")
        /// </summary>
        public string SelectedColumn
        {
            get => (_columnSelector.SelectedItem as SimpleItem)?.Name ?? "All Columns";
            set
            {
                var item = _columnSelector.Items.Cast<SimpleItem>().FirstOrDefault(s => s.Name == value);
                if (item != null)
                    _columnSelector.SelectedItem = item;
            }
        }

        /// <summary>
        /// Sets the available columns
        /// </summary>
        public string[] AvailableColumns
        {
            set
            {
                _columnSelector.Items.Clear();
                _columnSelector.Items.Add(new SimpleItem { Name = "All Columns", Text = "All Columns" });
                if (value != null)
                {
                    foreach (var col in value)
                    {
                        _columnSelector.Items.Add(new SimpleItem { Name = col, Text = col });
                    }
                }
                _columnSelector.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of active filters (displayed as badge)
        /// </summary>
        public int ActiveFilterCount
        {
            get => _activeFilterCount;
            set
            {
                _activeFilterCount = value;
                UpdateFilterCountDisplay();
            }
        }

        /// <summary>
        /// Initializes a new instance of the BeepQuickFilterBar class
        /// </summary>
        public BeepQuickFilterBar()
        {
            IsFrameless = false;
            ShowAllBorders = true;
            ShowShadow = true;
            Height = 50;

            InitializeControls();
            LayoutControls();
            HookEvents();
        }

        private void InitializeControls()
        {
            // Search icon label
            _searchLabel = new BeepLabel
            {
                Text = "🔍",
                Width = 30,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                IsChild = true,
                IsFrameless = true
            };

            // Column selector
            _columnSelector = new BeepComboBox
            {
                Width = 150,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };
            _columnSelector.Items.Add(new SimpleItem { Name = "All Columns", Text = "All Columns" });
            _columnSelector.SelectedIndex = 0;

            // Search text box
            _searchTextBox = new BeepTextBox
            {
                Width = 300,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true,
                PlaceholderText = "Type to filter..."
            };

            // Clear button
            _clearButton = new BeepButton
            {
                Text = "✕ Clear",
                Width = 80,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };

            // Advanced filter button
            _advancedButton = new BeepButton
            {
                Text = "⚙ Advanced",
                Width = 100,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };

            // Filter count label (badge)
            _filterCountLabel = new BeepLabel
            {
                Text = "",
                Width = 80,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                IsChild = true,
                IsFrameless = true,
                Visible = false
            };

            Controls.Add(_searchLabel);
            Controls.Add(_columnSelector);
            Controls.Add(_searchTextBox);
            Controls.Add(_clearButton);
            Controls.Add(_advancedButton);
            Controls.Add(_filterCountLabel);
        }

        private void LayoutControls()
        {
            int margin = 5;
            int x = margin;
            int y = (Height - 30) / 2; // Center vertically

            _searchLabel.Location = new Point(x, y);
            x += _searchLabel.Width + 2;

            _columnSelector.Location = new Point(x, y);
            x += _columnSelector.Width + margin;

            _searchTextBox.Location = new Point(x, y);
            x += _searchTextBox.Width + margin;

            _clearButton.Location = new Point(x, y);
            x += _clearButton.Width + margin;

            _advancedButton.Location = new Point(x, y);
            x += _advancedButton.Width + margin;

            _filterCountLabel.Location = new Point(x, y);
        }

        private void HookEvents()
        {
            _searchTextBox.TextChanged += (s, e) => OnFilterChanged();
            _columnSelector.SelectedIndexChanged += (s, e) => OnFilterChanged();
            _clearButton.Click += (s, e) => ClearFilter();
            _advancedButton.Click += (s, e) => AdvancedFilterRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnFilterChanged()
        {
            var args = new FilterEventArgs
            {
                SearchText = SearchText,
                ColumnName = SelectedColumn == "All Columns" ? null : SelectedColumn
            };
            FilterChanged?.Invoke(this, args);
        }

        private void ClearFilter()
        {
            _searchTextBox.Text = string.Empty;
            _columnSelector.SelectedIndex = 0;
            ActiveFilterCount = 0;
        }

        private void UpdateFilterCountDisplay()
        {
            if (_activeFilterCount > 0)
            {
                _filterCountLabel.Text = $"🔖 {_activeFilterCount} filter{(_activeFilterCount != 1 ? "s" : "")}";
                _filterCountLabel.Visible = true;
            }
            else
            {
                _filterCountLabel.Visible = false;
            }
        }

        /// <summary>
        /// Sets the filter state from a configuration
        /// </summary>
        public void SetFromConfiguration(FilterConfiguration config)
        {
            if (config == null || config.Criteria.Count == 0)
            {
                ClearFilter();
                return;
            }

            // If there's only one criterion with Contains operator, set it in quick filter
            if (config.Criteria.Count == 1)
            {
                var criterion = config.Criteria[0];
                if (criterion.Operator == FilterOperator.Contains)
                {
                    SelectedColumn = criterion.ColumnName;
                    SearchText = criterion.Value?.ToString() ?? string.Empty;
                }
            }

            ActiveFilterCount = config.EnabledFilterCount;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _searchTextBox?.Dispose();
                _clearButton?.Dispose();
                _advancedButton?.Dispose();
                _columnSelector?.Dispose();
                _searchLabel?.Dispose();
                _filterCountLabel?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Event args for filter change events
    /// </summary>
    public class FilterEventArgs : EventArgs
    {
        /// <summary>
        /// The search text entered
        /// </summary>
        public string SearchText { get; set; } = string.Empty;

        /// <summary>
        /// The column to filter, or null for all columns
        /// </summary>
        public string? ColumnName { get; set; }
    }
}
