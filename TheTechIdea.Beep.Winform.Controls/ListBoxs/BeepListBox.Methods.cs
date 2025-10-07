using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Public methods for BeepListBox
    /// </summary>
    public partial class BeepListBox
    {
        #region Item Management
        
        /// <summary>
        /// Adds an item to the list
        /// </summary>
        public void AddItem(SimpleItem item)
        {
            if (item != null)
            {
                _listItems.Add(item);
            }
        }
        
        /// <summary>
        /// Adds multiple items to the list
        /// </summary>
        public void AddItems(IEnumerable<SimpleItem> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    _listItems.Add(item);
                }
            }
        }
        
        /// <summary>
        /// Removes an item from the list
        /// </summary>
        public void RemoveItem(SimpleItem item)
        {
            if (item != null && _listItems.Contains(item))
            {
                _listItems.Remove(item);
                
                // Clean up checkbox if exists
                if (_itemCheckBoxes.ContainsKey(item))
                {
                    _itemCheckBoxes.Remove(item);
                }
                
                // Clear selection if this was the selected item
                if (_selectedItem == item)
                {
                    _selectedItem = null;
                    _selectedIndex = -1;
                }
            }
        }
        
        /// <summary>
        /// Clears all items from the list
        /// </summary>
        public void ClearItems()
        {
            _listItems.Clear();
            _itemCheckBoxes.Clear();
            _selectedItem = null;
            _selectedIndex = -1;
            _hoveredItem = null;
        }
        
        /// <summary>
        /// Refreshes the list display
        /// </summary>
        public void RefreshItems()
        {
            _needsLayoutUpdate = true;
            RequestDelayedInvalidate();
        }
        
        #endregion
        
        #region Selection Management
        
        /// <summary>
        /// Clears the current selection
        /// </summary>
        public void ClearSelection()
        {
            _selectedItem = null;
            _selectedIndex = -1;
            RequestDelayedInvalidate();
        }
        
        /// <summary>
        /// Selects an item by its text
        /// </summary>
        public bool SelectItemByText(string text)
        {
            var item = _helper.FindItemByText(text);
            if (item != null)
            {
                SelectedItem = item;
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Gets the item at the specified point
        /// </summary>
        public SimpleItem GetItemAtPoint(System.Drawing.Point point)
        {
            if (_listBoxPainter == null || _contentAreaRect.IsEmpty)
                return null;
            
            var visibleItems = _helper.GetVisibleItems();
            int itemHeight = _listBoxPainter.GetPreferredItemHeight();
            
            return _helper.GetItemAtPoint(
                point,
                _contentAreaRect,
                itemHeight);
        }
        
        #endregion
        
        #region Checkbox Management
        
        /// <summary>
        /// Toggles the checkbox state for an item
        /// </summary>
        public void ToggleItemCheckbox(SimpleItem item)
        {
            if (item == null || !_showCheckBox) return;
            
            if (!_itemCheckBoxes.ContainsKey(item))
            {
                _itemCheckBoxes[item] = new BeepCheckBoxBool();
            }
            
            var checkbox = _itemCheckBoxes[item];
            checkbox.State = checkbox.State == CheckBoxState.Checked
                ? CheckBoxState.Unchecked
                : CheckBoxState.Checked;
            
            RequestDelayedInvalidate();
        }
        
        /// <summary>
        /// Sets the checkbox state for an item
        /// </summary>
        public void SetItemCheckbox(SimpleItem item, bool isChecked)
        {
            if (item == null || !_showCheckBox) return;
            
            if (!_itemCheckBoxes.ContainsKey(item))
            {
                _itemCheckBoxes[item] = new BeepCheckBoxBool();
            }
            
            _itemCheckBoxes[item].State = isChecked
                ? CheckBoxState.Checked
                : CheckBoxState.Unchecked;
            
            RequestDelayedInvalidate();
        }
        
        /// <summary>
        /// Gets the checkbox state for an item
        /// </summary>
        public bool GetItemCheckbox(SimpleItem item)
        {
            if (item == null || !_itemCheckBoxes.ContainsKey(item))
                return false;
            
            return _itemCheckBoxes[item].State == CheckBoxState.Checked;
        }
        
        /// <summary>
        /// Clears all checkbox selections
        /// </summary>
        public void ClearAllCheckboxes()
        {
            foreach (var checkbox in _itemCheckBoxes.Values)
            {
                checkbox.State = CheckBoxState.Unchecked;
            }
            RequestDelayedInvalidate();
        }
        
        /// <summary>
        /// Checks all checkboxes
        /// </summary>
        public void CheckAllCheckboxes()
        {
            foreach (var item in _listItems)
            {
                if (!_itemCheckBoxes.ContainsKey(item))
                {
                    _itemCheckBoxes[item] = new BeepCheckBoxBool();
                }
                _itemCheckBoxes[item].State = CheckBoxState.Checked;
            }
            RequestDelayedInvalidate();
        }
        
        #endregion
        
        #region Search
        
        /// <summary>
        /// Filters items by the search text
        /// </summary>
        public void FilterByText(string searchText)
        {
            SearchText = searchText;
        }
        
        /// <summary>
        /// Clears the search text
        /// </summary>
        public void ClearSearch()
        {
            SearchText = string.Empty;
        }
        
        /// <summary>
        /// Gets the currently visible items (after search filtering)
        /// </summary>
        public List<SimpleItem> GetVisibleItems()
        {
            return _helper.GetVisibleItems();
        }
        
        #endregion
        
        #region Sorting
        
        /// <summary>
        /// Sorts items alphabetically by text
        /// </summary>
        public void SortItemsByText(bool ascending = true)
        {
            var sortedItems = ascending
                ? _listItems.OrderBy(i => i.Text).ToList()
                : _listItems.OrderByDescending(i => i.Text).ToList();
            
            _listItems.Clear();
            foreach (var item in sortedItems)
            {
                _listItems.Add(item);
            }
        }
        
        #endregion
        
        #region Theme
        
        /// <summary>
        /// Applies the current theme to the control
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            if (_listBoxPainter != null)
            {
                _listBoxPainter.Initialize(this, _currentTheme);
                RequestDelayedInvalidate();
            }
        }
        
        #endregion
        
        #region Scrolling (Placeholder)
        
        /// <summary>
        /// Scrolls to ensure an item is visible
        /// </summary>
        public void EnsureItemVisible(SimpleItem item)
        {
            // TODO: Implement scrolling when item is out of view
            // For now, just refresh
            RequestDelayedInvalidate();
        }
        
        /// <summary>
        /// Scrolls to the top of the list
        /// </summary>
        public void ScrollToTop()
        {
            // TODO: Implement scrolling
            RequestDelayedInvalidate();
        }
        
        /// <summary>
        /// Scrolls to the bottom of the list
        /// </summary>
        public void ScrollToBottom()
        {
            // TODO: Implement scrolling
            RequestDelayedInvalidate();
        }
        
        #endregion
        
        #region Legacy Compatibility Methods
        
        /// <summary>
        /// Calculates the maximum width needed to display all items
        /// </summary>
        public int GetMaxWidth()
        {
            if (_listItems == null || _listItems.Count == 0)
                return Width > 0 ? Width : 100;
            
            if (_listBoxPainter == null)
                return Width;
            
            int maxWidth = 0;
            var visibleItems = _helper.GetVisibleItems();
            
            foreach (var item in visibleItems)
            {
                int itemWidth = 0;
                
                // Checkbox width
                if (_showCheckBox && _listBoxPainter.SupportsCheckboxes())
                {
                    itemWidth += 28; // Checkbox + padding
                }
                
                // Image width
                if (_showImage && !string.IsNullOrEmpty(item.ImagePath))
                {
                    itemWidth += _imageSize + 8; // Image + padding
                }
                
                // Text width
                var textSize = _helper.MeasureText(item.Text ?? string.Empty, _textFont);
                itemWidth += textSize.Width + 16; // Text + padding
                
                maxWidth = Math.Max(maxWidth, itemWidth);
            }
            
            return Math.Max(maxWidth, 100);
        }
        
        /// <summary>
        /// Calculates the maximum height needed to display all items
        /// </summary>
        public int GetMaxHeight()
        {
            if (_listItems == null || _listItems.Count == 0)
                return 50;
            
            int totalHeight = 0;
            
            // Search area height
            if (_showSearch && _listBoxPainter != null && _listBoxPainter.SupportsSearch())
            {
                totalHeight += 40;
            }
            
            // Items height
            var visibleItems = _helper.GetVisibleItems();
            if (_listBoxPainter != null)
            {
                int itemHeight = _listBoxPainter.GetPreferredItemHeight();
                totalHeight += visibleItems.Count * itemHeight;
            }
            else
            {
                totalHeight += visibleItems.Count * _menuItemHeight;
            }
            
            // Padding
            totalHeight += 10;
            
            return Math.Max(totalHeight, 50);
        }
        
        /// <summary>
        /// Initializes the menu (legacy method for compatibility - now handled automatically)
        /// </summary>
        public void InitializeMenu()
        {
            // In the new architecture, this is handled automatically through the painter system
            // Just trigger a refresh
            RefreshItems();
        }
        
        /// <summary>
        /// Filters items by search text (legacy method - use SearchText property or FilterByText instead)
        /// </summary>
        public void Filter(string searchText)
        {
            FilterByText(searchText);
        }
        
        /// <summary>
        /// Sets colors based on theme (legacy method - now handled automatically by painter system)
        /// </summary>
        public void SetColors()
        {
            // Legacy method - colors now handled by painter system and theme
            ApplyTheme();
            RequestDelayedInvalidate();
        }
        
        /// <summary>
        /// Resets the list box to its initial state (legacy method)
        /// </summary>
        public void Reset()
        {
            ClearItems();
            ClearSelection();
            SearchText = string.Empty;
            RequestDelayedInvalidate();
        }
        
        #endregion
    }
}
