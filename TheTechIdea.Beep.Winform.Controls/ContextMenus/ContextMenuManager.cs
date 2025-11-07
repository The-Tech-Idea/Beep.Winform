using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    /// <summary>
    /// Centralized manager for BeepContextMenu with full features (sub-menus, tracking, etc.)
    /// All synchronous - no async complexity.
    /// </summary>
    public static class ContextMenuManager
    {
        #region Fields
        
        private static readonly Dictionary<string, MenuContext> _activeMenus = new Dictionary<string, MenuContext>();
        private static readonly Dictionary<string, string> _parentChildRelationships = new Dictionary<string, string>(); // childMenuId -> parentMenuId
        private static readonly object _lock = new object();
        private static int _menuIdCounter = 0;
        
        // Sub-menu tracking
      
        private static System.Windows.Forms.Timer _closeMenuTimer;
        private static string _lastShownChildMenuId;
        
        // Focus loss buffer - prevents immediate close when showing child menu
        private static System.Windows.Forms.Timer _focusLossBufferTimer;
        private static string _menuPendingFocusLossClose;
        
        // Click-outside filter
        private static ClickOutsideFilter _clickOutsideFilter;
        
        #endregion

        #region Properties

        /// <summary>
        /// Enable/disable automatic sub-menu support (default: true)
        /// </summary>
        public static bool EnableSubMenus { get; set; } = true;

        /// <summary>
        /// Gets whether any context menu is currently active
        /// </summary>
        public static bool IsAnyMenuActive
        {
            get
            {
                lock (_lock)
                {
                    return _activeMenus.Count > 0;
                }
            }
        }

        /// <summary>
        /// Gets the count of currently active menus
        /// </summary>
        public static int ActiveMenuCount
        {
            get
            {
                lock (_lock)
                {
                    return _activeMenus.Count;
                }
            }
        }

        #endregion

        #region Internal Classes

        private class MenuContext
        {
            public string Id { get; set; }
            public BeepContextMenu Menu { get; set; }
            public Control Owner { get; set; }
            public SimpleItem SelectedItem { get; set; }
            public List<SimpleItem> SelectedItems { get; set; }
            public bool Closed { get; set; }
            public bool MultiSelect { get; set; }
            public string ParentMenuId { get; set; }
            public List<string> ChildMenuIds { get; set; } = new List<string>();
        }

        /// <summary>
        /// Message filter to detect clicks outside all active menus
        /// </summary>
        private class ClickOutsideFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                const int WM_LBUTTONDOWN = 0x0201;
                const int WM_RBUTTONDOWN = 0x0204;
                const int WM_MBUTTONDOWN = 0x0207;
                const int WM_NCLBUTTONDOWN = 0x00A1;
                const int WM_NCRBUTTONDOWN = 0x00A4;

                // ONLY check for mouse button down events (not move, not up)
                if (m.Msg != WM_LBUTTONDOWN && m.Msg != WM_RBUTTONDOWN && m.Msg != WM_MBUTTONDOWN &&
                    m.Msg != WM_NCLBUTTONDOWN && m.Msg != WM_NCRBUTTONDOWN)
                {
                    return false; // Not a click event, ignore
                }

                try
                {
                    // Check if the click is on a BeepContextMenu window itself
                    var clickedControl = Control.FromHandle(m.HWnd);
                    if (clickedControl is BeepContextMenu)
                    {
                        return false; // Click is on a menu, don't close
                    }

                    // Also check if the clicked control is a child of any menu
                    var parent = clickedControl;
                    while (parent != null)
                    {
                        if (parent is BeepContextMenu)
                        {
                            return false; // Click is inside a menu hierarchy
                        }
                        parent = parent.Parent;
                    }

                    // Get click position in screen coordinates
                    Point screenPos;
                    if (m.Msg == WM_NCLBUTTONDOWN || m.Msg == WM_NCRBUTTONDOWN)
                    {
                        // Non-client area clicks - use cursor position
                        screenPos = Cursor.Position;
                    }
                    else
                    {
                        // Client area clicks need to be converted
                        if (clickedControl != null)
                        {
                            int x = unchecked((short)(long)m.LParam);
                            int y = unchecked((short)((long)m.LParam >> 16));
                            screenPos = clickedControl.PointToScreen(new Point(x, y));
                        }
                        else
                        {
                            screenPos = Cursor.Position;
                        }
                    }

                    // Check if click is inside any active menu bounds
                    lock (_lock)
                    {
                        // Early exit if no menus are active
                        if (_activeMenus.Count == 0)
                        {
                            return false; // No menus to close
                        }
                        
                        bool clickedInsideAnyMenu = false;
                        
                        foreach (var context in _activeMenus.Values)
                        {
                            try
                            {
                                if (context.Menu != null && !context.Menu.IsDisposed && context.Menu.Visible)
                                {
                                    var menuBounds = new Rectangle(context.Menu.Location, context.Menu.Size);
                                    if (menuBounds.Contains(screenPos))
                                    {
                                        clickedInsideAnyMenu = true;
                                        break;
                                    }
                                }
                            }
                            catch (ObjectDisposedException)
                            {
                                // Menu was disposed during check - ignore it
                                continue;
                            }
                        }

                        // If clicked outside all menus, close them all
                        if (!clickedInsideAnyMenu && _activeMenus.Count > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"[ClickOutsideFilter] Click outside detected at {screenPos}, closing {_activeMenus.Count} menu(s)");
                            CloseAllMenus();
                        }
                    }
                }
                catch
                {
                    // Ignore errors in message filter
                }

                return false; // Don't block the message
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows a context menu synchronously
        /// </summary>
        public static SimpleItem Show(
            List<SimpleItem> items,
            Point screenLocation,
            Control owner = null,
            FormStyle style = FormStyle.Modern,
            bool multiSelect = false,
            string theme = null,
            string parentMenuId = null)
        {
            if (items == null || items.Count == 0)
                return null;

            string menuId = null;
            
            lock (_lock)
            {
                // Close existing ROOT menus (not sub-menus)
                if (string.IsNullOrEmpty(parentMenuId))
                {
                    var rootMenus = _activeMenus.Where(kvp => string.IsNullOrEmpty(kvp.Value.ParentMenuId)).Select(kvp => kvp.Key).ToList();
                    foreach (var id in rootMenus)
                    {
                        CloseMenu(id);
                    }
                }

                menuId = GenerateMenuId();
                var context = new MenuContext
                {
                    Id = menuId,
                    MultiSelect = multiSelect,
                    ParentMenuId = parentMenuId
                };

                // Register with parent if this is a sub-menu
                if (!string.IsNullOrEmpty(parentMenuId) && _activeMenus.ContainsKey(parentMenuId))
                {
                    // Close existing sibling sub-menus
                    var siblings = _activeMenus[parentMenuId].ChildMenuIds.ToList();
                    foreach (var siblingId in siblings)
                    {
                        CloseMenu(siblingId);
                    }
                    
                    _activeMenus[parentMenuId].ChildMenuIds.Add(menuId);
                    _parentChildRelationships[menuId] = parentMenuId;
                }

                _activeMenus[menuId] = context;
            }

            try
            {
            // Create menu
            var menu = new BeepContextMenu
            {
                ContextMenuType = style,
                DestroyOnClose = true,
                MultiSelect = multiSelect,
                ShowCheckBox = multiSelect,
                StartPosition = FormStartPosition.Manual,
                CloseOnFocusLost = false // ✅ Disable - use buffer timer + click-outside instead
            };

                if (!string.IsNullOrEmpty(theme))
                {
                    menu.Theme = theme;
                }

                foreach (var item in items)
                {
                    menu.AddItem(item);
                }

                lock (_lock)
                {
                    _activeMenus[menuId].Menu = menu;
                }

                // Hook events
                EventHandler<MenuItemEventArgs> itemClickedHandler = null;
                EventHandler<MenuItemEventArgs> itemHoveredHandler = null;
                FormClosedEventHandler formClosedHandler = null;

                itemClickedHandler = (sender, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ItemClicked: Menu={menuId}, Item={e?.Item?.DisplayField}, HasChildren={HasChildren(e.Item)}");
                    
                    // Don't close if item has children (it's a parent item)
                    if (!HasChildren(e.Item))
                    {
                        lock (_lock)
                        {
                            if (_activeMenus.ContainsKey(menuId))
                            {
                                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ItemClicked: Setting Closed=true for menu {menuId}");
                                _activeMenus[menuId].SelectedItem = e.Item;
                                _activeMenus[menuId].Closed = true;
                                
                                // Close entire hierarchy (parent and all siblings)
                                CloseMenuHierarchy(menuId);
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ItemClicked: Item has children, NOT closing menu {menuId}");
                    }
                };

                // Hook mouse events for auto-close on mouse leave
                EventHandler mouseLeaveHandler = (sender, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] MouseLeave: Menu {menuId}, starting buffer timer");
                    StartFocusLossBuffer(menuId);
                };
                
                EventHandler mouseEnterHandler = (sender, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] MouseEnter: Menu {menuId}, canceling timers");
                    CancelFocusLossBuffer();
                    CancelCloseTimer();
                };
                
                menu.MouseLeave += mouseLeaveHandler;
                menu.MouseEnter += mouseEnterHandler;

                // Hook submenu opening event (triggered when expand icon is clicked)
                EventHandler<MenuItemEventArgs> submenuOpeningHandler = null;
                
                if (EnableSubMenus)
                {
                    submenuOpeningHandler = (sender, e) =>
                    {
                        if (e?.Item == null || !HasChildren(e.Item))
                            return;

                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] SubmenuOpening: Item={e.Item.DisplayField} with {e.Item.Children.Count} children");
                        
                        // Cancel any pending timers
                        CancelFocusLossBuffer();
                        CancelCloseTimer();
                        
                        // Close existing sibling sub-menus
                        CloseAllChildMenus(menuId);
                        
                        var menuLocation = menu.PointToScreen(Point.Empty);
                        var itemIndex = menu.MenuItems.IndexOf(e.Item);
                        var itemBounds = new Rectangle(0, itemIndex * menu.PreferredItemHeight, menu.Width, menu.PreferredItemHeight);
                        var subMenuLocation = CalculateSubMenuPosition(menuLocation, itemBounds);
                        
                        // Show sub-menu
                        try
                        {
                            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Showing child menu at {subMenuLocation}");
                            var childMenuId = ShowChildMenu(e.Item.Children.ToList(), subMenuLocation, menu, style, theme, menuId);
                            _lastShownChildMenuId = childMenuId;
                            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Child menu shown with ID: {childMenuId}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Error showing sub-menu: {ex.Message}");
                        }
                    };
                    
                    menu.SubmenuOpening += submenuOpeningHandler;
                }

                formClosedHandler = (sender, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] FormClosed: Menu {menuId} is closing");
                    
                    lock (_lock)
                    {
                        if (_activeMenus.ContainsKey(menuId))
                        {
                            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] FormClosed: Setting Closed=true for menu {menuId}");
                            _activeMenus[menuId].Closed = true;
                            
                            // Close all child menus when parent closes
                            CloseAllChildMenus(menuId);
                        }
                    }
                    
                    menu.ItemClicked -= itemClickedHandler;
                    if (submenuOpeningHandler != null)
                        menu.SubmenuOpening -= submenuOpeningHandler;
                    if (mouseLeaveHandler != null)
                        menu.MouseLeave -= mouseLeaveHandler;
                    if (mouseEnterHandler != null)
                        menu.MouseEnter -= mouseEnterHandler;
                    menu.FormClosed -= formClosedHandler;
                };

                menu.ItemClicked += itemClickedHandler;
                menu.MouseLeave += mouseLeaveHandler;
                menu.MouseEnter += mouseEnterHandler;
                menu.FormClosed += formClosedHandler;

                // Show menu
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Show: Calling menu.Show() for menu {menuId}, owner={(owner != null ? owner.GetType().Name : "null")}");
                menu.Show(screenLocation, owner);
                
                // Activate and focus the menu
                try
                {
                    menu.Activate();
                    menu.Focus();
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Show: Menu activated and focused");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Show: Failed to activate/focus - {ex.Message}");
                }
                
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Show: menu.Show() returned for menu {menuId}");

                // Install click-outside filter if this is the first ROOT menu
                if (string.IsNullOrEmpty(parentMenuId))
                {
                    lock (_lock)
                    {
                        if (_clickOutsideFilter == null)
                        {
                            _clickOutsideFilter = new ClickOutsideFilter();
                            Application.AddMessageFilter(_clickOutsideFilter);
                            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Click-outside filter installed");
                        }
                    }
                }

                // Wait for menu to close (standard WinForms pattern)
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Show: Entering message loop for menu {menuId}");
                bool menuClosed = false;
                int loopCount = 0;
                while (!menuClosed)
                {
                    lock (_lock)
                    {
                        if (_activeMenus.ContainsKey(menuId))
                        {
                            menuClosed = _activeMenus[menuId].Closed || _activeMenus[menuId].Menu == null || _activeMenus[menuId].Menu.IsDisposed;
                        }
                        else
                        {
                            menuClosed = true;
                        }
                    }

                    if (!menuClosed)
                    {
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(10);
                        loopCount++;
                        if (loopCount % 100 == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Show: Menu {menuId} still open after {loopCount} loops");
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Show: Exiting message loop for menu {menuId} after {loopCount} loops");

                SimpleItem result = null;
                lock (_lock)
                {
                    if (_activeMenus.ContainsKey(menuId))
                    {
                        result = _activeMenus[menuId].SelectedItem;
                    }
                }

                return result;
            }
            finally
            {
                CleanupMenuContext(menuId);
            }
        }

        /// <summary>
        /// Shows a context menu in multi-select mode synchronously
        /// </summary>
        public static List<SimpleItem> ShowMultiSelect(
            List<SimpleItem> items,
            Point screenLocation,
            Control owner = null,
            FormStyle style = FormStyle.Modern,
            string theme = null)
        {
            if (items == null || items.Count == 0)
                return new List<SimpleItem>();

            string menuId = null;
            
            lock (_lock)
            {
                // Close existing menus
                var rootMenus = _activeMenus.Where(kvp => string.IsNullOrEmpty(kvp.Value.ParentMenuId)).Select(kvp => kvp.Key).ToList();
                foreach (var id in rootMenus)
                {
                    CloseMenu(id);
                }

                menuId = GenerateMenuId();
                var context = new MenuContext
                {
                    Id = menuId,
                    MultiSelect = true,
                    SelectedItems = new List<SimpleItem>()
                };

                _activeMenus[menuId] = context;
            }

            try
            {
                var menu = new BeepContextMenu
                {
                    ContextMenuType = style,
                    DestroyOnClose = true,
                    MultiSelect = true,
                    ShowCheckBox = true,
                    StartPosition = FormStartPosition.Manual
                };

                if (!string.IsNullOrEmpty(theme))
                {
                    menu.Theme = theme;
                }

                foreach (var item in items)
                {
                    menu.AddItem(item);
                }

                lock (_lock)
                {
                    _activeMenus[menuId].Menu = menu;
                }

                EventHandler<MenuItemsEventArgs> itemsSelectedHandler = null;
                FormClosedEventHandler formClosedHandler = null;

                itemsSelectedHandler = (sender, e) =>
                {
                    lock (_lock)
                    {
                        if (_activeMenus.ContainsKey(menuId))
                        {
                            _activeMenus[menuId].SelectedItems = e.Items ?? new List<SimpleItem>();
                            _activeMenus[menuId].Closed = true;
                        }
                    }
                };

                formClosedHandler = (sender, e) =>
                {
                    lock (_lock)
                    {
                        if (_activeMenus.ContainsKey(menuId))
                        {
                            if (_activeMenus[menuId].SelectedItems == null || _activeMenus[menuId].SelectedItems.Count == 0)
                            {
                                _activeMenus[menuId].SelectedItems = menu.GetSelectedItems() ?? new List<SimpleItem>();
                            }
                            _activeMenus[menuId].Closed = true;
                        }
                    }
                    
                    menu.ItemsSelected -= itemsSelectedHandler;
                    menu.FormClosed -= formClosedHandler;
                };

                menu.ItemsSelected += itemsSelectedHandler;
                menu.FormClosed += formClosedHandler;

                menu.Show(screenLocation, owner);

                // Install click-outside filter
                lock (_lock)
                {
                    if (_clickOutsideFilter == null)
                    {
                        _clickOutsideFilter = new ClickOutsideFilter();
                        Application.AddMessageFilter(_clickOutsideFilter);
                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Click-outside filter installed for multi-select");
                    }
                }

                // Wait for close
                bool menuClosed = false;
                while (!menuClosed)
                {
                    lock (_lock)
                    {
                        if (_activeMenus.ContainsKey(menuId))
                        {
                            menuClosed = _activeMenus[menuId].Closed || _activeMenus[menuId].Menu == null || _activeMenus[menuId].Menu.IsDisposed;
                        }
                        else
                        {
                            menuClosed = true;
                        }
                    }

                    if (!menuClosed)
                    {
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(10);
                    }
                }

                List<SimpleItem> result = null;
                lock (_lock)
                {
                    if (_activeMenus.ContainsKey(menuId))
                    {
                        result = _activeMenus[menuId].SelectedItems ?? new List<SimpleItem>();
                    }
                }

                return result;
            }
            finally
            {
                CleanupMenuContext(menuId);
            }
        }

        /// <summary>
        /// Closes a specific menu
        /// </summary>
        public static void CloseMenu(string menuId)
        {
            if (string.IsNullOrEmpty(menuId))
                return;

            lock (_lock)
            {
                if (_activeMenus.ContainsKey(menuId))
                {
                    try
                    {
                        var menu = _activeMenus[menuId].Menu;
                        if (menu != null && !menu.IsDisposed)
                        {
                            if (menu.InvokeRequired)
                            {
                                menu.Invoke(new Action(() => menu.Close()));
                            }
                            else
                            {
                                menu.Close();
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Closes all menus
        /// </summary>
        public static void CloseAllMenus()
        {
      lock (_lock)
       {
          var menuIds = _activeMenus.Keys.ToList();
        foreach (var id in menuIds)
       {
    CloseMenu(id);
     }

                // Remove click-outside filter when all menus are closed
    if (_clickOutsideFilter != null)
                {
       try
             {
            Application.RemoveMessageFilter(_clickOutsideFilter);
     System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Click-outside filter removed in CloseAllMenus");
            }
           catch (Exception ex)
         {
        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Error removing filter in CloseAllMenus: {ex.Message}");
       }
          _clickOutsideFilter = null;
          }
             
// Clear all tracking dictionaries
           _activeMenus.Clear();
  _parentChildRelationships.Clear();
         
                // Cancel any pending timers
    CancelFocusLossBuffer();
           CancelCloseTimer();
          }
        }

        /// <summary>
        /// Checks if an item has children
        /// </summary>
        public static bool HasChildren(SimpleItem item)
        {
            return item?.Children != null && item.Children.Count > 0;
        }

        #endregion

        #region Sub-Menu Support

        /// <summary>
        /// Shows a child menu without blocking (returns immediately with menu ID)
        /// </summary>
        private static string ShowChildMenu(
            List<SimpleItem> items,
            Point screenLocation,
            Control owner,
            FormStyle style,
            string theme,
            string parentMenuId)
        {
            if (items == null || items.Count == 0)
                return null;

            string menuId = GenerateMenuId();
            
            lock (_lock)
            {
                var context = new MenuContext
                {
                    Id = menuId,
                    MultiSelect = false,
                    ParentMenuId = parentMenuId
                };

                // Register with parent
                if (!string.IsNullOrEmpty(parentMenuId) && _activeMenus.ContainsKey(parentMenuId))
                {
                    // Close existing sibling sub-menus
                    var siblings = _activeMenus[parentMenuId].ChildMenuIds.ToList();
                    foreach (var siblingId in siblings)
                    {
                        CloseMenu(siblingId);
                    }
                    
                    _activeMenus[parentMenuId].ChildMenuIds.Add(menuId);
                    _parentChildRelationships[menuId] = parentMenuId;
                }

                _activeMenus[menuId] = context;
            }

            // Create menu
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Creating BeepContextMenu for menu {menuId}");
            var menu = new BeepContextMenu
            {
                ContextMenuType = style,
                DestroyOnClose = false, // ✅ CRITICAL: Don't auto-destroy child menus
                MultiSelect = false,
                ShowCheckBox = false,
                StartPosition = FormStartPosition.Manual,
                CloseOnFocusLost = false // ✅ Don't close when parent regains focus
            };
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Menu created, IsDisposed={menu.IsDisposed}");

            if (!string.IsNullOrEmpty(theme))
            {
                menu.Theme = theme;
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Theme set to {theme}");
            }

            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Adding {items.Count} items");
            foreach (var item in items)
            {
                menu.AddItem(item);
            }
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Items added, IsDisposed={menu.IsDisposed}");
            
            // Recalculate size to fit items
            menu.RecalculateSize();
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Size recalculated, Width={menu.Width}, Height={menu.Height}");

            lock (_lock)
            {
                _activeMenus[menuId].Menu = menu;
            }
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Menu stored in _activeMenus");

            // Hook events
            EventHandler<MenuItemEventArgs> itemClickedHandler = null;
            EventHandler<MenuItemEventArgs> submenuOpeningHandler = null;
            EventHandler mouseLeaveHandler = null;
            EventHandler mouseEnterHandler = null;
            FormClosedEventHandler formClosedHandler = null;

            itemClickedHandler = (sender, e) =>
            {
                if (!HasChildren(e.Item))
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Child menu item clicked: {e.Item.DisplayField}");
                    
                    // Propagate the click to the ROOT menu so user code receives it
                    lock (_lock)
                    {
                        if (_activeMenus.ContainsKey(menuId))
                        {
                            _activeMenus[menuId].SelectedItem = e.Item;
                            
                            // Find root menu and set its selected item
                            string rootMenuId = menuId;
                            while (!string.IsNullOrEmpty(rootMenuId) && _activeMenus.ContainsKey(rootMenuId))
                            {
                                var parentId = _activeMenus[rootMenuId].ParentMenuId;
                                if (string.IsNullOrEmpty(parentId))
                                    break;
                                rootMenuId = parentId;
                            }
                            
                            // Set selected item on root menu
                            if (!string.IsNullOrEmpty(rootMenuId) && _activeMenus.ContainsKey(rootMenuId))
                            {
                                _activeMenus[rootMenuId].SelectedItem = e.Item;
                                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Propagated selection to root menu {rootMenuId}");
                            }
                            
                            _activeMenus[menuId].Closed = true;
                            CloseMenuHierarchy(menuId);
                        }
                    }
                }
            };

            // Support nested sub-menus
            submenuOpeningHandler = (sender, e) =>
            {
                if (e?.Item == null || !HasChildren(e.Item))
                    return;

                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Child SubmenuOpening: Item={e.Item.DisplayField}");
                
                // Cancel timers
                CancelFocusLossBuffer();
                CancelCloseTimer();
                
                // Close existing sibling sub-menus
                CloseAllChildMenus(menuId);
                
                var menuLocation = menu.PointToScreen(Point.Empty);
                var itemIndex = menu.MenuItems.IndexOf(e.Item);
                var itemBounds = new Rectangle(0, itemIndex * menu.PreferredItemHeight, menu.Width, menu.PreferredItemHeight);
                var subMenuLocation = CalculateSubMenuPosition(menuLocation, itemBounds);
                
                // Show nested sub-menu
                try
                {
                    var nestedChildMenuId = ShowChildMenu(e.Item.Children.ToList(), subMenuLocation, menu, style, theme, menuId);
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Nested child menu shown with ID: {nestedChildMenuId}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Error showing nested sub-menu: {ex.Message}");
                }
            };

            // Mouse leave - start close sequence
            mouseLeaveHandler = (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Child MouseLeave: Menu {menuId}");
                StartFocusLossBuffer(menuId);
            };

            // Mouse enter - cancel timers
            mouseEnterHandler = (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Child MouseEnter: Menu {menuId}");
                CancelFocusLossBuffer();
                CancelCloseTimer();
            };

            formClosedHandler = (sender, e) =>
            {
                lock (_lock)
                {
                    if (_activeMenus.ContainsKey(menuId))
                    {
                        _activeMenus[menuId].Closed = true;
                        CloseAllChildMenus(menuId);
                    }
                }
                
                menu.ItemClicked -= itemClickedHandler;
                menu.SubmenuOpening -= submenuOpeningHandler;
                menu.MouseLeave -= mouseLeaveHandler;
                menu.MouseEnter -= mouseEnterHandler;
                menu.FormClosed -= formClosedHandler;
                
                CleanupMenuContext(menuId);
                
                // Manually dispose child menu since DestroyOnClose = false
                try
                {
                    if (!menu.IsDisposed)
                    {
                        menu.Dispose();
                    }
                }
                catch { }
            };

            menu.ItemClicked += itemClickedHandler;
            menu.SubmenuOpening += submenuOpeningHandler;
            menu.MouseLeave += mouseLeaveHandler;
            menu.MouseEnter += mouseEnterHandler;
            menu.FormClosed += formClosedHandler;

            // Show menu (non-blocking)
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: About to show menu {menuId} at {screenLocation}");
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Menu state before show - IsDisposed={menu.IsDisposed}, IsHandleCreated={menu.IsHandleCreated}");
            
            try
            {
                // Force handle creation BEFORE calling Show
                if (!menu.IsHandleCreated)
                {
                    var handle = menu.Handle;
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Handle created - {handle}");
                }
                
                // Show without owner to avoid disposal issues
                menu.Location = screenLocation;
                menu.Show();
                
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: menu.Show() completed for menu {menuId}, Visible={menu.Visible}, IsDisposed={menu.IsDisposed}");
                
                // Activate and bring to front
                menu.BringToFront();
                menu.Activate();
                menu.Focus();
                
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Child menu activated and focused");
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: ObjectDisposedException - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Stack trace - {ex.StackTrace}");
                lock (_lock)
                {
                    if (_activeMenus.ContainsKey(menuId))
                    {
                        _activeMenus.Remove(menuId);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Exception showing menu - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Stack trace - {ex.StackTrace}");
                lock (_lock)
                {
                    if (_activeMenus.ContainsKey(menuId))
                    {
                        _activeMenus.Remove(menuId);
                    }
                }
                return null;
            }
            
            return menuId;
        }

        /// <summary>
        /// Starts a buffer timer before starting the close timer
        /// Step 1: Buffer timer (200ms) - gives time for child menu creation
        /// Step 2: Close timer (500ms) - actual close delay
        /// </summary>
        private static void StartFocusLossBuffer(string menuId)
        {
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] StartFocusLossBuffer: Starting 200ms buffer for menu {menuId}");
            
            // Cancel existing timers
            CancelFocusLossBuffer();
            CancelCloseTimer();

            _menuPendingFocusLossClose = menuId;

            // Start buffer timer (200ms delay) - Step 1
            _focusLossBufferTimer = new System.Windows.Forms.Timer();
            _focusLossBufferTimer.Interval = 200;
            _focusLossBufferTimer.Tick += (sender, e) =>
            {
                _focusLossBufferTimer.Stop();
                _focusLossBufferTimer.Dispose();
                _focusLossBufferTimer = null;
                
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Buffer timer expired for menu {_menuPendingFocusLossClose}");
                
                // Check if focus is in any menu
                bool focusInAnyMenu = false;
                lock (_lock)
                {
                    foreach (var context in _activeMenus.Values)
                    {
                        if (context.Menu != null && !context.Menu.IsDisposed && 
                            (context.Menu.Focused || context.Menu.ContainsFocus))
                        {
                            focusInAnyMenu = true;
                            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Focus is in menu {context.Id}, NOT starting close timer");
                            break;
                        }
                    }
                }
                
                // If focus is outside all menus, START close timer (Step 2)
                if (!focusInAnyMenu && !string.IsNullOrEmpty(_menuPendingFocusLossClose))
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Focus is outside all menus, starting CLOSE TIMER");
                    StartCloseTimer(_menuPendingFocusLossClose);
                }
                
                _menuPendingFocusLossClose = null;
            };
            _focusLossBufferTimer.Start();
        }

        /// <summary>
        /// Cancels the focus loss buffer timer
        /// </summary>
        private static void CancelFocusLossBuffer()
        {
            if (_focusLossBufferTimer != null)
            {
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] CancelFocusLossBuffer: Canceling buffer timer");
                _focusLossBufferTimer.Stop();
                _focusLossBufferTimer.Dispose();
                _focusLossBufferTimer = null;
                _menuPendingFocusLossClose = null;
            }
        }

        /// <summary>
        /// Starts the close timer (Step 2 - after buffer)
        /// </summary>
        private static void StartCloseTimer(string menuId)
        {
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] StartCloseTimer: Starting 500ms close timer for menu {menuId}");
            
            // Cancel existing close timer
            CancelCloseTimer();

            // Start close timer (500ms delay)
            _closeMenuTimer = new System.Windows.Forms.Timer();
            _closeMenuTimer.Interval = 500;
            _closeMenuTimer.Tick += (sender, e) =>
            {
                _closeMenuTimer.Stop();
                _closeMenuTimer.Dispose();
                _closeMenuTimer = null;
                
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Close timer expired, closing menu {menuId}");
                
                // Check one more time if focus is in any menu
                bool focusInAnyMenu = false;
                lock (_lock)
                {
                    foreach (var context in _activeMenus.Values)
                    {
                        if (context.Menu != null && !context.Menu.IsDisposed && 
                            (context.Menu.Focused || context.Menu.ContainsFocus))
                        {
                            focusInAnyMenu = true;
                            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Focus returned to menu {context.Id}, NOT closing");
                            break;
                        }
                    }
                }
                
                // If focus is still outside, close the hierarchy
                if (!focusInAnyMenu)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Closing hierarchy from menu {menuId}");
                    CloseMenuHierarchy(menuId);
                }
            };
            _closeMenuTimer.Start();
        }

        /// <summary>
        /// Cancels the close timer
        /// </summary>
        private static void CancelCloseTimer()
        {
            if (_closeMenuTimer != null)
            {
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] CancelCloseTimer: Canceling close timer");
                _closeMenuTimer.Stop();
                _closeMenuTimer.Dispose();
                _closeMenuTimer = null;
            }
        }

        /// <summary>
        /// Calculates the position for a sub-menu
        /// </summary>
        private static Point CalculateSubMenuPosition(Point parentMenuLocation, Rectangle parentItemBounds)
        {
            // Position to the right with 5px overlap
            var x = parentMenuLocation.X + parentItemBounds.Right - 5;
            var y = parentMenuLocation.Y + parentItemBounds.Top;

            // Screen edge detection
            var screen = Screen.FromPoint(new Point(x, y));
            var screenBounds = screen.WorkingArea;
            
            const int estimatedMenuWidth = 250;
            const int estimatedMenuHeight = 300;
            
            // Flip to left if off-screen right
            if (x + estimatedMenuWidth > screenBounds.Right)
            {
                x = parentMenuLocation.X - estimatedMenuWidth + 5;
                if (x < screenBounds.Left)
                    x = screenBounds.Left + 5;
            }
            
            // Adjust vertical if off-screen
            if (y + estimatedMenuHeight > screenBounds.Bottom)
            {
                y = screenBounds.Bottom - estimatedMenuHeight - 5;
                if (y < screenBounds.Top)
                    y = screenBounds.Top + 5;
            }

            return new Point(x, y);
        }

        #endregion

        #region Private Methods

        private static string GenerateMenuId()
        {
            lock (_lock)
            {
                return $"Menu_{++_menuIdCounter}_{DateTime.Now.Ticks}";
            }
        }

        private static void CleanupMenuContext(string menuId)
        {
            if (string.IsNullOrEmpty(menuId))
                return;

            lock (_lock)
            {
                if (_activeMenus.ContainsKey(menuId))
                {
                    // Close all child menus first
                    var childIds = _activeMenus[menuId].ChildMenuIds.ToList();
                    foreach (var childId in childIds)
                    {
                        CloseMenu(childId);
                    }

                    // Remove from parent's child list
                    if (!string.IsNullOrEmpty(_activeMenus[menuId].ParentMenuId) && _activeMenus.ContainsKey(_activeMenus[menuId].ParentMenuId))
                    {
                        _activeMenus[_activeMenus[menuId].ParentMenuId].ChildMenuIds.Remove(menuId);
                    }

                    // Remove from tracking
                    _activeMenus.Remove(menuId);
                    _parentChildRelationships.Remove(menuId);
                }

           // ALWAYS check and remove filter when all menus are closed (moved outside the if block)
if (_activeMenus.Count == 0 && _clickOutsideFilter != null)
 {
         try
       {
         Application.RemoveMessageFilter(_clickOutsideFilter);
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Click-outside filter removed in CleanupMenuContext");
     }
            catch (Exception ex)
        {
        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Error removing filter: {ex.Message}");
           }
 _clickOutsideFilter = null;
    }
            }
        }

        /// <summary>
        /// Closes all child menus of a specific menu
        /// </summary>
        private static void CloseAllChildMenus(string parentMenuId)
        {
            if (string.IsNullOrEmpty(parentMenuId))
                return;

            lock (_lock)
            {
                if (_activeMenus.ContainsKey(parentMenuId))
                {
                    var childIds = _activeMenus[parentMenuId].ChildMenuIds.ToList();
                    foreach (var childId in childIds)
                    {
                        CloseMenu(childId);
                    }
                }
            }
        }

        /// <summary>
        /// Closes the entire menu hierarchy (walks up to root, then closes all)
        /// </summary>
        private static void CloseMenuHierarchy(string menuId)
        {
            if (string.IsNullOrEmpty(menuId))
                return;

            lock (_lock)
            {
                // Find the root menu
                string rootMenuId = menuId;
                while (!string.IsNullOrEmpty(rootMenuId) && _activeMenus.ContainsKey(rootMenuId))
                {
                    var parentId = _activeMenus[rootMenuId].ParentMenuId;
                    if (string.IsNullOrEmpty(parentId))
                        break;
                    rootMenuId = parentId;
                }

                // Close the root menu (which will cascade to all children)
                if (!string.IsNullOrEmpty(rootMenuId))
                {
                    CloseMenu(rootMenuId);
                }
            }
        }

        #endregion
    }
}
