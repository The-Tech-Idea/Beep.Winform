using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Helpers;


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

            // ADD THIS LINE - forces handle creation BEFORE RecalculateSize/CreateGraphics
            if (!IsHandleCreated) { var h = Handle; }

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
            
            // CRITICAL FIX: Get the parent form and show as owned TopMost form
            Form parentForm = owner?.FindForm();
            
            if (parentForm != null && !parentForm.IsDisposed && parentForm.IsHandleCreated)
            {
                // Show with parent form as owner
                base.Show(parentForm);
            }
            else
            {
                // No valid parent form, show standalone
                base.Show();
            }
            TopMost = true;
BringToFront();

// IMMEDIATE HOVER: Highlight item under mouse on open
var clientPos = PointToClient(Cursor.Position);
if (ClientRectangle.Contains(clientPos))
{
    UpdateHoveredItem(GetItemAtPoint(clientPos));
}

// Start fade-in animation if enabled
if (_enableAnimations)
{
    _opacity = 0;
    Opacity = 0;
    _fadeTimer.Start();
}
else
{
    _opacity = 1.0;
    Opacity = 1.0;
}

// Keep focus for keyboard
try { Activate(); Focus(); } catch { }
            // Focus search textbox if present
            if (_showSearchBox && _searchTextBox != null)
            {
                try { _searchTextBox.Focus(); } catch { }
            }
            // Ensure TopMost after showing
            TopMost = true;
            BringToFront();
            
            // CRITICAL: Ensure the menu can receive mouse events
            try 
            { 
                Activate();
                Focus();
            } 
            catch
            {
                // Silently handle activation failures
            }
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
            _fullMenuItems.Add(item);
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
            _fullMenuItems.Clear();
            _searchText = string.Empty;
            if (_searchTextBox != null) _searchTextBox.Text = string.Empty;
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
        private void UpdateHoveredItem(SimpleItem newItem)
        {
            if (newItem == _hoveredItem) return;

            _hoveredItem = newItem;
            _hoveredIndex = newItem != null ? _menuItems.IndexOf(newItem) : -1;

            Invalidate(); // Redraw highlight

            // Submenu timer
            _submenuTimer.Stop();
            _submenuPendingItem = null;

            if (newItem != null && newItem.Children?.Count > 0)
            {
                _submenuPendingItem = newItem;
                _submenuTimer.Start();
            }

            OnItemHovered(newItem); // Notify ContextMenuManager
        }
        private SimpleItem GetItemAtPoint(Point clientPoint)
        {
            // 1. Must be inside content area (ignores border/shadow)
            if (!_contentAreaRect.Contains(clientPoint))
                return null;

            // 2. Translate Y to virtual content (add scroll, subtract top padding)
            int relY = clientPoint.Y - _contentAreaRect.Y + _scrollOffset;

            // Top padding area = no item
            if (relY < InternalPadding)
                return null;

            relY -= InternalPadding; // now 0 = start of first item

            // 3. Walk items by cumulative height
            int cumulativeY = 0;
            for (int i = 0; i < _menuItems.Count; i++)
            {
                var item = _menuItems[i];
                int itemHeight = GetItemHeight(item);

                if (relY >= cumulativeY && relY < cumulativeY + itemHeight)
                {
                    return IsSeparator(item) || !item.IsEnabled ? null : item;
                }

                cumulativeY += itemHeight;
            }

            return null; // Bottom padding or empty
        }
        /// <summary>
        /// Recalculates the form size based on menu items - FULLY FIXED VERSION
        /// Fixes: 
        /// - Consistent width calculation (no more mixing insets vs content)
        /// - Correct scrollbar positioning 
        /// - Accurate scrolling metrics (no off-by-pixel issues)
        /// - Matches exact drawing logic in DrawMenuItemsSimple
        /// - Eliminates the mysterious "+10" hack
        /// - Forces handle creation BEFORE CreateGraphics() to prevent rare hangs
        /// - Optimized with dirty flags to avoid unnecessary recalculations
        /// </summary>
        public void RecalculateSize()
        {
            // Check if recalculation is needed
            bool needsRecalc = !_sizeCacheValid ||
                _cachedItemCount != (_menuItems?.Count ?? 0) ||
                _cachedWidth != Width ||
                _cachedMenuItemHeight != _menuItemHeight ||
                _cachedImageSize != _imageSize ||
                _cachedShowCheckBox != _showCheckBox ||
                _cachedShowImage != _showImage ||
                _cachedShowShortcuts != _showShortcuts ||
                _cachedShowSearchBox != _showSearchBox ||
                _cachedSearchBoxHeight != SearchBoxHeight;
            
            if (!needsRecalc)
            {
                return; // Cache is still valid
            }
            
            // Force handle creation FIRST - this prevents CreateGraphics() hangs on some systems
            if (!IsHandleCreated)
            {
                var ensureHandle = Handle; // Forces WinForms to create the window handle
            }

            var effectiveStyle = ControlStyle;

            int beepPadding = BeepStyling.GetPadding(effectiveStyle);
            float beepBorderWidth = BeepStyling.GetBorderThickness(effectiveStyle);
            int beepShadow = StyleShadows.HasShadow(effectiveStyle)
                ? Math.Max(2, StyleShadows.GetShadowBlur(effectiveStyle) / 2)
                : 0;

            int beepInsets = beepPadding + (int)Math.Ceiling(beepBorderWidth) + beepShadow;
            const int internalPadding = 4; // Matches your DrawMenuItemsSimple (4px top/bottom/left/right)
            int searchAreaHeight = _showSearchBox ? (_searchTextBox != null ? _searchTextBox.Height : 40) : 0;

            int calculatedMinHeight = PreferredItemHeight + (internalPadding * 2) + (beepInsets * 2);

            // Handle empty menu
            if (_menuItems == null || _menuItems.Count == 0)
            {
                Width = _menuWidth + (internalPadding * 2) + (beepInsets * 2);
                Height = calculatedMinHeight;
                _needsScrolling = false;
                _scrollBar.Visible = false;
                _totalContentHeight = Height;
                _scrollOffset = 0;
                return;
            }

            // === HEIGHT CALCULATION (internal content only) ===
            int contentHeight = internalPadding; // top
            // include search area if enabled
            if (searchAreaHeight > 0) contentHeight += searchAreaHeight + 8; // 8px spacing after search
            foreach (var item in _menuItems)
            {
                contentHeight += GetItemHeight(item);
            }
            contentHeight += internalPadding; // bottom

            int totalHeight = contentHeight + (beepInsets * 2);
            _totalContentHeight = totalHeight;

            // === WIDTH CALCULATION (exact match to your DrawMenuItemSimple logic) ===
            int requiredContentWidth = 0;

            using (var g = CreateGraphics())
            {
                foreach (var item in _menuItems)
                {
                    if (IsSeparator(item)) continue;

                    SizeF textSizeF = TextUtils.MeasureText(item.DisplayField ?? "", _textFont, int.MaxValue);
                    var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);

                    int itemWidth = 8; // Left margin (matches your draw code)

                    if (_showCheckBox) itemWidth += 20;
                    if (_showImage) itemWidth += _imageSize + 4;
                    itemWidth += textSize.Width + 8;

                    if (_showShortcuts && !string.IsNullOrEmpty(item.KeyCombination))
                    {
                        SizeF shortcutSizeF = TextUtils.MeasureText(item.KeyCombination, _shortcutFont, int.MaxValue);
                        var shortcutSize = new Size((int)shortcutSizeF.Width, (int)shortcutSizeF.Height);
                        itemWidth += shortcutSize.Width + 16;
                    }
                    else if (ContextMenuManager.HasChildren(item)) // Use manager's HasChildren for consistency
                    {
                        itemWidth += 20; // Arrow
                    }

                    itemWidth += 8; // Right margin

                    if (itemWidth > requiredContentWidth)
                        requiredContentWidth = itemWidth;
                }
            }

            // Apply your min/max constraints (these appear to be for the *content* area, not total width)
            int contentWidth = Math.Max(_menuWidth, requiredContentWidth);
            contentWidth = Math.Max(contentWidth, _minWidth);
            contentWidth = Math.Min(contentWidth, _maxWidth);

            // === SCROLLING & FINAL SIZE ===
            _needsScrolling = totalHeight > _maxHeight;

            int menuHeight = _needsScrolling
                ? Math.Max(Math.Min(_maxHeight, totalHeight), calculatedMinHeight)
                : totalHeight;

            int scrollBarWidth = _needsScrolling ? SCROLL_BAR_WIDTH : 0;

            // Final size: insets + internal padding + content + scrollbar
            int menuWidth = (beepInsets * 2) + (internalPadding * 2) + contentWidth + scrollBarWidth;

            Width = menuWidth;
            Height = menuHeight;

            // === SCROLLBAR CONFIG (perfect alignment) ===
            if (_needsScrolling)
            {
                _scrollBar.Visible = true;
                _scrollBar.Left = beepInsets + internalPadding + contentWidth;
                _scrollBar.Top = beepInsets;
                _scrollBar.Height = Height - (beepInsets * 2);

                // Configure properties that may not exist on Control
                if (_scrollBar is VScrollBar vs)
                {
                    vs.Minimum = 0;
                    vs.SmallChange = PreferredItemHeight;
                    int visibleContentHeight = Height - (beepInsets * 2) - (internalPadding * 2);
                    vs.LargeChange = Math.Max(1, visibleContentHeight);
                    vs.Maximum = contentHeight + vs.LargeChange - 1;
                    int maxScroll = Math.Max(0, contentHeight - visibleContentHeight);
                    _scrollOffset = Math.Min(_scrollOffset, maxScroll);
                    vs.Value = _scrollOffset;
                }
                else
                {
                    // Use reflection for BeepScrollBar or other custom scroll implementations
                    try
                    {
                        var t = _scrollBar.GetType();
                        var propMin = t.GetProperty("Minimum");
                        var propMax = t.GetProperty("Maximum");
                        var propSmall = t.GetProperty("SmallChange");
                        var propLarge = t.GetProperty("LargeChange");
                        var propValue = t.GetProperty("Value");
                        if (propMin != null) propMin.SetValue(_scrollBar, 0);
                        if (propSmall != null) propSmall.SetValue(_scrollBar, PreferredItemHeight);
                        int visibleContentHeight = Height - (beepInsets * 2) - (internalPadding * 2);
                        if (propLarge != null) propLarge.SetValue(_scrollBar, Math.Max(1, visibleContentHeight));
                        if (propMax != null && propLarge != null)
                        {
                            int large = (int)propLarge.GetValue(_scrollBar);
                            propMax.SetValue(_scrollBar, contentHeight + large - 1);
                        }
                        int maxScroll = Math.Max(0, contentHeight - visibleContentHeight);
                        _scrollOffset = Math.Min(_scrollOffset, maxScroll);
                        if (propValue != null) propValue.SetValue(_scrollBar, _scrollOffset);
                    }
                    catch { }
                }
            }
            else
            {
                _scrollBar.Visible = false;
                _scrollOffset = 0;
            }

            // Update content rectangle for any internal use
            _contentAreaRect = new Rectangle(
                beepInsets + internalPadding,
                beepInsets + internalPadding + (searchAreaHeight > 0 ? searchAreaHeight + 8 : 0),
                contentWidth,
                Height - (beepInsets * 2) - (internalPadding * 2) - (searchAreaHeight > 0 ? searchAreaHeight + 8 : 0));
            
            // Update cache tracking
            _cachedItemCount = _menuItems?.Count ?? 0;
            _cachedWidth = Width;
            _cachedMenuItemHeight = _menuItemHeight;
            _cachedImageSize = _imageSize;
            _cachedShowCheckBox = _showCheckBox;
            _cachedShowImage = _showImage;
            _cachedShowShortcuts = _showShortcuts;
            _cachedShowSearchBox = _showSearchBox;
            _cachedSearchBoxHeight = SearchBoxHeight;
            _sizeCacheValid = true;
        }
        
        /// <summary>
        /// Invalidates the size cache, forcing next RecalculateSize() to recalculate
        /// </summary>
        private void InvalidateSizeCache()
        {
            _sizeCacheValid = false;
        }

        private int GetItemHeight(SimpleItem item)
        {
            if (IsSeparator(item)) return 8;
            int baseHeight = PreferredItemHeight;
            if (!string.IsNullOrEmpty(item.SubText)) baseHeight += 14; // extra room for subtitle
            return baseHeight;
        }
        /// <summary>
        /// Shows a submenu for an item
        /// NOTE: This is now handled by ContextMenuManager via ItemHovered events
        /// This method is kept for keyboard navigation (Right arrow key)
        /// </summary>
        private void ShowSubmenu(SimpleItem parentItem)
        {
            if (parentItem == null || parentItem.Children == null || parentItem.Children.Count == 0)
            {
                return;
            }
            
            // Trigger the ItemHovered event, which will be caught by ContextMenuManager
            OnItemHovered(parentItem);
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
                _openSubmenu.SetCloseReason(BeepContextMenuCloseReason.AppFocusChange);
                _openSubmenu.Close();
                _openSubmenu.Dispose();
                _openSubmenu = null;
            }

            // Fire MenuClosing event with close reason
            var closingArgs = new BeepContextMenuClosingEventArgs(_closeReason, e.Cancel);
            OnMenuClosing(closingArgs);
            e.Cancel = closingArgs.Cancel;

            // If we're not configured to destroy on close, hide instead of disposing
            if (!_destroyOnClose && e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                // Reset transient state so the menu can be shown again cleanly
                _hoveredItem = null;
                _hoveredIndex = -1;
                _submenuTimer.Stop();
                _fadeTimer.Stop();
                _opacity = 1.0;
                Opacity = 1.0;
                
                // Fire Closed event before hiding
                var closedArgs = new BeepContextMenuClosedEventArgs(_closeReason);
                OnMenuClosed(closedArgs);
                
                Hide();
                return;
            }
            base.OnFormClosing(e);
        }
        // Avoid owner reactivation on close to prevent desktop flicker when opening another menu immediately
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!Enabled) return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                case Keys.Up:
                    SelectPreviousItem();
                    EnsureIndexVisible(_hoveredIndex);
                    return true;
                    
                case Keys.Down:
                    SelectNextItem();
                    EnsureIndexVisible(_hoveredIndex);
                    return true;
                    
                case Keys.Home:
                    if (_menuItems.Count > 0)
                    {
                        SelectFirstItem();
                        EnsureIndexVisible(_hoveredIndex);
                        return true;
                    }
                    break;
                    
                case Keys.End:
                    if (_menuItems.Count > 0)
                    {
                        SelectLastItem();
                        EnsureIndexVisible(_hoveredIndex);
                        return true;
                    }
                    break;
                    
                case Keys.PageUp:
                    if (_menuItems.Count > 0)
                    {
                        SelectPageUp();
                        EnsureIndexVisible(_hoveredIndex);
                        return true;
                    }
                    break;
                    
                case Keys.PageDown:
                    if (_menuItems.Count > 0)
                    {
                        SelectPageDown();
                        EnsureIndexVisible(_hoveredIndex);
                        return true;
                    }
                    break;
                    
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
        
        private void SelectFirstItem()
        {
            for (int i = 0; i < _menuItems.Count; i++)
            {
                var item = _menuItems[i];
                if (!IsSeparator(item) && item.IsEnabled)
                {
                    _hoveredItem = item;
                    _hoveredIndex = i;
                    OnItemHovered(item);
                    Invalidate();
                    return;
                }
            }
        }
        
        private void SelectLastItem()
        {
            for (int i = _menuItems.Count - 1; i >= 0; i--)
            {
                var item = _menuItems[i];
                if (!IsSeparator(item) && item.IsEnabled)
                {
                    _hoveredItem = item;
                    _hoveredIndex = i;
                    OnItemHovered(item);
                    Invalidate();
                    return;
                }
            }
        }
        
        private void SelectPageUp()
        {
            if (_menuItems.Count == 0) return;
            
            int pageSize = Math.Max(1, _menuItems.Count / 10); // 10% of list
            int targetIndex = Math.Max(0, _hoveredIndex - pageSize);
            
            for (int i = targetIndex; i >= 0; i--)
            {
                var item = _menuItems[i];
                if (!IsSeparator(item) && item.IsEnabled)
                {
                    _hoveredItem = item;
                    _hoveredIndex = i;
                    OnItemHovered(item);
                    Invalidate();
                    return;
                }
            }
        }
        
        private void SelectPageDown()
        {
            if (_menuItems.Count == 0) return;
            
            int pageSize = Math.Max(1, _menuItems.Count / 10); // 10% of list
            int targetIndex = Math.Min(_menuItems.Count - 1, _hoveredIndex + pageSize);
            
            for (int i = targetIndex; i < _menuItems.Count; i++)
            {
                var item = _menuItems[i];
                if (!IsSeparator(item) && item.IsEnabled)
                {
                    _hoveredItem = item;
                    _hoveredIndex = i;
                    OnItemHovered(item);
                    Invalidate();
                    return;
                }
            }
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

        /// <summary>
        /// Ensures the provided index is visible in the current scroll viewport.
        /// Adjusts scrollbar value to bring item into view if necessary.
        /// </summary>
        private void EnsureIndexVisible(int index)
        {
            if (!_needsScrolling) return;
            if (index < 0 || index >= _menuItems.Count) return;

            int beepPadding = BeepStyling.GetPadding(_controlStyle);
            float beepBorderWidth = BeepStyling.GetBorderThickness(_controlStyle);
            int beepShadow = StyleShadows.HasShadow(ControlStyle) ? Math.Max(2, StyleShadows.GetShadowBlur(ControlStyle) / 2) : 0;
            int beepInsets = beepPadding + (int)Math.Ceiling(beepBorderWidth) + beepShadow;
            const int internalPadding = 4;
            int searchAreaHeight = _showSearchBox ? (_searchTextBox != null ? _searchTextBox.Height : 40) : 0;

            int visibleContentHeight = Height - (beepInsets * 2) - (internalPadding * 2) - (searchAreaHeight > 0 ? searchAreaHeight + 8 : 0);

            // Compute top of item relative to content beginning
            int cumulativeY = internalPadding;
            if (searchAreaHeight > 0) cumulativeY += searchAreaHeight + 8;
            for (int i = 0; i < index; i++) cumulativeY += GetItemHeight(_menuItems[i]);

            int itemTop = cumulativeY;
            int itemBottom = itemTop + GetItemHeight(_menuItems[index]);

            int currentScroll = GetScrollBarValue();

            if (itemTop < currentScroll)
            {
                currentScroll = itemTop;
            }
            else if (itemBottom > currentScroll + visibleContentHeight)
            {
                currentScroll = itemBottom - visibleContentHeight;
            }

            // Clamp and apply
            int min = GetScrollBarMinimum();
            int max = Math.Max(0, GetScrollBarMaximum() - GetScrollBarLargeChange() + 1);
            currentScroll = Math.Max(min, Math.Min(currentScroll, max));
            if (currentScroll != GetScrollBarValue())
            {
                SetScrollBarValue(currentScroll);
            }
            _scrollOffset = currentScroll;
            Invalidate();
        }
        
        #endregion
    }
}
