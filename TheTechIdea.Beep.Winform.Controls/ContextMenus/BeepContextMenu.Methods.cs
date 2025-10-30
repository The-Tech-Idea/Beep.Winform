using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;


namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    public partial class BeepContextMenu
    {
        #region Public Methods
        
        /// <summary>
        /// Shows the context menu at the specified screen location
        /// </summary>
        public void Show(Point screenLocation)
        {
            Show(screenLocation, null);
        }
        
        /// <summary>
        /// Shows the context menu at the specified screen location relative to an owner control
        /// </summary>
        public void Show(Point screenLocation, Control owner)
        {
            _owner = owner;
            
            // Recalculate size based on current items
            RecalculateSize();
            
            // Adjust position to keep menu on screen
            var screen = Screen.FromPoint(screenLocation);
            var screenBounds = screen.WorkingArea;
            
            int x = screenLocation.X;
            int y = screenLocation.Y;
            
            // Adjust horizontal position
            if (x + Width > screenBounds.Right)
            {
                x = screenBounds.Right - Width;
            }
            if (x < screenBounds.Left)
            {
                x = screenBounds.Left;
            }
            
            // Adjust vertical position
            if (y + Height > screenBounds.Bottom)
            {
                y = screenBounds.Bottom - Height;
            }
            if (y < screenBounds.Top)
            {
                y = screenBounds.Top;
            }
            
            Location = new Point(x, y);
            
            // Show the menu as an owned form of the provided owner (or its parent form)
            var ownerCandidate = owner?.FindForm() ?? owner;
            if (ownerCandidate is System.Windows.Forms.IWin32Window ownerWindow)
            {
                base.Show(ownerWindow);
            }
            else
            {
                base.Show();
            }
            BringToFront();
            try { Activate(); Focus(); } catch { }
        }
        
        /// <summary>
        /// Shows the context menu relative to a control
        /// </summary>
        public void Show(Control control, Point offset)
        {
            if (control == null) return;
            
            var screenLocation = control.PointToScreen(offset);
            Show(screenLocation, control);
        }
        
        /// <summary>
        /// Adds a menu item
        /// </summary>
        public void AddItem(SimpleItem item)
        {
            if (item == null) return;
            _menuItems.Add(item);
        }
        
        /// <summary>
        /// Removes a menu item
        /// </summary>
        public void RemoveItem(SimpleItem item)
        {
            if (item == null) return;
            _menuItems.Remove(item);
        }
        public void ClearItems()
        {
            _menuItems.Clear();
            _selectedItem = null;
            _selectedIndex = -1;
            _selectedItems.Clear();
            _hoveredItem = null;
            _hoveredIndex = -1;
        }
        
        /// <summary>
        /// Gets all selected items (multi-select mode)
        /// </summary>
        public System.Collections.Generic.List<SimpleItem> GetSelectedItems()
        {
            return new System.Collections.Generic.List<SimpleItem>(_selectedItems);
        }
        
        /// <summary>
        /// Clears all selected items (multi-select mode)
        /// </summary>
        public void ClearSelectedItems()
        {
            foreach (var item in _selectedItems)
            {
                item.IsChecked = false;
            }
            _selectedItems.Clear();
            Invalidate();
        }
        
        /// <summary>
        /// Confirms the multi-select and returns selected items
        /// </summary>
        public System.Collections.Generic.List<SimpleItem> ConfirmMultiSelect()
        {
            var result = GetSelectedItems();
            OnItemsSelected();
            Close();
            return result;
        }
        
        /// <summary>
        /// Adds a separator item
        /// </summary>
        public void AddSeparator()
        {
            var separator = new SimpleItem
            {
                DisplayField = "-",
              
                Tag = "separator"
            };
            _menuItems.Add(separator);
        }
        
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Recalculates the form size based on menu items
        /// </summary>
        private void RecalculateSize()
        {
            // Get effective control style for BeepStyling calculations
            var effectiveStyle = GetEffectiveControlStyle();
            
            // Account for BeepStyling padding, border, and shadow
            int beepPadding = BeepStyling.GetPadding(effectiveStyle);
            float beepBorderWidth = BeepStyling.GetBorderThickness(effectiveStyle);
            int beepShadow = StyleShadows.HasShadow(effectiveStyle) ? Math.Max(2, StyleShadows.GetShadowBlur(effectiveStyle) / 2) : 0;
            
            // Calculate total BeepStyling insets (padding + border + shadow on all sides)
            int beepInsets = beepPadding + (int)Math.Ceiling(beepBorderWidth) + beepShadow;
            
            // Calculate minimum height as one item + padding + BeepStyling insets
            int calculatedMinHeight = PreferredItemHeight + 8 + (beepInsets * 2); // One item + top/bottom padding + BeepStyling
            
            if (_menuItems == null || _menuItems.Count == 0)
            {
                Width = _menuWidth + (beepInsets * 2);
                Height = calculatedMinHeight;
                _needsScrolling = false;
                _scrollBar.Visible = false;
                _totalContentHeight = Height;
                _scrollOffset = 0;
                return;
            }
            
            // Calculate required height - add BeepStyling padding
            int totalHeight = 4 + beepInsets; // Top padding + BeepStyling top inset
            
            foreach (var item in _menuItems)
            {
                if (IsSeparator(item))
                {
                    totalHeight += 8; // Separator height
                }
                else
                {
                    totalHeight += PreferredItemHeight;
                }
            }
            
            totalHeight += 4 + beepInsets; // Bottom padding + BeepStyling bottom inset
            
            // Store total content height for scrolling
            _totalContentHeight = totalHeight;
            
            // Calculate required width - add BeepStyling left/right insets
            int maxWidth = _menuWidth + (beepInsets * 2);
            
            using (var g = CreateGraphics())
            {
                foreach (var item in _menuItems)
                {
                    if (IsSeparator(item)) continue;
                    
                    // Calculate text width
                    var textSize = TextRenderer.MeasureText(g, item.DisplayField ?? "", _textFont);
                    int itemWidth =     8; // Left margin
                    
                    if (_showCheckBox)
                    {
                        itemWidth += 20; // Checkbox width
                    }
                    
                    if (_showImage)
                    {
                        itemWidth += _imageSize + 4; // Image + spacing
                    }
                    
                    itemWidth += textSize.Width + 8; // Text + spacing
                    
                    // Shortcut or submenu arrow
                    if (_showShortcuts && !string.IsNullOrEmpty(item.KeyCombination))
                    {
                        var shortcutSize = TextRenderer.MeasureText(g, item.KeyCombination, _shortcutFont);
                        itemWidth += shortcutSize.Width + 16;
                    }
                    else if (item.Children != null && item.Children.Count > 0)
                    {
                        itemWidth += 20; // Submenu arrow
                    }
                    
                    itemWidth += 8; // Right margin
                    
                    if (itemWidth > maxWidth)
                    {
                        maxWidth = itemWidth;
                    }
                }
            }
            
            // Constrain width
            maxWidth = Math.Max(maxWidth, _minWidth);
            maxWidth = Math.Min(maxWidth, _maxWidth);
            
            // Determine if scrolling is needed
            _needsScrolling = _totalContentHeight > _maxHeight;
            
            if (_needsScrolling)
            {
                // Calculate height with scrolling - ensure at least one item is visible
                Height = Math.Min(_totalContentHeight, _maxHeight);
                Height = Math.Max(Height, calculatedMinHeight);
                
                // Add space for scrollbar - already includes beepInsets
                Width = maxWidth + SCROLL_BAR_WIDTH;
                
                // Configure scrollbar
                _scrollBar.Visible = true;
                _scrollBar.Left = maxWidth - (beepInsets * 2); // Adjust for BeepStyling insets
                _scrollBar.Top = 0;
                _scrollBar.Height = Height;
                _scrollBar.Minimum = 0;
                _scrollBar.Maximum = _totalContentHeight - 1;
                _scrollBar.LargeChange = Math.Max(1, Height);
                _scrollBar.SmallChange = PreferredItemHeight;
                
                // Clamp scroll offset to valid range
                int maxScroll = Math.Max(0, _totalContentHeight - Height);
                _scrollOffset = Math.Min(_scrollOffset, maxScroll);
                _scrollBar.Value = Math.Min(_scrollOffset, Math.Max(0, _scrollBar.Maximum - _scrollBar.LargeChange + 1));

                _contentAreaRect = new Rectangle(0, 0, maxWidth , Height);
            }
            else
            {
                // No scrolling needed - width already includes beepInsets
                Width = maxWidth + 10;
                Height = _totalContentHeight;
                _scrollBar.Visible = false;
                _scrollOffset = 0;
                
                _contentAreaRect = new Rectangle(0, 0, Width, Height);
            }
        }
        
        /// <summary>
        /// Shows a submenu for an item
        /// </summary>
        private void ShowSubmenu(SimpleItem parentItem)
        {
            if (parentItem == null || parentItem.Children == null || parentItem.Children.Count == 0)
            {
                return;
            }
            
            // Close any existing submenu
            if (_openSubmenu != null)
            {
                _openSubmenu.Close();
                _openSubmenu.Dispose();
            }
            
            // Create new submenu
            _openSubmenu = new BeepContextMenu();
            
            _openSubmenu.ContextMenuType = this.ContextMenuType;
            _openSubmenu.Theme = this.Theme; // propagate current theme
            _openSubmenu.ShowCheckBox = this.ShowCheckBox;
            _openSubmenu.ShowImage = this.ShowImage;
            _openSubmenu.ShowSeparators = this.ShowSeparators;
            _openSubmenu.ShowShortcuts = this.ShowShortcuts;
            _openSubmenu.MenuItemHeight = this.MenuItemHeight;
            _openSubmenu.ImageSize = this.ImageSize;
            _openSubmenu.Owner = this;
            _openSubmenu.Owner.Tag = parentItem;
            
            // Copy items
            foreach (var child in parentItem.Children)
            {
                _openSubmenu.MenuItems.Add(child);
            }
            
            // Position submenu to the right of parent item
            var itemRect = _layoutHelper.GetItemRect(parentItem);
            var submenuLocation = PointToScreen(new Point(Width, itemRect.Top));
            
            OnSubmenuOpening(parentItem);
            _openSubmenu.Show(submenuLocation, this);
        }
        
        /// <summary>
        /// Checks if an item is a separator
        /// </summary>
        private bool IsSeparator(SimpleItem item)
        {
            return item != null && (item.DisplayField == "-" || item.Tag?.ToString() == "separator");
        }
        
        #endregion
        
        #region Override Methods
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Close any open submenu
            if (_openSubmenu != null)
            {
                _openSubmenu.Close();
                _openSubmenu.Dispose();
                _openSubmenu = null;
            }

            OnMenuClosing(e);

            // If we're not configured to destroy on close, hide instead of disposing
            if (!_destroyOnClose && e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                // Reset transient state so the menu can be shown again cleanly
                _hoveredItem = null;
                _hoveredIndex = -1;
                _submenuTimer.Stop();
                _fadeTimer.Stop();
                Opacity = 1.0;
                Hide();
                return;
            }
            base.OnFormClosing(e);
        }
        // Avoid owner reactivation on close to prevent desktop flicker when opening another menu immediately
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                    SelectPreviousItem();
                    return true;
                    
                case Keys.Down:
                    SelectNextItem();
                    return true;
                    
                case Keys.Enter:
                    if (_hoveredItem != null && _hoveredItem.IsEnabled)
                    {
                        SelectedItem = _hoveredItem;
                        OnItemClicked(_hoveredItem);
                        if (_closeOnItemClick)
                        {
                            Close();
                        }
                    }
                    return true;
                    
                case Keys.Escape:
                    Close();
                    return true;
                    
                case Keys.Right:
                    if (_hoveredItem != null && _hoveredItem.Children != null && _hoveredItem.Children.Count > 0)
                    {
                        ShowSubmenu(_hoveredItem);
                    }
                    return true;
                    
                case Keys.Left:
                    if (Owner is BeepContextMenu parentMenu)
                    {
                        parentMenu.Focus();
                        Close();
                    }
                    return true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        private void SelectPreviousItem()
        {
            if (_menuItems.Count == 0) return;
            
            int index = _hoveredIndex - 1;
            while (index >= 0)
            {
                var item = _menuItems[index];
                if (!IsSeparator(item) && item.IsEnabled)
                {
                    _hoveredItem = item;
                    _hoveredIndex = index;
                    OnItemHovered(item);
                    Invalidate();
                    return;
                }
                index--;
            }
        }
        
        private void SelectNextItem()
        {
            if (_menuItems.Count == 0) return;
            
            int index = _hoveredIndex + 1;
            while (index < _menuItems.Count)
            {
                var item = _menuItems[index];
                if (!IsSeparator(item) && item.IsEnabled)
                {
                    _hoveredItem = item;
                    _hoveredIndex = index;
                    OnItemHovered(item);
                    Invalidate();
                    return;
                }
                index++;
            }
        }
        
        #endregion
    }
}
