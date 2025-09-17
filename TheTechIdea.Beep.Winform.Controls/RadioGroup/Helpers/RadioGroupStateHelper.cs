using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers
{
    /// <summary>
    /// Helper class for managing selection state in BeepRadioGroupAdvanced
    /// </summary>
    internal class RadioGroupStateHelper
    {
        private readonly BaseControl _owner;
        
        private List<SimpleItem> _items = new List<SimpleItem>();
        private HashSet<string> _selectedValues = new HashSet<string>();
        private bool _allowMultipleSelection = false;
        private string _singleSelectedValue = string.Empty;

        public RadioGroupStateHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        #region Properties
        /// <summary>
        /// Gets or sets whether multiple items can be selected simultaneously
        /// </summary>
        public bool AllowMultipleSelection
        {
            get => _allowMultipleSelection;
            set
            {
                if (_allowMultipleSelection != value)
                {
                    _allowMultipleSelection = value;
                    
                    // If switching to single selection, keep only the first selected item
                    if (!_allowMultipleSelection && _selectedValues.Count > 1)
                    {
                        var firstSelected = _selectedValues.First();
                        _selectedValues.Clear();
                        _selectedValues.Add(firstSelected);
                        _singleSelectedValue = firstSelected;
                        
                        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(GetSelectedItems()));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the currently selected value for single selection mode
        /// </summary>
        public string SelectedValue
        {
            get => _allowMultipleSelection ? string.Empty : _singleSelectedValue;
            set
            {
                if (!_allowMultipleSelection)
                {
                    SetSingleSelection(value);
                }
            }
        }

        /// <summary>
        /// Gets the currently selected values for multiple selection mode
        /// </summary>
        public List<string> SelectedValues => new List<string>(_selectedValues);

        /// <summary>
        /// Gets the currently selected items
        /// </summary>
        public List<SimpleItem> SelectedItems => GetSelectedItems();

        /// <summary>
        /// Gets the number of selected items
        /// </summary>
        public int SelectedCount => _selectedValues.Count;

        /// <summary>
        /// Gets whether any items are selected
        /// </summary>
        public bool HasSelection => _selectedValues.Count > 0;
        #endregion

        #region Events
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public event EventHandler<ItemSelectionChangedEventArgs> ItemSelectionChanged;
        #endregion

        #region Setup Methods
        /// <summary>
        /// Updates the items list for state management
        /// </summary>
        public void UpdateItems(List<SimpleItem> items)
        {
            _items = items ?? new List<SimpleItem>();
            
            // Remove any selected values that no longer exist in items
            var validValues = new HashSet<string>(_items.Where(i => !string.IsNullOrEmpty(i.Text)).Select(i => i.Text));
            var invalidValues = _selectedValues.Where(v => !validValues.Contains(v)).ToList();
            
            if (invalidValues.Count > 0)
            {
                foreach (var invalid in invalidValues)
                {
                    _selectedValues.Remove(invalid);
                }
                
                // Update single selection value if it was removed
                if (!_allowMultipleSelection && !_selectedValues.Contains(_singleSelectedValue))
                {
                    _singleSelectedValue = _selectedValues.FirstOrDefault() ?? string.Empty;
                }
                
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(GetSelectedItems()));
            }
        }
        #endregion

        #region Selection Methods
        /// <summary>
        /// Selects an item by index
        /// </summary>
        public bool SelectItem(int index)
        {
            if (index < 0 || index >= _items.Count)
                return false;

            var item = _items[index];
            return SelectItem(item);
        }

        /// <summary>
        /// Selects an item
        /// </summary>
        public bool SelectItem(SimpleItem item)
        {
            if (item == null || string.IsNullOrEmpty(item.Text))
                return false;

            return SelectValue(item.Text);
        }

        /// <summary>
        /// Selects an item by value
        /// </summary>
        public bool SelectValue(string value)
        {
            if (string.IsNullOrEmpty(value) || !_items.Any(i => i.Text == value))
                return false;

            bool wasSelected = _selectedValues.Contains(value);
            bool changed = false;

            if (_allowMultipleSelection)
            {
                if (!wasSelected)
                {
                    _selectedValues.Add(value);
                    changed = true;
                }
            }
            else
            {
                // Single selection mode
                if (_singleSelectedValue != value)
                {
                    _selectedValues.Clear();
                    _selectedValues.Add(value);
                    _singleSelectedValue = value;
                    changed = true;
                }
            }

            if (changed)
            {
                var selectedItem = _items.First(i => i.Text == value);
                ItemSelectionChanged?.Invoke(this, new ItemSelectionChangedEventArgs(selectedItem, true));
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(GetSelectedItems()));
                _owner.Invalidate();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deselects an item by index
        /// </summary>
        public bool DeselectItem(int index)
        {
            if (index < 0 || index >= _items.Count)
                return false;

            var item = _items[index];
            return DeselectItem(item);
        }

        /// <summary>
        /// Deselects an item
        /// </summary>
        public bool DeselectItem(SimpleItem item)
        {
            if (item == null || string.IsNullOrEmpty(item.Text))
                return false;

            return DeselectValue(item.Text);
        }

        /// <summary>
        /// Deselects an item by value
        /// </summary>
        public bool DeselectValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            bool wasSelected = _selectedValues.Contains(value);

            if (wasSelected)
            {
                _selectedValues.Remove(value);

                // Update single selection value
                if (!_allowMultipleSelection)
                {
                    _singleSelectedValue = _selectedValues.FirstOrDefault() ?? string.Empty;
                }

                var deselectedItem = _items.FirstOrDefault(i => i.Text == value);
                if (deselectedItem != null)
                {
                    ItemSelectionChanged?.Invoke(this, new ItemSelectionChangedEventArgs(deselectedItem, false));
                }
                
                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(GetSelectedItems()));
                _owner.Invalidate();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Toggles the selection state of an item
        /// </summary>
        public bool ToggleItem(int index)
        {
            if (index < 0 || index >= _items.Count)
                return false;

            var item = _items[index];
            return ToggleItem(item);
        }

        /// <summary>
        /// Toggles the selection state of an item
        /// </summary>
        public bool ToggleItem(SimpleItem item)
        {
            if (item == null || string.IsNullOrEmpty(item.Text))
                return false;

            return ToggleValue(item.Text);
        }

        /// <summary>
        /// Toggles the selection state of an item by value
        /// </summary>
        public bool ToggleValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            bool isSelected = _selectedValues.Contains(value);

            if (isSelected)
            {
                // In single selection mode, don't allow deselecting the only item
                if (!_allowMultipleSelection && _selectedValues.Count == 1)
                    return false;

                return DeselectValue(value);
            }
            else
            {
                return SelectValue(value);
            }
        }

        /// <summary>
        /// Clears all selections
        /// </summary>
        public void ClearSelection()
        {
            if (_selectedValues.Count > 0)
            {
                var previouslySelected = GetSelectedItems();
                _selectedValues.Clear();
                _singleSelectedValue = string.Empty;

                // Raise events for each deselected item
                foreach (var item in previouslySelected)
                {
                    ItemSelectionChanged?.Invoke(this, new ItemSelectionChangedEventArgs(item, false));
                }

                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(new List<SimpleItem>()));
                _owner.Invalidate();
            }
        }

        /// <summary>
        /// Selects all items (only in multiple selection mode)
        /// </summary>
        public void SelectAll()
        {
            if (!_allowMultipleSelection || _items.Count == 0)
                return;

            var previousCount = _selectedValues.Count;
            var newlySelected = new List<SimpleItem>();

            foreach (var item in _items.Where(i => !string.IsNullOrEmpty(i.Text)))
            {
                if (!_selectedValues.Contains(item.Text))
                {
                    _selectedValues.Add(item.Text);
                    newlySelected.Add(item);
                }
            }

            if (newlySelected.Count > 0)
            {
                foreach (var item in newlySelected)
                {
                    ItemSelectionChanged?.Invoke(this, new ItemSelectionChangedEventArgs(item, true));
                }

                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(GetSelectedItems()));
                _owner.Invalidate();
            }
        }

        /// <summary>
        /// Sets single selection without triggering events (for initialization)
        /// </summary>
        public void SetSingleSelection(string value)
        {
            if (string.IsNullOrEmpty(value) || !_items.Any(i => i.Text == value))
            {
                _selectedValues.Clear();
                _singleSelectedValue = string.Empty;
                return;
            }

            _selectedValues.Clear();
            _selectedValues.Add(value);
            _singleSelectedValue = value;
        }

        /// <summary>
        /// Sets multiple selections without triggering events (for initialization)
        /// </summary>
        public void SetMultipleSelection(IEnumerable<string> values)
        {
            _selectedValues.Clear();
            
            if (values != null)
            {
                var validValues = values.Where(v => !string.IsNullOrEmpty(v) && _items.Any(i => i.Text == v));
                foreach (var value in validValues)
                {
                    _selectedValues.Add(value);
                }
            }

            if (!_allowMultipleSelection)
            {
                _singleSelectedValue = _selectedValues.FirstOrDefault() ?? string.Empty;
                if (_selectedValues.Count > 1)
                {
                    var first = _selectedValues.First();
                    _selectedValues.Clear();
                    _selectedValues.Add(first);
                }
            }
        }
        #endregion

        #region Query Methods
        /// <summary>
        /// Checks if an item is selected
        /// </summary>
        public bool IsSelected(int index)
        {
            if (index < 0 || index >= _items.Count)
                return false;

            var item = _items[index];
            return IsSelected(item);
        }

        /// <summary>
        /// Checks if an item is selected
        /// </summary>
        public bool IsSelected(SimpleItem item)
        {
            if (item == null || string.IsNullOrEmpty(item.Text))
                return false;

            return IsSelected(item.Text);
        }

        /// <summary>
        /// Checks if a value is selected
        /// </summary>
        public bool IsSelected(string value)
        {
            return !string.IsNullOrEmpty(value) && _selectedValues.Contains(value);
        }

        /// <summary>
        /// Gets the index of the first selected item
        /// </summary>
        public int GetFirstSelectedIndex()
        {
            if (_selectedValues.Count == 0)
                return -1;

            var firstValue = _allowMultipleSelection ? _selectedValues.First() : _singleSelectedValue;
            return _items.FindIndex(i => i.Text == firstValue);
        }

        /// <summary>
        /// Gets all selected indices
        /// </summary>
        public List<int> GetSelectedIndices()
        {
            var indices = new List<int>();
            
            for (int i = 0; i < _items.Count; i++)
            {
                if (!string.IsNullOrEmpty(_items[i].Text) && _selectedValues.Contains(_items[i].Text))
                {
                    indices.Add(i);
                }
            }

            return indices;
        }

        private List<SimpleItem> GetSelectedItems()
        {
            return _items.Where(i => !string.IsNullOrEmpty(i.Text) && _selectedValues.Contains(i.Text)).ToList();
        }
        #endregion
    }

    #region Event Args Classes
    public class SelectionChangedEventArgs : EventArgs
    {
        public List<SimpleItem> SelectedItems { get; }

        public SelectionChangedEventArgs(List<SimpleItem> selectedItems)
        {
            SelectedItems = selectedItems ?? new List<SimpleItem>();
        }
    }

    public class ItemSelectionChangedEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public bool IsSelected { get; }

        public ItemSelectionChangedEventArgs(SimpleItem item, bool isSelected)
        {
            Item = item;
            IsSelected = isSelected;
        }
    }
    #endregion
}