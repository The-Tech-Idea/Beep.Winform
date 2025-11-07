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
            
            // Show the menu as an owned form of the provided owner (or its parent form)
            var ownerCandidate = owner?.FindForm() ?? owner;
            if (ownerCandidate is System.Windows.Forms.IWin32Window ownerWindow)
            {
                try
                {
                    // Check if owner is disposed before showing
                    if (ownerCandidate is Control ctrl && (ctrl.IsDisposed || !ctrl.IsHandleCreated))
                    {
                        // Owner is disposed, show without owner
                        base.Show();
                    }
                    else
                    {
                        base.Show(ownerWindow);
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Owner was disposed during show, fall back to showing without owner
                    base.Show();
                }
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
        /// Recalculates the form size based on menu items - FULLY FIXED VERSION
        /// Fixes: 
        /// - Consistent width calculation (no more mixing insets vs content)
        /// - Correct scrollbar positioning 
        /// - Accurate scrolling metrics (no off-by-pixel issues)
        /// - Matches exact drawing logic in DrawMenuItemsSimple
        /// - Eliminates the mysterious "+10" hack
        /// - Forces handle creation BEFORE CreateGraphics() to prevent rare hangs
        /// </summary>
        public void RecalculateSize()
        {
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
            foreach (var item in _menuItems)
            {
                contentHeight += IsSeparator(item) ? 8 : PreferredItemHeight;
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

                    var textSize = TextRenderer.MeasureText(g, item.DisplayField ?? "", _textFont);

                    int itemWidth = 8; // Left margin (matches your draw code)

                    if (_showCheckBox) itemWidth += 20;
                    if (_showImage) itemWidth += _imageSize + 4;
                    itemWidth += textSize.Width + 8;

                    if (_showShortcuts && !string.IsNullOrEmpty(item.KeyCombination))
                    {
                        var shortcutSize = TextRenderer.MeasureText(g, item.KeyCombination, _shortcutFont);
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
                _scrollBar.Minimum = 0;
                _scrollBar.SmallChange = PreferredItemHeight;

                int visibleContentHeight = Height - (beepInsets * 2) - (internalPadding * 2);
                _scrollBar.LargeChange = Math.Max(1, visibleContentHeight);

                // Maximum = total scrollable content + visible - 1 (standard WinForms pattern)
                _scrollBar.Maximum = contentHeight + _scrollBar.LargeChange - 1;

                // Clamp scroll offset
                int maxScroll = Math.Max(0, contentHeight - visibleContentHeight);
                _scrollOffset = Math.Min(_scrollOffset, maxScroll);
                _scrollBar.Value = _scrollOffset;
            }
            else
            {
                _scrollBar.Visible = false;
                _scrollOffset = 0;
            }

            // Update content rectangle for any internal use
            _contentAreaRect = new Rectangle(
                beepInsets + internalPadding,
                beepInsets + internalPadding,
                contentWidth,
                Height - (beepInsets * 2) - (internalPadding * 2));
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
