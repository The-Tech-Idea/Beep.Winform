using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    public partial class BeepContextMenu
    {
        #region Public Events
        
        /// <summary>
        /// Fired when a menu item is clicked
        /// </summary>
        public event EventHandler<MenuItemEventArgs> ItemClicked;
        
        /// <summary>
        /// Fired when multiple items are selected (multi-select mode)
        /// </summary>
        public event EventHandler<MenuItemsEventArgs> ItemsSelected;
        
        /// <summary>
        /// Fired when the selected item changes
        /// </summary>
        public event EventHandler SelectedItemChanged;
        
        /// <summary>
        /// Fired when a menu item is hovered
        /// </summary>
        public event EventHandler<MenuItemEventArgs> ItemHovered;
        
        /// <summary>
        /// Fired when a submenu is about to open
        /// </summary>
        public event EventHandler<MenuItemEventArgs> SubmenuOpening;
        
        /// <summary>
        /// Fired when the menu is closing
        /// </summary>
        public event EventHandler<FormClosingEventArgs> MenuClosing;
        
        #endregion
        
        #region Protected Event Raisers
        
        protected virtual void OnItemClicked(SimpleItem item)
        {
            ItemClicked?.Invoke(this, new MenuItemEventArgs(item));
        }
        
        protected virtual void OnItemsSelected()
        {
            ItemsSelected?.Invoke(this, new MenuItemsEventArgs(_selectedItems));
        }
        
        protected virtual void OnSelectedItemChanged()
        {
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }
        
        protected virtual void OnItemHovered(SimpleItem item)
        {
            ItemHovered?.Invoke(this, new MenuItemEventArgs(item));
        }
        
        protected virtual void OnSubmenuOpening(SimpleItem item)
        {
            SubmenuOpening?.Invoke(this, new MenuItemEventArgs(item));
        }
        
        protected virtual void OnMenuClosing(FormClosingEventArgs e)
        {
            MenuClosing?.Invoke(this, e);
        }
        
        #endregion
        
        #region Private Event Handlers
        
        private void BeepContextMenu_MouseMove(object sender, MouseEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"[BeepContextMenu] MouseMove: Location={e.Location}, Button={e.Button}");

            //var item = _inputHelper.HitTest(e.Location);

            //if (item != _hoveredItem)
            //{
            //    _hoveredItem = item;
            //    _hoveredIndex = item != null ? _menuItems.IndexOf(item) : -1;

            //    System.Diagnostics.Debug.WriteLine($"[BeepContextMenu] Hover changed: Item={item?.DisplayField ?? "null"}, Index={_hoveredIndex}");

            //    // Fire ItemHovered event - ContextMenuManager will handle sub-menu logic
            //    OnItemHovered(item);
            //    Invalidate();
            //}
            UpdateHoveredItem(GetItemAtPoint(e.Location));
        }
        
        private void BeepContextMenu_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var item = _inputHelper.HitTest(e.Location);

            if (item != null && item.IsEnabled)
            {
                // If item has children (submenu), fire SubmenuOpening event
                if (item.Children != null && item.Children.Count > 0)
                {
                    OnSubmenuOpening(item);
                    return;
                }

                // Handle checkbox
                if (_showCheckBox)
                {
                    var checkRect = _layoutHelper.GetCheckBoxRect(item);
                    if (checkRect.Contains(e.Location))
                    {
                        item.IsChecked = !item.IsChecked;
                        Invalidate();

                        // In multi-select mode, update selected items list
                        if (_multiSelect)
                        {
                            if (item.IsChecked)
                            {
                                if (!_selectedItems.Contains(item))
                                {
                                    _selectedItems.Add(item);
                                }
                            }
                            else
                            {
                                _selectedItems.Remove(item);
                            }
                        }

                        return;
                    }
                }

                // Handle multi-select mode
                if (_multiSelect)
                {
                    // Toggle selection
                    if (_selectedItems.Contains(item))
                    {
                        _selectedItems.Remove(item);
                        item.IsChecked = false;
                    }
                    else
                    {
                        _selectedItems.Add(item);
                        item.IsChecked = true;
                    }

                    _selectedItem = item;
                    OnItemClicked(item);
                    Invalidate();

                    // Don't close in multi-select mode
                    return;
                }

                // Single-select mode
                SelectedItem = item;
                OnItemClicked(item);

                if (_closeOnItemClick)
                {
                    Close();
                }
            }
            //if (e.Button != MouseButtons.Left) return;

            //var item = GetItemAtPoint(e.Location);
            //if (item == null || !item.IsEnabled)
            //{
            //    if (_closeOnItemClick) Close();
            //    return;
            //}

            //if (_multiSelect)
            //{
            //    item.IsChecked = !item.IsChecked;
            //    if (item.IsChecked) _selectedItems.Add(item);
            //    else _selectedItems.Remove(item);
            //    Invalidate();
            //}
            //else
            //{
            //    _selectedItem = item;
            //    _selectedIndex = _menuItems.IndexOf(item);
            //    OnSelectedItemChanged();
            //    OnItemClicked(item);
            //    if (_closeOnItemClick) Close();
            //}
        }
        
        private void BeepContextMenu_MouseLeave(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"[BeepContextMenu] MouseLeave event fired");

            //_hoveredItem = null;
            //_hoveredIndex = -1;

            //// Fire ItemHovered with null to cancel any pending sub-menus
            //OnItemHovered(null);
            //Invalidate();
            UpdateHoveredItem(null);
        }
        
        private void BeepContextMenu_Deactivate(object sender, EventArgs e)
        {
            // NOTE: Deactivation is now handled by BeepMenuManager.ModalMenuFilter
            // This matches WinForms ToolStripManager.ModalMenuFilter pattern
            // The message filter will detect activation changes and close menus appropriately
        }
        
        private void BeepContextMenu_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible || DesignMode) return;

            // Wait for fade-in to finish (if used)
            if (Opacity >= 1.0)
            {
                var clientPos = PointToClient(Cursor.Position);
                if (ClientRectangle.Contains(clientPos))
                {
                    UpdateHoveredItem(GetItemAtPoint(clientPos));
                }
            }
        }
        
        private void SubmenuTimer_Tick(object sender, EventArgs e)
        {
            // NOTE: Sub-menu logic is now handled by ContextMenuManager
            // This timer is kept for backward compatibility but does nothing
            _submenuTimer.Stop();
        }
        
        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            _opacity += 1.0 / FADE_STEPS;
            
            if (_opacity >= 1.0)
            {
                _opacity = 1.0;
                _fadeTimer.Stop();
            }
            
            Opacity = _opacity;
        }
        
        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _scrollOffset = e.NewValue;
            Invalidate();
        }

        private void InternalScrollBarValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender is VScrollBar v)
                {
                    _scrollOffset = v.Value;
                }
                else
                {
                    var valProp = sender.GetType().GetProperty("Value");
                    if (valProp != null)
                    {
                        var vObj = valProp.GetValue(sender);
                        if (vObj is int vi) _scrollOffset = vi;
                    }
                }
            }
            catch { }
            Invalidate();
        }
        
        private void BeepContextMenu_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!_needsScrolling) return;
            
            // Calculate new scroll position
            int delta = e.Delta / 120; // Standard mouse wheel delta
            int smallChange = GetScrollBarSmallChange();
            int scrollAmount = delta * smallChange;
            int newValue = GetScrollBarValue() - scrollAmount;
            
            // Clamp to valid range
            newValue = Math.Max(GetScrollBarMinimum(), Math.Min(newValue, GetScrollBarMaximum() - GetScrollBarLargeChange() + 1));
            
            if (newValue != GetScrollBarValue())
            {
                SetScrollBarValue(newValue);
                _scrollOffset = newValue;
                Invalidate();
            }
        }

        private int GetScrollBarValue()
        {
            if (_scrollBar is VScrollBar v) return v.Value;
            var prop = _scrollBar.GetType().GetProperty("Value");
            if (prop == null) return 0;
            return (int)(prop.GetValue(_scrollBar) ?? 0);
        }

        private void SetScrollBarValue(int newValue)
        {
            if (_scrollBar is VScrollBar v) v.Value = newValue;
            else
            {
                var prop = _scrollBar.GetType().GetProperty("Value");
                if (prop != null) prop.SetValue(_scrollBar, newValue);
            }
        }

        private int GetScrollBarMinimum()
        {
            if (_scrollBar is VScrollBar v) return v.Minimum;
            var prop = _scrollBar.GetType().GetProperty("Minimum");
            if (prop == null) return 0;
            return (int)(prop.GetValue(_scrollBar) ?? 0);
        }

        private int GetScrollBarMaximum()
        {
            if (_scrollBar is VScrollBar v) return v.Maximum;
            var prop = _scrollBar.GetType().GetProperty("Maximum");
            if (prop == null) return 0;
            return (int)(prop.GetValue(_scrollBar) ?? 0);
        }

        private int GetScrollBarLargeChange()
        {
            if (_scrollBar is VScrollBar v) return v.LargeChange;
            var prop = _scrollBar.GetType().GetProperty("LargeChange");
            if (prop == null) return 1;
            return (int)(prop.GetValue(_scrollBar) ?? 1);
        }

        private int GetScrollBarSmallChange()
        {
            if (_scrollBar is VScrollBar v) return v.SmallChange;
            var prop = _scrollBar.GetType().GetProperty("SmallChange");
            if (prop == null) return PreferredItemHeight;
            return (int)(prop.GetValue(_scrollBar) ?? PreferredItemHeight);
        }
        
        #endregion
    }
    
    #region Event Args
    
    /// <summary>
    /// Event arguments for menu item events
    /// </summary>
    public class MenuItemEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        
        public MenuItemEventArgs(SimpleItem item)
        {
            Item = item;
        }
    }
    
    /// <summary>
    /// Event arguments for multi-select menu item events
    /// </summary>
    public class MenuItemsEventArgs : EventArgs
    {
        public List<SimpleItem> Items { get; }
        
        public MenuItemsEventArgs(List<SimpleItem> items)
        {
            Items = items ?? new List<SimpleItem>();
        }
    }
    
    #endregion
}
