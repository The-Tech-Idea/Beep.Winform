using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ContextMenus;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.SideBar;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    /// <summary>
    /// BaseControl partial class containing context menu functionality
    /// </summary>
    public partial class BaseControl
    {
        private FormStyle menustyle = FormStyle.Modern;
        // Keep a reference for async menus to avoid premature GC
        private BeepContextMenu _activeAsyncContextMenu;

        /// <summary>
        /// Gets or sets the theme for styling
        /// </summary>
        [Category("Beep")]
        [Description("The theme for styling the context menu")]
        [Browsable(true)]
        public FormStyle MenuStyle
        {
            get => menustyle;
            set
            {
                if (menustyle != value)
                {
                    menustyle = value;
                    Invalidate();
                }
            }
        }

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
            menustyle = style;
            // Create a fresh context menu per invocation
            var menu = new BeepContextMenu
            {
                ContextMenuType = menustyle,
                DestroyOnClose = true,
                MultiSelect = multiSelect,
                ShowCheckBox = multiSelect,
                Theme = this.Theme
            };

            foreach (var item in items)
            {
                menu.AddItem(item);
            }

            // Track the selected item
            SimpleItem selectedItem = null;
            EventHandler<MenuItemEventArgs> itemClickedHandler = (sender, e) =>
            {
                selectedItem = e.Item;
                OnContextMenuItemSelected(new ContextMenuItemSelectedEventArgs(e.Item));
            };
            menu.ItemClicked += itemClickedHandler;

            try
            {
                menu.Show(screenLocation, this);
                while (menu.Visible)
                {
                    Application.DoEvents();
                }
                return selectedItem;
            }
            finally
            {
                menu.ItemClicked -= itemClickedHandler;
                try { menu.Close(); } catch { }
                try { menu.Dispose(); } catch { }
            }
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

            var menu = new BeepContextMenu
            {
               
                ContextMenuType = FormStyle.Modern,
                DestroyOnClose = true,
                MultiSelect = true,
                ShowCheckBox = true,
                Theme = this.Theme
            };

            foreach (var item in items)
            {
                menu.AddItem(item);
            }

            List<SimpleItem> selectedItems = new List<SimpleItem>();
            EventHandler<MenuItemsEventArgs> itemsSelectedHandler = (sender, e) =>
            {
                selectedItems = e.Items;
                OnContextMenuItemsSelected(new ContextMenuItemsSelectedEventArgs(e.Items));
            };
            menu.ItemsSelected += itemsSelectedHandler;

            try
            {
                menu.Show(screenLocation, this);
                while (menu.Visible)
                {
                    Application.DoEvents();
                }
                return menu.GetSelectedItems();
            }
            finally
            {
                menu.ItemsSelected -= itemsSelectedHandler;
                try { menu.Close(); } catch { }
                try { menu.Dispose(); } catch { }
            }
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
            if (_beepContextMenu == null)
                return null;

            // Track the selected item
            SimpleItem selectedItem = null;

            // Handle item clicked event
            EventHandler<MenuItemEventArgs> itemClickedHandler = (sender, e) =>
            {
                selectedItem = e.Item;
                OnContextMenuItemSelected(new ContextMenuItemSelectedEventArgs(e.Item));
            };

            _beepContextMenu.ItemClicked += itemClickedHandler;

            try
            {
                // Show the menu
                _beepContextMenu.Show(screenLocation, this);

                // Wait for menu to close
                while (_beepContextMenu.Visible)
                {
                    Application.DoEvents();
                }

                return selectedItem;
            }
            finally
            {
                // Clean up event handler
                _beepContextMenu.ItemClicked -= itemClickedHandler;
            }
        }

        /// <summary>
        /// Shows a context menu asynchronously (non-blocking)
        /// </summary>
        /// <param name="items">List of menu items to display</param>
        /// <param name="screenLocation">Screen coordinates where the menu should appear</param>
        public void ShowContextMenuAsync(List<SimpleItem> items, Point screenLocation, FormStyle style  = FormStyle.Modern)
        {
            if (items == null || items.Count == 0)
                return;
            menustyle = style;
            // Create a fresh context menu and keep a reference until it closes
            var menu = new BeepContextMenu
            {
               
                ContextMenuType = FormStyle.Modern,
                DestroyOnClose = true,
                Theme = this.Theme
            };
            foreach (var item in items)
            {
                menu.AddItem(item);
            }
            _activeAsyncContextMenu = menu;

            menu.FormClosed += (s, e) =>
            {
                try { menu.Dispose(); } catch { }
                if (ReferenceEquals(_activeAsyncContextMenu, menu)) _activeAsyncContextMenu = null;
            };

            menu.ItemClicked += (sender, e) =>
            {
                OnContextMenuItemSelected(new ContextMenuItemSelectedEventArgs(e.Item));
            };

            menu.Show(screenLocation, this);
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

        /// <summary>
        /// Configures the context menu with items and Style
        /// </summary>
        /// <param name="items">List of menu items</param>
        /// <param name="menuType">Visual Style of the menu</param>
        /// <param name="multiSelect">Enable multi-select mode</param>
        public void ConfigureContextMenu(List<SimpleItem> items, FormStyle menuType =  FormStyle.Modern, bool multiSelect = false)
        {
            if (_beepContextMenu == null)
            {
                _beepContextMenu = new BeepContextMenu();
               // _beepContextMenu.sty = menustyle;
            }

            _beepContextMenu.ContextMenuType = menuType;
            _beepContextMenu.MultiSelect = multiSelect;
            _beepContextMenu.ShowCheckBox = multiSelect; // Auto-enable checkboxes in multi-select mode
            _beepContextMenu.ClearItems();

            if (items != null)
            {
                foreach (var item in items)
                {
                    _beepContextMenu.AddItem(item);
                }
            }
        }

        /// <summary>
        /// Sets the context menu Style
        /// </summary>
        /// <param name="menuType">Visual Style to apply</param>
        public void SetContextMenuStyle(FormStyle menuType)
        {
            if (_beepContextMenu != null)
            {
                _beepContextMenu.ContextMenuType = menuType;
            }
        }

        /// <summary>
        /// Adds a single item to the context menu
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddContextMenuItem(SimpleItem item)
        {
            if (_beepContextMenu == null)
            {
                _beepContextMenu = new BeepContextMenu();
              
            }

            _beepContextMenu.AddItem(item);
        }

        /// <summary>
        /// Adds a separator to the context menu
        /// </summary>
        public void AddContextMenuSeparator()
        {
            if (_beepContextMenu == null)
            {
                _beepContextMenu = new BeepContextMenu();
                
            }

            _beepContextMenu.AddSeparator();
        }

        /// <summary>
        /// Clears all items from the context menu
        /// </summary>
        public void ClearContextMenuItems()
        {
            if (_beepContextMenu != null)
            {
                _beepContextMenu.ClearItems();
            }
        }

        /// <summary>
        /// Gets the currently configured context menu items
        /// </summary>
        /// <returns>List of menu items</returns>
        public List<SimpleItem> GetContextMenuItems()
        {
            if (_beepContextMenu == null)
                return new List<SimpleItem>();

            return _beepContextMenu.MenuItems.ToList();
        }

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
