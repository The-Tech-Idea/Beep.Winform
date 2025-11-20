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
            if (_selectedItems != null) _selectedItems.Clear();
            _anchorItem = null;
            foreach (var kvp in _itemCheckBoxes)
            {
                kvp.Value.State = CheckBoxState.Unchecked;
            }
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
            bool current = GetItemCheckbox(item);
            SetItemCheckbox(item, !current);
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
            
            // If multi-select is enabled, keep selection state in sync (for consistency)
            if (isChecked && (MultiSelect || SelectionMode == SelectionMode.MultiSimple || SelectionMode == SelectionMode.MultiExtended))
            {
                if (!_selectedItems.Contains(item)) _selectedItems.Add(item);
            }
            else if (!isChecked && (MultiSelect || SelectionMode == SelectionMode.MultiSimple || SelectionMode == SelectionMode.MultiExtended))
            {
                if (_selectedItems.Contains(item)) _selectedItems.Remove(item);
            }
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
        /// Toggle selection for the given item in multi-select mode (or if checkboxes are not used), toggles selection membership
        /// </summary>
        public void ToggleSelection(SimpleItem item)
        {
            if (item == null) return;
            if (SelectionMode == SelectionMode.MultiSimple || SelectionMode == SelectionMode.MultiExtended || MultiSelect)
            {
                if (_selectedItems.Contains(item)) _selectedItems.Remove(item);
                else _selectedItems.Add(item);
                // set anchor and focus
                _anchorItem = item;
                SelectedItem = item;
            }
            else
            {
                if (_selectedItem == item) ClearSelection();
                else SelectedItem = item;
            }
            RequestDelayedInvalidate();
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

            // Apply theme defaults for selection visuals if not explicitly set
            try
            {
                if (_currentTheme != null)
                {
                    if (SelectionBackColor == Color.Empty) SelectionBackColor = _currentTheme.PrimaryColor;
                    if (SelectionBorderColor == Color.Empty) SelectionBorderColor = _currentTheme.AccentColor;
                    if (FocusOutlineColor == Color.Empty) FocusOutlineColor = _currentTheme.PrimaryColor;
                }
            }
            catch { }
        }
        
        #endregion
        
        #region Scrolling (Placeholder)
        
        /// <summary>
        /// Scrolls to ensure an item is visible
        /// </summary>
        public void EnsureItemVisible(SimpleItem item)
        {
            if (item == null) return;
            var layout = _layout.GetCachedLayout();
            if (layout == null || layout.Count == 0) return;

            var info = layout.FirstOrDefault(i => i.Item == item);
            if (info == null) return;

            var clientArea = GetClientArea();
            if (clientArea.IsEmpty) return;

            int itemTopVirtual = info.RowRect.Top + _yOffset; // virtual y of item
            int itemBottomVirtual = itemTopVirtual + info.RowRect.Height;

            if (itemTopVirtual < clientArea.Top)
            {
                _yOffset = itemTopVirtual - clientArea.Top;
            }
            else if (itemBottomVirtual > clientArea.Bottom)
            {
                _yOffset = itemBottomVirtual - clientArea.Bottom;
            }

            // Clamp
            _yOffset = Math.Max(0, Math.Min(_yOffset, Math.Max(0, _virtualSize.Height - clientArea.Height)));
            if (_verticalScrollBar != null && _verticalScrollBar.Visible)
            {
                _verticalScrollBar.Value = _yOffset;
            }
            try { _layoutHelper?.CalculateLayout(); _hitHelper?.RegisterHitAreas(); } catch { }
            Invalidate();
        }

        /// <summary>
        /// Ensures the item at index is visible (based on visible/filtered items set)
        /// </summary>
        public void EnsureIndexVisible(int index)
        {
            var visibleItems = GetVisibleItems();
            if (visibleItems == null || visibleItems.Count == 0) return;
            if (index < 0 || index >= visibleItems.Count) return;
            EnsureItemVisible(visibleItems[index]);
        }
        
        /// <summary>
        /// Scrolls to the top of the list
        /// </summary>
        public void ScrollToTop()
        {
            _yOffset = 0;
            if (_verticalScrollBar != null && _verticalScrollBar.Visible)
            {
                _verticalScrollBar.Value = _yOffset;
            }
            try { _layoutHelper?.CalculateLayout(); _hitHelper?.RegisterHitAreas(); } catch { }
            Invalidate();
        }
        
        /// <summary>
        /// Scrolls to the bottom of the list
        /// </summary>
        public void ScrollToBottom()
        {
            var clientArea = GetClientArea();
            if (clientArea.IsEmpty) return;
            _yOffset = Math.Max(0, _virtualSize.Height - clientArea.Height);
            if (_verticalScrollBar != null && _verticalScrollBar.Visible)
            {
                _verticalScrollBar.Value = _yOffset;
            }
            try { _layoutHelper?.CalculateLayout(); _hitHelper?.RegisterHitAreas(); } catch { }
            Invalidate();
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

        /// <summary>
        /// Select all visible items in multi-select mode
        /// </summary>
        public void SelectAll()
        {
            var visible = GetVisibleItems();
            if (visible == null || visible.Count == 0) return;
            if (SelectionMode == SelectionMode.Single) return;
            if (SelectionMode == SelectionMode.MultiSimple || SelectionMode == SelectionMode.MultiExtended)
            {
                _selectedItems.Clear();
                foreach (var item in visible)
                {
                    _selectedItems.Add(item);
                    if (_showCheckBox) SetItemCheckbox(item, true);
                }
                RequestDelayedInvalidate();
            }
        }
        
        #endregion
    }
}
