using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Chips;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.CombinedControls
{
    /// <summary>
    /// Public methods for BeepChipListBox
    /// </summary>
    public partial class BeepChipListBox
    {
        #region Selection Methods

        /// <summary>
        /// Selects an item in both the list and chip group
        /// </summary>
        /// <param name="item">The item to select</param>
        public void SelectItem(SimpleItem item)
        {
            if (item == null) return;

            if (_allowMultiSelect)
            {
                AddToSelection(item);
            }
            else
            {
                ClearSelection();
                if (_listBox != null)
                {
                    _listBox.SelectedItem = item;
                }
            }
        }

        /// <summary>
        /// Adds an item to the current selection
        /// </summary>
        /// <param name="item">The item to add</param>
        public void AddToSelection(SimpleItem item)
        {
            if (item == null || !_allowMultiSelect) return;

            _listBox?.AddToSelection(item);
            if (_listBox != null && _listBox.ShowCheckBox)
            {
                _listBox.SetItemCheckbox(item, true);
            }
        }

        /// <summary>
        /// Removes an item from the current selection
        /// </summary>
        /// <param name="item">The item to remove</param>
        public void RemoveFromSelection(SimpleItem item)
        {
            if (item == null) return;

            _listBox?.RemoveFromSelection(item);
            if (_listBox != null && _listBox.ShowCheckBox)
            {
                _listBox.SetItemCheckbox(item, false);
            }
        }

        /// <summary>
        /// Clears all selections
        /// </summary>
        public void ClearSelection()
        {
            _listBox?.ClearSelection();
            _chipGroup?.ClearSelection();
        }

        /// <summary>
        /// Selects all items (if multi-select is enabled)
        /// </summary>
        public void SelectAll()
        {
            if (!_allowMultiSelect) return;

            if (_listBox != null)
            {
                foreach (var item in _items)
                {
                    _listBox.AddToSelection(item);
                    if (_listBox.ShowCheckBox)
                    {
                        _listBox.SetItemCheckbox(item, true);
                    }
                }
            }
        }

        /// <summary>
        /// Toggles the selection state of an item
        /// </summary>
        /// <param name="item">The item to toggle</param>
        public void ToggleSelection(SimpleItem item)
        {
            if (item == null) return;

            var isSelected = SelectedItems.Any(i => i.GuidId == item.GuidId);

            if (isSelected)
            {
                RemoveFromSelection(item);
            }
            else
            {
                if (_allowMultiSelect)
                {
                    AddToSelection(item);
                }
                else
                {
                    SelectItem(item);
                }
            }
        }

        /// <summary>
        /// Sets the selection from a list of items
        /// </summary>
        /// <param name="items">The items to select</param>
        public void SetSelection(IEnumerable<SimpleItem> items)
        {
            if (items == null) return;

            ClearSelection();

            foreach (var item in items)
            {
                AddToSelection(item);
            }
        }

        #endregion

        #region Item Management

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="item">The item to add</param>
        public void AddItem(SimpleItem item)
        {
            if (item == null) return;
            _items.Add(item);
        }

        /// <summary>
        /// Adds multiple items to the collection
        /// </summary>
        /// <param name="items">The items to add</param>
        public void AddItems(IEnumerable<SimpleItem> items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                _items.Add(item);
            }
        }

        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the item was removed</returns>
        public bool RemoveItem(SimpleItem item)
        {
            if (item == null) return false;
            return _items.Remove(item);
        }

        /// <summary>
        /// Clears all items from the collection
        /// </summary>
        public void ClearItems()
        {
            _items.Clear();
        }

        /// <summary>
        /// Finds an item by its GuidId
        /// </summary>
        /// <param name="guidId">The GuidId to search for</param>
        /// <returns>The item if found, null otherwise</returns>
        public SimpleItem FindItem(string guidId)
        {
            return _items.FirstOrDefault(i => i.GuidId == guidId);
        }

        /// <summary>
        /// Finds items matching a predicate
        /// </summary>
        /// <param name="predicate">The predicate to match</param>
        /// <returns>List of matching items</returns>
        public List<SimpleItem> FindItems(Func<SimpleItem, bool> predicate)
        {
            return _items.Where(predicate).ToList();
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// Clears the search text
        /// </summary>
        public void ClearSearch()
        {
            if (_searchBox != null)
            {
                _searchBox.Text = "";
            }
        }

        /// <summary>
        /// Focuses the search box
        /// </summary>
        public void FocusSearch()
        {
            if (_showSearch && _searchBox != null)
            {
                _searchBox.Focus();
            }
        }

        #endregion

        #region Synchronization Methods

        /// <summary>
        /// Manually triggers synchronization from list to chips
        /// </summary>
        public void SyncFromListToChips()
        {
            _syncHelper?.SyncFromListBox();
        }

        /// <summary>
        /// Manually triggers synchronization from chips to list
        /// </summary>
        public void SyncFromChipsToList()
        {
            _syncHelper?.SyncFromChipGroup();
        }

        /// <summary>
        /// Enables or disables automatic synchronization
        /// </summary>
        /// <param name="enabled">Whether sync is enabled</param>
        public void SetSyncEnabled(bool enabled)
        {
            if (_syncHelper != null)
            {
                _syncHelper.IsEnabled = enabled;
            }
        }

        /// <summary>
        /// Refreshes the control, re-syncing all data
        /// </summary>
        public void Refresh()
        {
            SyncDataToControls();
            _listBox?.Invalidate();
            _chipGroup?.Invalidate();
            Invalidate();
        }

        #endregion

        #region Data Binding

        public override void SetValue(object value)
        {
            if (value is SimpleItem item)
            {
                SelectItem(item);
            }
            else if (value is IEnumerable<SimpleItem> items)
            {
                SetSelection(items);
            }
            else if (value is string guidId)
            {
                var found = FindItem(guidId);
                if (found != null)
                {
                    SelectItem(found);
                }
            }
        }

        public override object GetValue()
        {
            if (_allowMultiSelect)
            {
                return SelectedItems;
            }
            return SelectedItem;
        }

        public override void ClearValue()
        {
            ClearSelection();
        }

        #endregion

        #region Style Methods

        /// <summary>
        /// Applies a coordinated style preset to both ListBox and ChipGroup
        /// </summary>
        /// <param name="preset">The style preset to apply</param>
        public void ApplyStyle(ChipListBoxStylePreset preset)
        {
            StylePreset = preset;
        }

        /// <summary>
        /// Sets independent styles for ListBox and ChipGroup without coordination
        /// </summary>
        /// <param name="listBoxType">The ListBox style</param>
        /// <param name="chipStyle">The Chip style</param>
        public void SetIndependentStyles(ListBoxType listBoxType, ChipStyle chipStyle)
        {
            var wasCoordinating = _coordinateStyles;
            _coordinateStyles = false;

            try
            {
                ListBoxType = listBoxType;
                ChipStyle = chipStyle;
            }
            finally
            {
                _coordinateStyles = wasCoordinating;
            }
        }

        /// <summary>
        /// Gets the recommended ChipStyle for the current ListBoxType
        /// </summary>
        public ChipStyle GetRecommendedChipStyle()
        {
            return ChipListBoxStyleCoordinator.GetChipStyleFor(_listBoxType);
        }

        /// <summary>
        /// Gets the recommended ListBoxType for the current ChipStyle
        /// </summary>
        public ListBoxType GetRecommendedListBoxType()
        {
            return ChipListBoxStyleCoordinator.GetListBoxTypeFor(_chipStyle);
        }

        /// <summary>
        /// Synchronizes the ChipSize with the ListBox item height
        /// </summary>
        public void SyncChipSizeWithItemHeight()
        {
            if (_listBox != null && _chipGroup != null)
            {
                var recommendedSize = ChipListBoxStyleCoordinator.GetChipSizeForItemHeight(_listBox.MenuItemHeight);
                _chipGroup.ChipSize = recommendedSize;
                _chipSize = recommendedSize;
            }
        }

        /// <summary>
        /// Synchronizes the ListBox item height with the ChipSize
        /// </summary>
        public void SyncItemHeightWithChipSize()
        {
            if (_listBox != null && _chipGroup != null)
            {
                var recommendedHeight = ChipListBoxStyleCoordinator.GetItemHeightForChipSize(_chipSize);
                _listBox.MenuItemHeight = recommendedHeight;
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the count of items
        /// </summary>
        public int ItemCount => _items.Count;

        /// <summary>
        /// Gets the count of selected items
        /// </summary>
        public int SelectedCount => SelectedItems.Count;

        /// <summary>
        /// Checks if an item is selected
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if the item is selected</returns>
        public bool IsSelected(SimpleItem item)
        {
            if (item == null) return false;
            return SelectedItems.Any(i => i.GuidId == item.GuidId);
        }

        /// <summary>
        /// Scrolls the list to make an item visible
        /// </summary>
        /// <param name="item">The item to scroll to</param>
        public void EnsureVisible(SimpleItem item)
        {
            if (item == null || _listBox == null) return;

            var index = _items.IndexOf(item);
            if (index >= 0)
            {
                _listBox.SelectedIndex = index;
            }
        }

        #endregion
    }
}

