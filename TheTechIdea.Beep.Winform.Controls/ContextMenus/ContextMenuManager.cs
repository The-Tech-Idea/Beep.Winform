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
        private static readonly Dictionary<string, string> _parentChildRelationships = new Dictionary<string, string>();
        private static readonly object _lock = new object();
        private static int _menuIdCounter = 0;
        private static readonly Dictionary<string, bool> _menusHovered = new Dictionary<string, bool>();
        private static System.Windows.Forms.Timer _bufferTimer;
        private static System.Windows.Forms.Timer _closeTimer;
        private static ClickOutsideFilter _clickOutsideFilter;
        // Global re-entrancy protection
        private static bool _isGlobalClosing = false;
        #endregion
        #region Properties
        public static bool EnableSubMenus { get; set; } = true;
        public static bool IsAnyMenuActive { get { lock (_lock) return _activeMenus.Count > 0; } }
        public static int ActiveMenuCount { get { lock (_lock) return _activeMenus.Count; } }
        #endregion
        #region Internal Classes
        private class MenuContext
        {
            public string Id { get; set; }
            public BeepContextMenu Menu { get; set; }
            public Control Owner { get; set; }
            public SimpleItem SelectedItem { get; set; }
            public List<SimpleItem> SelectedItems { get; set; }
            public bool Closed = false;
            public bool MultiSelect = false;
            public string ParentMenuId { get; set; }
            public List<string> ChildMenuIds { get; set; } = new List<string>();
            public bool CloseInitiated = false; // **NEW**: Set BEFORE closing starts
        }
        private class ClickOutsideFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                const int WM_LBUTTONDOWN = 0x0201;
                const int WM_RBUTTONDOWN = 0x0204;
                const int WM_MBUTTONDOWN = 0x0207;
                const int WM_NCLBUTTONDOWN = 0x00A1;
                const int WM_NCRBUTTONDOWN = 0x00A4;
                if (m.Msg != WM_LBUTTONDOWN && m.Msg != WM_RBUTTONDOWN && m.Msg != WM_MBUTTONDOWN &&
                    m.Msg != WM_NCLBUTTONDOWN && m.Msg != WM_NCRBUTTONDOWN)
                {
                    return false;
                }
                try
                {
                    var clickedControl = Control.FromHandle(m.HWnd);
                    if (clickedControl is BeepContextMenu)
                        return false;
                    var parent = clickedControl;
                    while (parent != null)
                    {
                        if (parent is BeepContextMenu)
                            return false;
                        parent = parent.Parent;
                    }
                    Point screenPos;
                    if (m.Msg == WM_NCLBUTTONDOWN || m.Msg == WM_NCRBUTTONDOWN)
                    {
                        screenPos = Cursor.Position;
                    }
                    else
                    {
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
                    lock (_lock)
                    {
                        if (_isGlobalClosing || _activeMenus.Count == 0)
                            return false;
                        bool clickedInsideAnyMenu = false;
                        foreach (var context in _activeMenus.Values)
                        {
                            try
                            {
                                if (context.Menu != null && !context.Menu.IsDisposed && context.Menu.Visible && !context.CloseInitiated)
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
                                continue;
                            }
                        }
                        if (!clickedInsideAnyMenu)
                        {
                            System.Diagnostics.Debug.WriteLine($"[ClickOutsideFilter] Click outside detected at {screenPos}, closing {_activeMenus.Count} menu(s)");
                            CloseAllMenus();
                        }
                    }
                }
                catch { }
                return false;
            }
        }
        #endregion
        #region Public Methods
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
                if (_isGlobalClosing)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Show: Aborting - global closing in progress");
                    return null;
                }
                if (string.IsNullOrEmpty(parentMenuId))
                {
                    // Close existing root menus and use BeepMenuManager to switch
                    var rootMenus = _activeMenus.Where(kvp => string.IsNullOrEmpty(kvp.Value.ParentMenuId)).Select(kvp => kvp.Key).ToList();
                    foreach (var id in rootMenus)
                    {
                        CloseMenu(id);
                    }
                    
                    // Wait a moment for closing to complete
                    System.Threading.Thread.Sleep(10);
                }
                menuId = GenerateMenuId();
                var context = new MenuContext
                {
                    Id = menuId,
                    MultiSelect = multiSelect,
                    ParentMenuId = parentMenuId
                };
                if (!string.IsNullOrEmpty(parentMenuId) && _activeMenus.ContainsKey(parentMenuId))
                {
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
                var menu = new BeepContextMenu
                {
                    ContextMenuType = style,
                    DestroyOnClose = true,
                    MultiSelect = multiSelect,
                    ShowCheckBox = multiSelect,
                    StartPosition = FormStartPosition.Manual,
                    CloseOnFocusLost = false
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
                    // Do NOT set _menusHovered here�calculate after show
                }

                // **BULLETPROOF MOUSE TRACKING**
                Timer mousePoller = new Timer { Interval = 50 };
                bool wasHovered=false; // Declare here but set after show
                mousePoller.Tick += (s, e) =>
                {
                    // **CRITICAL**: Check if menu is closing/disposed FIRST
                    if (menu.IsDisposed || !menu.Visible)
                    {
                        mousePoller.Stop();
                        mousePoller.Dispose();
                        return;
                    }
                    // **CRITICAL**: Check if we're in a closing state
                    lock (_lock)
                    {
                        if (!_activeMenus.TryGetValue(menuId, out var context) || context.CloseInitiated)
                        {
                            mousePoller.Stop();
                            mousePoller.Dispose();
                            return;
                        }
                    }
                    Point mousePos = Cursor.Position;
                    bool isHovered = menu.Bounds.Contains(mousePos);
                    if (wasHovered && !isHovered)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] MOUSE LEAVE DETECTED for menu {menuId}");
                        lock (_lock)
                        {
                            _menusHovered[menuId] = false;
                        }
                        StartBufferTimer();
                        wasHovered = false;
                    }
                    else if (!wasHovered && isHovered)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] MOUSE ENTER DETECTED for menu {menuId}");
                        lock (_lock)
                        {
                            _menusHovered[menuId] = true;
                        }
                        StopAllTimers();
                        wasHovered = true;
                    }
                };

                // (Existing itemClickedHandler, submenuOpeningHandler, formClosedHandler definitions remain unchanged)
                EventHandler<MenuItemEventArgs> itemClickedHandler = null;
                EventHandler<MenuItemEventArgs> submenuOpeningHandler = null;
                FormClosedEventHandler formClosedHandler = null;
                itemClickedHandler = (sender, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ItemClicked: Menu={menuId}, Item={e?.Item?.DisplayField}, HasChildren={HasChildren(e.Item)}");
                    if (!HasChildren(e.Item))
                    {
                        lock (_lock)
                        {
                            if (_activeMenus.ContainsKey(menuId))
                            {
                                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ItemClicked: Setting Closed=true for menu {menuId}");
                                _activeMenus[menuId].SelectedItem = e.Item;
                                _activeMenus[menuId].Closed = true;
                                CloseMenuHierarchy(menuId);
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ItemClicked: Item has children, NOT closing menu {menuId}");
                    }
                };
                submenuOpeningHandler = (sender, e) =>
                {
                    if (e?.Item == null || !HasChildren(e.Item))
                        return;
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] SubmenuOpening: Item={e.Item.DisplayField} with {e.Item.Children.Count} children");
                    StopAllTimers();
                    CloseAllChildMenus(menuId);
                    var menuLocation = menu.PointToScreen(Point.Empty);
                    var itemIndex = menu.MenuItems.IndexOf(e.Item);
                    var itemBounds = new Rectangle(0, itemIndex * menu.PreferredItemHeight, menu.Width, menu.PreferredItemHeight);
                    var subMenuLocation = CalculateSubMenuPosition(menuLocation, itemBounds);
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Showing child menu at {subMenuLocation}");
                        var childMenuId = ShowChildMenu(e.Item.Children.ToList(), subMenuLocation, menu, style, theme, menuId);
                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Child menu shown with ID: {childMenuId}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Error showing sub-menu: {ex.Message}");
                    }
                };
                formClosedHandler = (sender, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] FormClosed: Menu {menuId} is closing");
                    mousePoller.Stop();
                    mousePoller.Dispose();
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] MousePoller STOPPED for menu {menuId}");
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
                    menu.FormClosed -= formClosedHandler;
                };
                menu.ItemClicked += itemClickedHandler;
                menu.SubmenuOpening += submenuOpeningHandler;
                menu.FormClosed += formClosedHandler;

                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Show: Calling menu.Show() for menu {menuId}, owner={(owner != null ? owner.GetType().Name : "null")}");
                // Show the menu FIRST
                menu.Show(screenLocation, owner);
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

                // NOW calculate initial hover state (bounds are reliable post-show)
                Point mousePos = Cursor.Position;
                bool initialHovered = false;
                try
                {
                    initialHovered = menu.Bounds.Contains(mousePos);
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Show: Initial hover for {menuId}: {initialHovered} at {mousePos}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Show: Error checking initial hover - {ex.Message}");
                }
                lock (_lock)
                {
                    if (_activeMenus.ContainsKey(menuId))
                    {
                        _menusHovered[menuId] = initialHovered;
                    }
                }
                wasHovered = initialHovered;

                // NOW start the poller
                mousePoller.Start();
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] MousePoller STARTED for menu {menuId}");

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
                    // Do NOT set _menusHovered here�calculate after show
                }

                Timer mousePoller = new Timer { Interval = 50 };
                bool wasHovered=false; // Declare here but set after show
                mousePoller.Tick += (s, e) =>
                {
                    if (menu.IsDisposed || !menu.Visible)
                    {
                        mousePoller.Stop();
                        mousePoller.Dispose();
                        return;
                    }
                    lock (_lock)
                    {
                        if (!_activeMenus.TryGetValue(menuId, out var context) || context.CloseInitiated)
                        {
                            mousePoller.Stop();
                            mousePoller.Dispose();
                            return;
                        }
                    }
                    Point mousePos = Cursor.Position;
                    bool isHovered = menu.Bounds.Contains(mousePos);
                    if (wasHovered && !isHovered)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] MultiSelect MOUSE LEAVE DETECTED for menu {menuId}");
                        lock (_lock)
                        {
                            _menusHovered[menuId] = false;
                        }
                        StartBufferTimer();
                        wasHovered = false;
                    }
                    else if (!wasHovered && isHovered)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] MultiSelect MOUSE ENTER DETECTED for menu {menuId}");
                        lock (_lock)
                        {
                            _menusHovered[menuId] = true;
                        }
                        StopAllTimers();
                        wasHovered = true;
                    }
                };

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
                    mousePoller.Stop();
                    mousePoller.Dispose();
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

                // NOW calculate initial hover state
                Point mousePos = Cursor.Position;
                bool initialHovered = false;
                try
                {
                    initialHovered = menu.Bounds.Contains(mousePos);
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowMultiSelect: Initial hover for {menuId}: {initialHovered} at {mousePos}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowMultiSelect: Error checking initial hover - {ex.Message}");
                }
                lock (_lock)
                {
                    if (_activeMenus.ContainsKey(menuId))
                    {
                        _menusHovered[menuId] = initialHovered;
                    }
                }
                wasHovered = initialHovered;

                // NOW start the poller
                mousePoller.Start();

                lock (_lock)
                {
                    if (_clickOutsideFilter == null)
                    {
                        _clickOutsideFilter = new ClickOutsideFilter();
                        Application.AddMessageFilter(_clickOutsideFilter);
                    }
                }

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
        public static void CloseMenu(string menuId)
        {
            if (string.IsNullOrEmpty(menuId))
                return;
            lock (_lock)
            {
                if (_activeMenus.ContainsKey(menuId))
                {
                    // **CRITICAL**: Mark as closing BEFORE any actual close operations
                    _activeMenus[menuId].CloseInitiated = true;
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
        public static void CloseAllMenus()
        {
            lock (_lock)
            {
                // CRITICAL: Prevent re-entrancy
                if (_isGlobalClosing || _activeMenus.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] CloseAllMenus: Aborting - isClosing={_isGlobalClosing}, count={_activeMenus.Count}");
                    return;
                }
                _isGlobalClosing = true;
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] CloseAllMenus: STARTING close sequence for {_activeMenus.Count} menu(s)");
                // Stop timers FIRST to prevent re-triggering
                StopAllTimers();
                var menuIds = _activeMenus.Keys.ToList();
                foreach (var id in menuIds)
                {
                    try
                    {
                        CloseMenu(id);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] CloseAllMenus: Error closing {id} - {ex.Message}");
                    }
                }
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
                _activeMenus.Clear();
                _parentChildRelationships.Clear();
                _menusHovered.Clear();
                _isGlobalClosing = false;
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] CloseAllMenus: COMPLETED");
            }
        }
        public static bool HasChildren(SimpleItem item)
        {
            return item?.Children != null && item.Children.Count > 0;
        }
        #endregion
        #region Sub-Menu Support
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
                if (_isGlobalClosing)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Aborting - global closing in progress");
                    return null;
                }
                var context = new MenuContext
                {
                    Id = menuId,
                    MultiSelect = false,
                    ParentMenuId = parentMenuId
                };
                if (!string.IsNullOrEmpty(parentMenuId) && _activeMenus.ContainsKey(parentMenuId))
                {
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
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Creating BeepContextMenu for menu {menuId}");
            var menu = new BeepContextMenu
            {
                ContextMenuType = style,
                DestroyOnClose = false,
                MultiSelect = false,
                ShowCheckBox = false,
                StartPosition = FormStartPosition.Manual,
                CloseOnFocusLost = false
            };
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
            menu.RecalculateSize();
            lock (_lock)
            {
                _activeMenus[menuId].Menu = menu;
                // Do NOT set _menusHovered here�calculate after show
            }

            // **BULLETPROOF MOUSE TRACKING FOR CHILD MENUS**
            Timer mousePoller = new Timer { Interval = 50 };
            bool wasHovered=false; // Declare here but set after show
            mousePoller.Tick += (s, e) =>
            {
                if (menu.IsDisposed || !menu.Visible)
                {
                    mousePoller.Stop();
                    mousePoller.Dispose();
                    return;
                }
                lock (_lock)
                {
                    if (!_activeMenus.TryGetValue(menuId, out var context) || context.CloseInitiated)
                    {
                        mousePoller.Stop();
                        mousePoller.Dispose();
                        return;
                    }
                }
                Point mousePos = Cursor.Position;
                bool isHovered = menu.Bounds.Contains(mousePos);
                if (wasHovered && !isHovered)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] CHILD MOUSE LEAVE DETECTED for menu {menuId}");
                    lock (_lock)
                    {
                        _menusHovered[menuId] = false;
                    }
                    StartBufferTimer();
                    wasHovered = false;
                }
                else if (!wasHovered && isHovered)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] CHILD MOUSE ENTER DETECTED for menu {menuId}");
                    lock (_lock)
                    {
                        _menusHovered[menuId] = true;
                    }
                    StopAllTimers();
                    wasHovered = true;
                }
            };

            EventHandler<MenuItemEventArgs> itemClickedHandler = null;
            EventHandler<MenuItemEventArgs> submenuOpeningHandler = null;
            FormClosedEventHandler formClosedHandler = null;
            itemClickedHandler = (sender, e) =>
            {
                if (!HasChildren(e.Item))
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Child menu item clicked: {e.Item.DisplayField}");
                    lock (_lock)
                    {
                        if (_activeMenus.ContainsKey(menuId))
                        {
                            _activeMenus[menuId].SelectedItem = e.Item;
                            string rootMenuId = menuId;
                            while (!string.IsNullOrEmpty(rootMenuId) && _activeMenus.ContainsKey(rootMenuId))
                            {
                                var parentId = _activeMenus[rootMenuId].ParentMenuId;
                                if (string.IsNullOrEmpty(parentId))
                                    break;
                                rootMenuId = parentId;
                            }
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
            submenuOpeningHandler = (sender, e) =>
            {
                if (e?.Item == null || !HasChildren(e.Item))
                    return;
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Child SubmenuOpening: Item={e.Item.DisplayField}");
                StopAllTimers();
                CloseAllChildMenus(menuId);
                var menuLocation = menu.PointToScreen(Point.Empty);
                var itemIndex = menu.MenuItems.IndexOf(e.Item);
                var itemBounds = new Rectangle(0, itemIndex * menu.PreferredItemHeight, menu.Width, menu.PreferredItemHeight);
                var subMenuLocation = CalculateSubMenuPosition(menuLocation, itemBounds);
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
            formClosedHandler = (sender, e) =>
            {
                mousePoller.Stop();
                mousePoller.Dispose();
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
                menu.FormClosed -= formClosedHandler;
                CleanupMenuContext(menuId);
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
            menu.FormClosed += formClosedHandler;
            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: About to show menu {menuId} at {screenLocation}");
            try
            {
                if (!menu.IsHandleCreated)
                {
                    var handle = menu.Handle;
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Handle created - {handle}");
                }
                menu.Location = screenLocation;
                menu.Show();
                menu.BringToFront();
                menu.Activate();
                menu.Focus();
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Child menu activated and focused");
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: ObjectDisposedException - {ex.Message}");
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
                lock (_lock)
                {
                    if (_activeMenus.ContainsKey(menuId))
                    {
                        _activeMenus.Remove(menuId);
                    }
                }
                return null;
            }

            // NOW calculate initial hover state
            Point mousePos = Cursor.Position;
            bool initialHovered = false;
            try
            {
                if (!menu.IsDisposed)
                {
                    initialHovered = menu.Bounds.Contains(mousePos);
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Initial hover for {menuId}: {initialHovered} at {mousePos}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] ShowChildMenu: Error checking initial hover - {ex.Message}");
            }
            lock (_lock)
            {
                if (_activeMenus.ContainsKey(menuId))
                {
                    _menusHovered[menuId] = initialHovered;
                }
            }
            wasHovered = initialHovered;

            // NOW start the poller
            mousePoller.Start();

            return menuId;
        }
        private static void StartBufferTimer()
        {
            lock (_lock)
            {
                if (_isGlobalClosing || _bufferTimer != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] StartBufferTimer: Skipped - isClosing={_isGlobalClosing}, timerExists={_bufferTimer != null}");
                    return;
                }
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] StartBufferTimer: Mouse left a menu, starting 200ms buffer");
                _bufferTimer = new Timer { Interval = 200 };
                _bufferTimer.Tick += (sender, e) =>
                {
                    _bufferTimer.Stop();
                    _bufferTimer.Dispose();
                    _bufferTimer = null;
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Buffer timer expired");
                    lock (_lock)
                    {
                        bool anyMenuHovered = _menusHovered.Values.Any(isHovered => isHovered);
                        if (anyMenuHovered)
                        {
                            var hoveredMenus = string.Join(", ", _menusHovered.Where(kvp => kvp.Value).Select(kvp => kvp.Key));
                            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Mouse is over menu(s): {hoveredMenus}, NOT starting close timer");
                            return;
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Mouse outside ALL menus, starting CLOSE TIMER");
                    StartCloseTimer();
                };
                _bufferTimer.Start();
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Buffer timer STARTED successfully");
            }
        }
        private static void StartCloseTimer()
        {
            lock (_lock)
            {
                if (_isGlobalClosing || _closeTimer != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] StartCloseTimer: Skipped - isClosing={_isGlobalClosing}, timerExists={_closeTimer != null}");
                    return;
                }
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] StartCloseTimer: Starting 500ms close timer");
                _closeTimer = new Timer { Interval = 500 };
                _closeTimer.Tick += (sender, e) =>
                {
                    _closeTimer.Stop();
                    _closeTimer.Dispose();
                    _closeTimer = null;
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Close timer expired");
                    lock (_lock)
                    {
                        bool anyMenuHovered = _menusHovered.Values.Any(isHovered => isHovered);
                        if (anyMenuHovered)
                        {
                            var hoveredMenus = string.Join(", ", _menusHovered.Where(kvp => kvp.Value).Select(kvp => kvp.Key));
                            System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Mouse returned to menu(s): {hoveredMenus}, NOT closing");
                            return;
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Mouse still outside ALL menus, closing ALL menus");
                    CloseAllMenus();
                };
                _closeTimer.Start();
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] Close timer STARTED successfully");
            }
        }
        private static void StopAllTimers()
        {
            lock (_lock)
            {
                System.Diagnostics.Debug.WriteLine($"[ContextMenuManager] StopAllTimers: Canceling all timers");
                if (_bufferTimer != null)
                {
                    _bufferTimer.Stop();
                    _bufferTimer.Dispose();
                    _bufferTimer = null;
                }
                if (_closeTimer != null)
                {
                    _closeTimer.Stop();
                    _closeTimer.Dispose();
                    _closeTimer = null;
                }
            }
        }
        private static Point CalculateSubMenuPosition(Point parentMenuLocation, Rectangle parentItemBounds)
        {
            var x = parentMenuLocation.X + parentItemBounds.Right - 5;
            var y = parentMenuLocation.Y + parentItemBounds.Top;
            var screen = Screen.FromPoint(new Point(x, y));
            var screenBounds = screen.WorkingArea;
            const int estimatedMenuWidth = 250;
            const int estimatedMenuHeight = 300;
            if (x + estimatedMenuWidth > screenBounds.Right)
            {
                x = parentMenuLocation.X - estimatedMenuWidth + 5;
                if (x < screenBounds.Left)
                    x = screenBounds.Left + 5;
            }
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
                    var childIds = _activeMenus[menuId].ChildMenuIds.ToList();
                    foreach (var childId in childIds)
                    {
                        CloseMenu(childId);
                    }
                    if (!string.IsNullOrEmpty(_activeMenus[menuId].ParentMenuId) && _activeMenus.ContainsKey(_activeMenus[menuId].ParentMenuId))
                    {
                        _activeMenus[_activeMenus[menuId].ParentMenuId].ChildMenuIds.Remove(menuId);
                    }
                    _activeMenus.Remove(menuId);
                    _parentChildRelationships.Remove(menuId);
                    _menusHovered.Remove(menuId);
                }
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
        private static void CloseMenuHierarchy(string menuId)
        {
            if (string.IsNullOrEmpty(menuId))
                return;
            lock (_lock)
            {
                string rootMenuId = menuId;
                while (!string.IsNullOrEmpty(rootMenuId) && _activeMenus.ContainsKey(rootMenuId))
                {
                    var parentId = _activeMenus[rootMenuId].ParentMenuId;
                    if (string.IsNullOrEmpty(parentId))
                        break;
                    rootMenuId = parentId;
                }
                if (!string.IsNullOrEmpty(rootMenuId))
                {
                    CloseMenu(rootMenuId);
                }
            }
        }
        #endregion
    }
}