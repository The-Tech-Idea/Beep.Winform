using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing filter and sort functionality for BeepSimpleGrid
    /// Handles data filtering, sorting, and related UI interactions
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Filtering

        /// <summary>
        /// Applies a filter to the data based on text and column
        /// </summary>
        private void ApplyFilter(string filterText, string columnName)
        {
            if (_fullData == null || _fullData.Count == 0)
            {
                _filteredData = new List<object>();
                IsFiltered = false;
                OnFilterChanged();
                return;
            }

            if (string.IsNullOrWhiteSpace(filterText))
            {
                _filteredData = new List<object>(_fullData);
                IsFiltered = false;
                OnFilterChanged();
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
                return;
            }

            _filteredData = new List<object>();

            foreach (var item in _fullData)
            {
                var wrapper = item as DataRowWrapper;
                if (wrapper == null || wrapper.OriginalData == null) continue;

                bool matches = false;

                if (string.IsNullOrEmpty(columnName))
                {
                    // Search all columns
                    foreach (var prop in wrapper.OriginalData.GetType().GetProperties())
                    {
                        var value = prop.GetValue(wrapper.OriginalData);
                        if (value != null && value.ToString().IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            matches = true;
                            break;
                        }
                    }
                }
                else
                {
                    // Search specific column
                    var prop = wrapper.OriginalData.GetType().GetProperty(columnName);
                    if (prop != null)
                    {
                        var value = prop.GetValue(wrapper.OriginalData);
                        if (value != null && value.ToString().IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            matches = true;
                        }
                    }
                }

                if (matches)
                {
                    _filteredData.Add(item);
                }
            }

            // Replace _fullData with filtered data temporarily
            var originalFullData = _fullData;
            _fullData = _filteredData;
            IsFiltered = true;

            _dataOffset = 0;
            UpdateIndexTrackingAfterFilterOrSort();
            FillVisibleRows();
            UpdateScrollBars();
            OnFilterChanged();
            Invalidate();
        }

        /// <summary>
        /// Raises the FilterChanged event
        /// </summary>
        protected virtual void OnFilterChanged()
        {
            FilterChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Filter Event Handlers

        /// <summary>
        /// Handles filter text box text changed event
        /// </summary>
        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            string selectedColumn = filterColumnComboBox.SelectedItem is SimpleItem item && item.Value != null
                ? item.Value.ToString()
                : null;
            ApplyFilter(filterTextBox.Text, selectedColumn);
        }

        /// <summary>
        /// Handles filter button click event
        /// </summary>
        private void FilterButton_Click(object sender, MouseEventArgs e)
        {
            ShowFilter = !ShowFilter;
        }

        /// <summary>
        /// Handles filter column combo box selection changed event
        /// </summary>
        private void FilterColumnComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedColumn = filterColumnComboBox.SelectedItem is SimpleItem item && item.Value != null
                ? item.Value.ToString()
                : null;
            if (selectedColumn == null) return;
            if (_editingControl != null) CloseCurrentEditor();
            ApplyFilter(filterTextBox.Text, selectedColumn);
        }

        #endregion

        #region Sorting

        /// <summary>
        /// Handles sort icon click
        /// </summary>
        private void HandleSortIconClick(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= Columns.Count) return;

            var column = Columns[columnIndex];

            // Toggle sort direction
            if (_selectedSortIconIndex == columnIndex)
            {
                // Cycle through: Ascending -> Descending -> None
                SortDirection = SortDirection switch
                {
                    SortDirection.Ascending => SortDirection.Descending,
                    SortDirection.Descending => SortDirection.None,
                    _ => SortDirection.Ascending
                };
            }
            else
            {
                // New column, start with ascending
                SortDirection = SortDirection.Ascending;
                _selectedSortIconIndex = columnIndex;
            }

            if (SortDirection == SortDirection.None)
            {
                ClearSort();
            }
            else
            {
                ApplySorting(column);
            }

            OnSortChanged();
            OnColumnSortClicked(new ColumnSortEventArgs(column, columnIndex, SortDirection));
        }

        /// <summary>
        /// Clears the current sort
        /// </summary>
        public void ClearSort()
        {
            SortDirection = SortDirection.None;
            _selectedSortIconIndex = -1;

            // Restore original order
            if (originalList != null && originalList.Count > 0)
            {
                _fullData = new List<object>(originalList);
                UpdateIndexTrackingAfterFilterOrSort();
                FillVisibleRows();
                UpdateScrollBars();
                OnSortChanged();
                Invalidate();
            }
        }

        /// <summary>
        /// Applies sort and filter operations
        /// </summary>
        private void ApplySortAndFilter()
        {
            // Apply filter first if active
            if (IsFiltered && !string.IsNullOrWhiteSpace(filterTextBox?.Text))
            {
                string selectedColumn = filterColumnComboBox?.SelectedItem is SimpleItem item && item.Value != null
                    ? item.Value.ToString()
                    : null;
                ApplyFilter(filterTextBox.Text, selectedColumn);
            }

            // Then apply sort if active
            if (SortDirection != SortDirection.None && _selectedSortIconIndex >= 0 && _selectedSortIconIndex < Columns.Count)
            {
                ApplySorting(Columns[_selectedSortIconIndex]);
            }
        }

        /// <summary>
        /// Applies sorting to the data based on the specified column
        /// </summary>
        private void ApplySorting(BeepColumnConfig sortColumn)
        {
            if (_fullData == null || _fullData.Count == 0 || sortColumn == null) return;

            var sortedData = new List<object>(_fullData);

            sortedData.Sort((x, y) =>
            {
                var wrapperX = x as DataRowWrapper;
                var wrapperY = y as DataRowWrapper;

                if (wrapperX == null || wrapperY == null) return 0;
                if (wrapperX.OriginalData == null || wrapperY.OriginalData == null) return 0;

                var propX = wrapperX.OriginalData.GetType().GetProperty(sortColumn.ColumnName ?? sortColumn.ColumnCaption);
                var propY = wrapperY.OriginalData.GetType().GetProperty(sortColumn.ColumnName ?? sortColumn.ColumnCaption);

                if (propX == null || propY == null) return 0;

                var valueX = propX.GetValue(wrapperX.OriginalData);
                var valueY = propY.GetValue(wrapperY.OriginalData);

                // Handle null values
                if (valueX == null && valueY == null) return 0;
                if (valueX == null) return SortDirection == SortDirection.Ascending ? -1 : 1;
                if (valueY == null) return SortDirection == SortDirection.Ascending ? 1 : -1;

                // Use SafeComparer for comparison
                var comparer = new SafeComparer();
                int result = comparer.Compare(valueX, valueY);

                return SortDirection == SortDirection.Ascending ? result : -result;
            });

            _fullData = sortedData;
            _dataOffset = 0;

            UpdateIndexTrackingAfterFilterOrSort();
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }

        /// <summary>
        /// Raises the SortChanged event
        /// </summary>
        protected virtual void OnSortChanged()
        {
            SortChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Filter Icon Click Handler

        /// <summary>
        /// Handles filter icon click
        /// </summary>
        private void HandleFilterIconClick(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= Columns.Count) return;

            var column = Columns[columnIndex];
            _selectedFilterIconIndex = columnIndex;

            // Show filter panel and set the selected column
            ShowFilter = true;
            if (filterColumnComboBox != null)
            {
                var item = filterColumnComboBox.ListItems.FirstOrDefault(i =>
                    i.Value?.ToString() == column.ColumnName);
                if (item != null)
                {
                    filterColumnComboBox.SelectedItem = item;
                }
            }

            OnColumnFilterClicked(new ColumnFilterEventArgs(column, columnIndex));
        }

        #endregion

        #region Event Raising

        /// <summary>
        /// Raises the ColumnFilterClicked event
        /// </summary>
        protected virtual void OnColumnFilterClicked(ColumnFilterEventArgs e)
        {
            ColumnFilterClicked?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the ColumnSortClicked event
        /// </summary>
        protected virtual void OnColumnSortClicked(ColumnSortEventArgs e)
        {
            ColumnSortClicked?.Invoke(this, e);
        }

        #endregion

        // Events ColumnFilterClicked and ColumnSortClicked are declared in BeepSimpleGrid.Events.cs

        #region Helper Classes

        /// <summary>
        /// Safe comparer that handles different types
        /// </summary>
        private class SafeComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                // If both are IComparable of the same type
                if (x is IComparable comparableX && x.GetType() == y.GetType())
                {
                    return comparableX.CompareTo(y);
                }

                // Fall back to string comparison
                return string.Compare(x.ToString(), y.ToString(), StringComparison.OrdinalIgnoreCase);
            }
        }

        #endregion
    }

    #region Event Args Classes

    /// <summary>
    /// Event arguments for column filter events
    /// </summary>
    public class ColumnFilterEventArgs : EventArgs
    {
        public BeepColumnConfig Column { get; }
        public int ColumnIndex { get; }

        public ColumnFilterEventArgs(BeepColumnConfig column, int columnIndex)
        {
            Column = column;
            ColumnIndex = columnIndex;
        }
    }

    /// <summary>
    /// Event arguments for column sort events
    /// </summary>
    public class ColumnSortEventArgs : EventArgs
    {
        public BeepColumnConfig Column { get; }
        public int ColumnIndex { get; }
        public SortDirection Direction { get; }

        public ColumnSortEventArgs(BeepColumnConfig column, int columnIndex, SortDirection direction)
        {
            Column = column;
            ColumnIndex = columnIndex;
            Direction = direction;
        }
    }

    #endregion
}
