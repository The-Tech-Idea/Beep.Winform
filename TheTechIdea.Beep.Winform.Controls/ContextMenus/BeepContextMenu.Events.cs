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
            var item = _inputHelper.HitTest(e.Location);
            
            if (item != _hoveredItem)
            {
                _hoveredItem = item;
                _hoveredIndex = item != null ? _menuItems.IndexOf(item) : -1;
                
                // Cancel any pending submenu
                _submenuTimer.Stop();
                _submenuPendingItem = null;
                
                // Close any open submenu if we're not hovering over its parent
                if (_openSubmenu != null && item != _openSubmenu.Owner?.Tag as SimpleItem)
                {
                    _openSubmenu.Close();
                    _openSubmenu = null;
                }
                
                // Start submenu timer if item has children
                if (item != null && item.Children != null && item.Children.Count > 0)
                {
                    _submenuPendingItem = item;
                    _submenuTimer.Start();
                }
                
                OnItemHovered(item);
                Invalidate();
            }
        }
        
        private void BeepContextMenu_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            
            var item = _inputHelper.HitTest(e.Location);
            
            if (item != null && item.IsEnabled)
            {
                // Don't close if item has children (submenu)
                if (item.Children != null && item.Children.Count > 0)
                {
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
        }
        
        private void BeepContextMenu_MouseLeave(object sender, EventArgs e)
        {
            // Don't clear hover if mouse moved to submenu
            if (_openSubmenu != null && _openSubmenu.Visible)
            {
                return;
            }
            
            _hoveredItem = null;
            _hoveredIndex = -1;
            _submenuTimer.Stop();
            Invalidate();
        }
        
        private void BeepContextMenu_Deactivate(object sender, EventArgs e)
        {
            // Don't close if submenu is active
            if (_openSubmenu != null && _openSubmenu.Visible)
            {
                return;
            }
            
            if (_closeOnFocusLost)
            {
                Close();
            }
        }
        
        private void BeepContextMenu_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                // Start fade-in animation
                _opacity = 0;
                Opacity = 0;
                _fadeTimer.Start();
            }
        }
        
        private void SubmenuTimer_Tick(object sender, EventArgs e)
        {
            _submenuTimer.Stop();
            
            if (_submenuPendingItem != null && _submenuPendingItem.Children != null && _submenuPendingItem.Children.Count > 0)
            {
                ShowSubmenu(_submenuPendingItem);
            }
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
