using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ContextMenus;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    /// <summary>
    /// BaseControl partial class containing context menu functionality
    /// </summary>
    public partial class BaseControl
    {
        #region Context Menu Events

        /// <summary>
        /// Raised when an item is selected from the context menu
        /// </summary>
        public event EventHandler<ContextMenuItemSelectedEventArgs> ContextMenuItemSelected;

        /// <summary>
        /// Raised when multiple items are selected from the context menu (if multi-select is enabled)
        /// </summary>
        public event EventHandler<ContextMenuItemsSelectedEventArgs> ContextMenuItemsSelected;

        #endregion

        #region Show Context Menu Methods

        /// <summary>
        /// Shows a context menu with the specified items at the current mouse position
        /// </summary>
        /// <param name="items">List of menu items to display</param>
        /// <param name="multiSelect">Enable multi-select mode</param>
        /// <returns>The selected item, or null if no selection was made</returns>
        public SimpleItem ShowContextMenu(List<SimpleItem> items, bool multiSelect = false)
        {
            return ShowContextMenu(items, Cursor.Position, multiSelect);
        }

        /// <summary>
        /// Shows a context menu with the specified items at a specific location
        /// </summary>
        /// <param name="items">List of menu items to display</param>
        /// <param name="screenLocation">Screen coordinates where the menu should appear</param>
        /// <param name="multiSelect">Enable multi-select mode</param>
        /// <returns>The selected item, or null if no selection was made</returns>
        public SimpleItem ShowContextMenu(List<SimpleItem> items, Point screenLocation, bool multiSelect = false, FormStyle style = FormStyle.Modern)
        {
            if (items == null || items.Count == 0)
                return null;

            // Use ContextMenuManager for centralized lifecycle management
            var selectedItem = ContextMenus.ContextMenuManager.Show(
                items,
                screenLocation,
                this,
                style,
                multiSelect,
                this.Theme
            );

            // Fire event if item was selected
            if (selectedItem != null)
            {
                OnContextMenuItemSelected(new ContextMenuItemSelectedEventArgs(selectedItem));
            }

            return selectedItem;
        }

        /// <summary>
        /// Shows a context menu in multi-select mode and returns all selected items
        /// </summary>
        /// <param name="items">List of menu items to display</param>
        /// <param name="screenLocation">Screen coordinates where the menu should appear</param>
        /// <returns>List of selected items</returns>
        public List<SimpleItem> ShowContextMenuMultiSelect(List<SimpleItem> items, Point screenLocation)
        {
            if (items == null || items.Count == 0)
                return new List<SimpleItem>();

            // Use ContextMenuManager for centralized lifecycle management
            var selectedItems = ContextMenus.ContextMenuManager.ShowMultiSelect(
                items,
                screenLocation,
                this,
                FormStyle.Modern,
                this.Theme
            );

            // Fire event if items were selected
            if (selectedItems != null && selectedItems.Count > 0)
            {
                OnContextMenuItemsSelected(new ContextMenuItemsSelectedEventArgs(selectedItems));
            }

            return selectedItems ?? new List<SimpleItem>();
        }

        /// <summary>
        /// Shows a context menu in multi-select mode at the current mouse position
        /// </summary>
        /// <param name="items">List of menu items to display</param>
        /// <returns>List of selected items</returns>
        public List<SimpleItem> ShowContextMenuMultiSelect(List<SimpleItem> items)
        {
            return ShowContextMenuMultiSelect(items, Cursor.Position);
        }

        /// <summary>
        /// Shows a context menu with the specified items relative to this control
        /// </summary>
        /// <param name="items">List of menu items to display</param>
        /// <param name="relativeLocation">Location relative to this control</param>
        /// <param name="multiSelect">Enable multi-select mode</param>
        /// <returns>The selected item, or null if no selection was made</returns>
        public SimpleItem ShowContextMenuRelative(List<SimpleItem> items, Point relativeLocation, bool multiSelect = false)
        {
            Point screenLocation = this.PointToScreen(relativeLocation);
            return ShowContextMenu(items, screenLocation, multiSelect);
        }

        /// <summary>
        /// Shows the configured BeepContextMenu at the current mouse position
        /// </summary>
        /// <returns>The selected item, or null if no selection was made</returns>
        public SimpleItem ShowBeepContextMenu()
        {
            if (_beepContextMenu == null || _beepContextMenu.MenuItems.Count == 0)
                return null;

            return ShowBeepContextMenu(Cursor.Position);
        }

        /// <summary>
        /// Shows the configured BeepContextMenu at a specific location
        /// </summary>
        /// <param name="screenLocation">Screen coordinates where the menu should appear</param>
        /// <returns>The selected item, or null if no selection was made</returns>
        public SimpleItem ShowBeepContextMenu(Point screenLocation)
        {
            if (_beepContextMenu == null || _beepContextMenu.MenuItems.Count == 0)
                return null;

            // Convert BeepContextMenu items to list and use ContextMenuManager
            // This ensures consistent lifecycle management and prevents ObjectDisposedException
            var items = _beepContextMenu.MenuItems.ToList();
            var style = _beepContextMenu.ContextMenuType;
            var multiSelect = _beepContextMenu.MultiSelect;

            // Use ContextMenuManager for centralized lifecycle management
            var selectedItem = ContextMenus.ContextMenuManager.Show(
                items,
                screenLocation,
                this,
                style,
                multiSelect,
                this.Theme
            );

            // Fire event if item was selected
            if (selectedItem != null)
            {
                OnContextMenuItemSelected(new ContextMenuItemSelectedEventArgs(selectedItem));
            }

            return selectedItem;
        }


        // Intentionally no owner reactivation here to avoid desktop flicker during rapid menu re-open

        /// <summary>
        /// Shows a context menu on right-click at the click location
        /// Call this from MouseDown or MouseClick event with e.Button == MouseButtons.Right
        /// </summary>
        /// <param name="items">List of menu items to display</param>
        /// <param name="e">Mouse event args from the click event</param>
        /// <param name="multiSelect">Enable multi-select mode</param>
        /// <returns>The selected item, or null if no selection was made</returns>
        public SimpleItem ShowContextMenuOnRightClick(List<SimpleItem> items, MouseEventArgs e, bool multiSelect = false)
        {
            if (e.Button != MouseButtons.Right)
                return null;

            Point screenLocation = this.PointToScreen(e.Location);
            return ShowContextMenu(items, screenLocation, multiSelect);
        }

        /// <summary>
        /// Shows a context menu on right-click in multi-select mode
        /// </summary>
        /// <param name="items">List of menu items to display</param>
        /// <param name="e">Mouse event args from the click event</param>
        /// <returns>List of selected items</returns>
        public List<SimpleItem> ShowContextMenuOnRightClickMultiSelect(List<SimpleItem> items, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return new List<SimpleItem>();

            Point screenLocation = this.PointToScreen(e.Location);
            return ShowContextMenuMultiSelect(items, screenLocation);
        }

        #endregion

        #region Context Menu Configuration

       

    

        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises the ContextMenuItemSelected event
        /// </summary>
        protected virtual void OnContextMenuItemSelected(ContextMenuItemSelectedEventArgs e)
        {
            ContextMenuItemSelected?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the ContextMenuItemsSelected event
        /// </summary>
        protected virtual void OnContextMenuItemsSelected(ContextMenuItemsSelectedEventArgs e)
        {
            ContextMenuItemsSelected?.Invoke(this, e);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a simple menu item
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="imagePath">Optional image path</param>
        /// <param name="tag">Optional tag object</param>
        /// <returns>SimpleItem configured for menu use</returns>
        public static SimpleItem CreateMenuItem(string text, string imagePath = null, object tag = null)
        {
            return new SimpleItem
            {
                DisplayField = text,
                ImagePath = imagePath,
                Tag = tag,
                IsEnabled = true,
                IsChecked = false
            };
        }

        /// <summary>
        /// Creates a menu item with a shortcut
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="shortcut">Shortcut text (e.g., "Ctrl+C")</param>
        /// <param name="imagePath">Optional image path</param>
        /// <param name="tag">Optional tag object</param>
        /// <returns>SimpleItem configured for menu use</returns>
        public static SimpleItem CreateMenuItemWithShortcut(string text, string shortcut, string imagePath = null, object tag = null)
        {
            return new SimpleItem
            {
                DisplayField = text,
                KeyCombination = shortcut,
                ImagePath = imagePath,
                Tag = tag,
                IsEnabled = true,
                IsChecked = false
            };
        }

        /// <summary>
        /// Creates a checkable menu item
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="isChecked">Initial checked state</param>
        /// <param name="imagePath">Optional image path</param>
        /// <param name="tag">Optional tag object</param>
        /// <returns>SimpleItem configured for menu use</returns>
        public static SimpleItem CreateCheckableMenuItem(string text, bool isChecked, string imagePath = null, object tag = null)
        {
            return new SimpleItem
            {
                DisplayField = text,
                ImagePath = imagePath,
                Tag = tag,
                IsEnabled = true,
                IsChecked = isChecked
            };
        }

        /// <summary>
        /// Creates a separator item
        /// </summary>
        /// <returns>SimpleItem configured as separator</returns>
        public static SimpleItem CreateMenuSeparator()
        {
            return new SimpleItem
            {
                DisplayField = "-",
                Tag = "separator"
            };
        }

        /// <summary>
        /// Creates a menu item with sub-items (hierarchical menu)
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="children">List of child menu items</param>
        /// <param name="imagePath">Optional image path</param>
        /// <param name="tag">Optional tag object</param>
        /// <returns>SimpleItem configured with children</returns>
        public static SimpleItem CreateMenuItemWithChildren(string text, List<SimpleItem> children, string imagePath = null, object tag = null)
        {
            return new SimpleItem
            {
                DisplayField = text,
                ImagePath = imagePath,
                Tag = tag,
                IsEnabled = true,
                Children = new BindingList<SimpleItem>(children) ?? new BindingList<SimpleItem>()
            };
        }

        /// <summary>
        /// Creates a menu item with sub-items and a shortcut
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="shortcut">Shortcut text (e.g., "Ctrl+C")</param>
        /// <param name="children">List of child menu items</param>
        /// <param name="imagePath">Optional image path</param>
        /// <param name="tag">Optional tag object</param>
        /// <returns>SimpleItem configured with children and shortcut</returns>
        public static SimpleItem CreateMenuItemWithChildrenAndShortcut(string text, string shortcut, List<SimpleItem> children, string imagePath = null, object tag = null)
        {
            return new SimpleItem
            {
                DisplayField = text,
                KeyCombination = shortcut,
                ImagePath = imagePath,
                Tag = tag,
                IsEnabled = true,
                Children = new BindingList<SimpleItem>(children) ?? new BindingList<SimpleItem>()
            };
        }

        #endregion
    }

    #region Event Args Classes

    /// <summary>
    /// Event arguments for single context menu item selection
    /// </summary>
    public class ContextMenuItemSelectedEventArgs : EventArgs
    {
        public SimpleItem SelectedItem { get; }

        public ContextMenuItemSelectedEventArgs(SimpleItem selectedItem)
        {
            SelectedItem = selectedItem;
        }
    }

    /// <summary>
    /// Event arguments for multiple context menu item selection
    /// </summary>
    public class ContextMenuItemsSelectedEventArgs : EventArgs
    {
        public List<SimpleItem> SelectedItems { get; }

        public ContextMenuItemsSelectedEventArgs(List<SimpleItem> selectedItems)
        {
            SelectedItems = selectedItems ?? new List<SimpleItem>();
        }
    }

    #endregion
}
